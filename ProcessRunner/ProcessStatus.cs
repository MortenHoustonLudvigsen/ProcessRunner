using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwoPS.Processes
{
    /// <summary>
    /// Specifies what state the process is in
    /// </summary>
    public enum ProcessStatus
    {
        /// <summary>
        /// The process has not started
        /// </summary>
        NotStarted,

        /// <summary>
        /// The process has started
        /// </summary>
        Started,

        /// <summary>
        /// The process has stopped because it timed out
        /// </summary>
        TimedOut,

        /// <summary>
        /// The process has stopped because it was cancelled
        /// </summary>
        Cancelled,

        /// <summary>
        /// The process has stopped because it was finished
        /// </summary>
        Finished
    }
}
