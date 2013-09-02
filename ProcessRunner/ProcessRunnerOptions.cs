using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessRunner
{
    /// <summary>
    /// Represents options for <see cref="ProcessRunner.ProcessRunner"/>
    /// </summary>
    public abstract class ProcessRunnerOptions
    {
        /// <summary>
        /// Creates a new instance of the ProcessRunnerOptions class
        /// </summary>
        public ProcessRunnerOptions()
        {
            LogStandardOutput = true;
            LogStandardError = true;
        }

        /// <summary>
        /// An delegate, that gets called to add command line arguments
        /// </summary>
        public Action<ProcessOptions> OtherArguments { get; set; }

        /// <summary>
        /// An delegate, that gets called to add standard input
        /// </summary>
        public Action<ProcessOptions> WriteStandardInput { get; set; }

        /// <summary>
        /// Specifies whether to log standard output
        /// </summary>
        public bool LogStandardOutput { get; set; }

        /// <summary>
        /// Specifies whether to log standard error
        /// </summary>
        public bool LogStandardError { get; set; }

        /// <summary>
        /// This method gets called to set options for the process
        /// </summary>
        /// <param name="processOptions">A <see cref="ProcessRunner.ProcessOptions"/> object, that contains the options for the process</param>
        public abstract void SetProcessOptions(ProcessOptions processOptions);

        /// <summary>
        /// This method gets called to add extra command line arguments by calling <see cref="OtherArguments"/> if it is set.
        /// </summary>
        /// <param name="processOptions">A <see cref="ProcessRunner.ProcessOptions"/> object, that contains the options for the process</param>
        /// <remarks><para>This method will typically be called from <see cref="SetProcessOptions"/> in derived classes.</para></remarks>
        protected void AddOtherArguments(ProcessOptions processOptions)
        {
            if (OtherArguments != null)
            {
                OtherArguments(processOptions);
            }
        }

        /// <summary>
        /// This method gets called to add standard input by calling <see cref="WriteStandardInput"/> if it is set.
        /// </summary>
        /// <param name="processOptions">A <see cref="ProcessRunner.ProcessOptions"/> object, that contains the options for the process</param>
        /// <remarks><para>This method will typically be called from <see cref="SetProcessOptions"/> in derived classes.</para></remarks>
        protected void AddStandardInput(ProcessOptions processOptions)
        {
            if (WriteStandardInput != null)
            {
                WriteStandardInput(processOptions);
            }
        }

        internal void SetProcessRunnerOptions(ProcessOptions processOptions)
        {
            processOptions.LogStandardOutput = LogStandardOutput;
            processOptions.LogStandardError = LogStandardError;
            SetProcessOptions(processOptions);
        }
    }
}
