using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwoPS.Processes
{
    /// <summary>
    /// Represents the results of running an executable file
    /// </summary>
    public class ProcessResult
    {
        /// <summary>
        /// Creates a new instance of the ProcessResult class
        /// </summary>
        /// <param name="process">The process that was run</param>
        /// <param name="commandLine">The command line</param>
        public ProcessResult(Process process, string commandLine)
        {
            process.StandardOutputRead += new EventHandler<ProcessEventArgs>(process_StandardOutputRead);
            process.StandardErrorRead += new EventHandler<ProcessEventArgs>(process_StandardErrorRead);
            CommandLine = commandLine;
            ExitCode = -1;
            Status = ProcessStatus.NotStarted;
            StandardOutputList = new List<string>();
            StandardErrorList = new List<string>();
            AllOutputList = new List<string>();
        }

        /// <summary>
        /// The command line
        /// </summary>
        public string CommandLine { get; private set; }

        /// <summary>
        /// The exit code
        /// </summary>
        public int ExitCode { get; internal set; }

        /// <summary>
        /// The last state of the process
        /// </summary>
        public ProcessStatus Status { get; internal set; }

        /// <summary>
        /// Specifies whether the process ran successfully
        /// </summary>
        public bool Success
        {
            get { return Status == ProcessStatus.Finished; }
        }

        /// <summary>
        /// Standard output from the process split into lines
        /// </summary>
        public List<string> StandardOutputList { get; private set; }

        /// <summary>
        /// Standard error from the process split into lines
        /// </summary>
        public List<string> StandardErrorList { get; private set; }

        /// <summary>
        /// Standard output and standard error from the process split into lines
        /// </summary>
        public List<string> AllOutputList { get; private set; }

        /// <summary>
        /// Standard output from the process
        /// </summary>
        public string StandardOutput
        {
            get { return string.Join(Environment.NewLine, StandardOutputList.ToArray()); }
        }

        /// <summary>
        /// Standard error from the process
        /// </summary>
        public string StandardError
        {
            get { return string.Join(Environment.NewLine, StandardErrorList.ToArray()); }
        }

        /// <summary>
        /// Standard output and standard error from the process
        /// </summary>
        public string AllOutput
        {
            get { return string.Join(Environment.NewLine, AllOutputList.ToArray()); }
        }

        private void process_StandardOutputRead(object sender, ProcessEventArgs e)
        {
            StandardOutputList.Add(e.Line);
            AllOutputList.Add(e.Line);
        }

        private void process_StandardErrorRead(object sender, ProcessEventArgs e)
        {
            StandardErrorList.Add(e.Line);
            AllOutputList.Add(e.Line);
        }
    }
}
