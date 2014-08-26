using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TwoPS.Processes
{
    /// <summary>
    /// Represents the execution of an executable file
    /// </summary>
    public class Process
    {
        /// <summary>
        /// An event is raised when the process starts
        /// </summary>
        public event EventHandler<ProcessEventArgs> Started;

        /// <summary>
        /// An event is raised when a line has been read from standard output of the process
        /// </summary>
        public event EventHandler<ProcessEventArgs> StandardOutputRead;

        /// <summary>
        /// An event is raised when a line has been read from standard error of the process
        /// </summary>
        public event EventHandler<ProcessEventArgs> StandardErrorRead;

        private void OnEvent(ProcessEventArgs eventArgs)
        {
            switch (eventArgs.EventType)
            {
                case ProcessEventType.Started:
                    OnEvent(Started, eventArgs);
                    break;
                case ProcessEventType.StandardOutputRead:
                    OnEvent(StandardOutputRead, eventArgs);
                    break;
                case ProcessEventType.StandardErrorRead:
                    OnEvent(StandardErrorRead, eventArgs);
                    break;
            }
        }

        private void OnEvent(EventHandler<ProcessEventArgs> eventHandler, ProcessEventArgs eventArgs)
        {
            if (eventHandler != null)
            {
                eventHandler(this, eventArgs);
            }
        }

        /// <summary>
        /// Runs an executable file, as specified in <paramref name="options"/>
        /// </summary>
        /// <param name="options">Specifies what to run</param>
        /// <returns>The result of running the executable file</returns>
        public static ProcessResult Run(ProcessOptions options)
        {
            return (new Process(options)).Run();
        }

        /// <summary>
        /// Runs an executable file, as specified by <paramref name="fileName"/> and <paramref name="arguments" />
        /// </summary>
        /// <param name="fileName">The path to the executable file to be run</param>
        /// <param name="arguments">Command line arguments</param>
        /// <returns>The result of running the executable file</returns>
        public static ProcessResult Run(string fileName, params string[] arguments)
        {
            return Run(fileName, 0, arguments);
        }

        /// <summary>
        /// Runs an executable file, as specified by <paramref name="fileName"/> and <paramref name="arguments" />
        /// </summary>
        /// <param name="fileName">The path to the executable file to be run</param>
        /// <param name="timeout">The time out</param>
        /// <param name="arguments">Command line arguments</param>
        /// <returns>The result of running the executable file</returns>
        public static ProcessResult Run(string fileName, int timeout, params string[] arguments)
        {
            Process process = new Process(fileName, arguments);
            process.Timeout = timeout;
            process.Started += new EventHandler<ProcessEventArgs>(delegate(object sender, ProcessEventArgs e)
            {
                e.Process.Cancel();
            });
            return process.Run();
        }

        /// <summary>
        /// Options specifying what to run
        /// </summary>
        public ProcessOptions Options { get; private set; }

        private System.Diagnostics.Process _process;
        private DateTime _endTime;
        private Queue<ProcessEventArgs> _events = new Queue<ProcessEventArgs>();

        private int _timeout = 0;
        /// <summary>
        /// The time out
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set
            {
                CheckNotStarted();
                _timeout = value;
            }
        }

        private ProcessResult _result;
        /// <summary>
        /// The result of running the executable file
        /// </summary>
        public ProcessResult Result
        {
            get { return _result; }
        }

        private void CheckNotStarted()
        {
            lock (this)
            {
                if (_result.Status != ProcessStatus.NotStarted)
                {
                    throw new ProcessRunnerException("Process has already started");
                }
            }
        }

        /// <summary>
        /// Creates a new instance of the Process class
        /// </summary>
        /// <param name="fileName">The path to the executable file to be run</param>
        /// <param name="arguments">Command line arguments</param>
        public Process(string fileName, params string[] arguments)
            : this(new ProcessOptions(fileName, (IEnumerable<string>)arguments))
        {
        }

        /// <summary>
        /// Creates a new instance of the Process class
        /// </summary>
        /// <param name="options">Options specifying what to run</param>
        public Process(ProcessOptions options)
        {
            Options = options;
            _result = new ProcessResult(this, Options.CommandLine);
            _timeout = Options.Timeout;
        }

        /// <summary>
        /// Runs the executable file
        /// </summary>
        /// <returns>The result of running the executable file</returns>
        public ProcessResult Run()
        {
            lock (this)
            {
                _process = new System.Diagnostics.Process();
                _process.StartInfo.UseShellExecute = false;
                _process.StartInfo.RedirectStandardInput = true;
                _process.StartInfo.RedirectStandardOutput = true;
                _process.StartInfo.RedirectStandardError = true;
                _process.StartInfo.CreateNoWindow = true;
                _process.StartInfo.FileName = Options.FileName;
                _process.StartInfo.Arguments = Options.Arguments;
                if (Options.StandardOutputEncoding != null)
                {
                    _process.StartInfo.StandardOutputEncoding = Options.StandardOutputEncoding;
                }
                if (Options.StandardErrorEncoding != null)
                {
                    _process.StartInfo.StandardErrorEncoding = Options.StandardErrorEncoding;
                }
                if (!string.IsNullOrEmpty(Options.WorkingDirectory))
                {
                    _process.StartInfo.WorkingDirectory = Options.WorkingDirectory;
                }

                CheckNotStarted();

                _result.Status = ProcessStatus.Started;

                _endTime = System.DateTime.Now.AddSeconds(Timeout);

                _process.Start();
            }

            var inputThread = new Thread(new ThreadStart(WriteStandardInput));
            inputThread.Start();
            var outputReader = new ProcessReader(this, ProcessReader.OutputType.StandardOutput);
            var errorReader = new ProcessReader(this, ProcessReader.OutputType.StandardError);

            lock (this)
            {
                AddEvent(new ProcessEventArgs(ProcessEventType.Started, this));
                DoEvents();

                while (!_process.HasExited)
                {
                    Monitor.Wait(this, 250);
                    DoEvents();
                    if ((_timeout > 0) && (System.DateTime.Now > _endTime))
                    {
                        // Not a very nice way to end a process,
                        // but effective.
                        _process.Kill();
                        _result.Status = ProcessStatus.TimedOut;
                    }
                    else if (_result.Status == ProcessStatus.Cancelled)
                    {
                        _process.Kill();
                    }
                }
            }

            _process.WaitForExit();

            outputReader.Join();
            errorReader.Join();
            inputThread.Join();

            DoEvents();

            _result.ExitCode = _process.ExitCode;
            if (_result.Status == ProcessStatus.Started)
            {
                _result.Status = ProcessStatus.Finished;
            }

            return _result;
        }

        /// <summary>
        /// Cancels the running of the executable file
        /// </summary>
        public void Cancel()
        {
            lock (this)
            {
                if (_result.Status == ProcessStatus.Started)
                {
                    _result.Status = ProcessStatus.Cancelled;
                }
            }
        }

        private void WriteStandardInput()
        {
            if (Options.StandardInputEncoding != null)
            {
                using (var writer = new StreamWriter(_process.StandardInput.BaseStream, Options.StandardInputEncoding))
                {
                    WriteStandardInput(writer);
                }
            }
            else
            {
                WriteStandardInput(_process.StandardInput);
            }
        }

        private void WriteStandardInput(StreamWriter writer)
        {
            writer.Write(Options.StandardInput);
            writer.Close();
        }

        private void DoEvents()
        {
            lock (this)
            {
                while (_events.Count > 0)
                {
                    OnEvent(_events.Dequeue());
                }
            }
        }

        private void AddEvent(ProcessEventArgs eventArgs)
        {
            lock (this)
            {
                _events.Enqueue(eventArgs);
                Monitor.Pulse(this);
            }
        }

        private class ProcessReader
        {
            public enum OutputType
            {
                StandardOutput,
                StandardError
            }

            public ProcessReader(Process process, OutputType type)
            {
                _process = process;
                if (type == OutputType.StandardOutput)
                {
                    _reader = _process._process.StandardOutput;
                    _outputList = _process.Result.StandardOutputList;
                    _eventType = ProcessEventType.StandardOutputRead;
                }
                else
                {
                    _reader = _process._process.StandardError;
                    _outputList = _process.Result.StandardErrorList;
                    _eventType = ProcessEventType.StandardErrorRead;
                }
                _thread = new Thread(new ThreadStart(Read));
                _thread.Start();
            }

            private Process _process;
            private StreamReader _reader;
            private List<string> _outputList;
            private ProcessEventType _eventType;
            private Thread _thread;

            public void Join()
            {
                _thread.Join();
            }

            public void Read()
            {
                var isRunning = true;
                string line;
                while (isRunning && ((line = _reader.ReadLine()) != null))
                {
                    lock (_process)
                    {
                        if (_process.Result.Status == ProcessStatus.Started)
                        {
                            _process.AddEvent(new ProcessEventArgs(_eventType, _process, line));
                        }
                        else
                        {
                            isRunning = false;
                        }
                    }
                }
            }
        }
    }
}
