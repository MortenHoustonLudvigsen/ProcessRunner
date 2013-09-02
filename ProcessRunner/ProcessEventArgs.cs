using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessRunner
{
    /// <summary>
    /// Represents event arguments from a running process
    /// </summary>
    public class ProcessEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the ProcessEventArgs class
        /// </summary>
        /// <param name="eventType">The type of event</param>
        /// <param name="process">The process</param>
        /// <param name="line">The line</param>
        public ProcessEventArgs(ProcessEventType eventType, Process process, string line)
        {
            EventType = eventType;
            Process = process;
            Line = line;
        }

        /// <summary>
        /// Creates a new instance of the ProcessEventArgs class
        /// </summary>
        /// <param name="eventType">The type of event</param>
        /// <param name="process">The process</param>
        public ProcessEventArgs(ProcessEventType eventType, Process process)
            : this(eventType, process, null)
        {
        }

        /// <summary>
        /// Type type of event
        /// </summary>
        public ProcessEventType EventType { get; private set; }

        /// <summary>
        /// The Process
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        /// If the event is an output from the process this property will contain the line of text
        /// </summary>
        public string Line { get; private set; }
    }
}
