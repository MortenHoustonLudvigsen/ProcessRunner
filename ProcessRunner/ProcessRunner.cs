using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessRunner
{
    /// <summary>
    /// Wraps the logic for calling an executable file
    /// </summary>
    public abstract class ProcessRunner
    {
        /// <summary>
        /// An event is raised when a process is started
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

        /// <summary>
        /// The executable file
        /// </summary>
        protected string ExeFile { get; private set; }

        /// <summary>
        /// Creates a new instance of the ProcessRunner class
        /// </summary>
        /// <param name="exeFile">The executable file</param>
        protected ProcessRunner(string exeFile)
        {
            ExeFile = exeFile;
        }

        /// <summary>
        /// Logs a line of text
        /// </summary>
        /// <param name="line">The line to be logged</param>
        protected void Log(string line)
        {
            OnEvent(this, new ProcessEventArgs(ProcessEventType.StandardOutputRead, null, line));
        }

        private void OnEvent(object sender, ProcessEventArgs eventArgs)
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
        /// Creates a new instance of <see cref="ProcessRunner.ProcessOptions"/> initialised with <see cref="ExeFile" />.
        /// </summary>
        /// <returns>The new <see cref="ProcessRunner.ProcessOptions"/></returns>
        protected virtual ProcessOptions CreateProcessOptions()
        {
            return new ProcessOptions(ExeFile);
        }

        /// <summary>
        /// Checks whether the execution has been succesful
        /// </summary>
        /// <param name="result">The result of running the process</param>
        /// <returns>True if succesful</returns>
        protected virtual bool HasSuccess(ProcessResult result)
        {
            return result.Success && (result.ExitCode == 0);
        }

        /// <summary>
        /// Runs an executable file, as specified by <paramref name="options"/>
        /// </summary>
        /// <param name="options">Specifies what to run</param>
        /// <returns>The result of running the process</returns>
        protected ProcessResult Run(ProcessRunnerOptions options)
        {
            var processOptions = CreateProcessOptions();
            options.SetProcessRunnerOptions(processOptions);
            var process = new Process(processOptions);
            process.Started += new EventHandler<ProcessEventArgs>(OnEvent);
            process.StandardOutputRead += new EventHandler<ProcessEventArgs>(OnEvent);
            process.StandardErrorRead += new EventHandler<ProcessEventArgs>(OnEvent);
            var result = process.Run();
            if (HasSuccess(result))
            {
                return result;
            }
            throw new ApplicationException(result.AllOutput);
        }
    }
}
