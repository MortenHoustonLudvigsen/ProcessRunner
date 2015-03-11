using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwoPS.Processes
{
    /// <summary>
    /// Event args for an event that is raised when the start info for the process is created
    /// </summary>
    public class StartInfoCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="StartInfoCreatedEventArgs"/> class
        /// </summary>
        /// <param name="process">The process</param>
        /// <param name="startInfo">The <see cref="System.Diagnostics.ProcessStartInfo"/> for the process</param>
        public StartInfoCreatedEventArgs(Process process, System.Diagnostics.ProcessStartInfo startInfo)
        {
            Process = process;
            StartInfo = startInfo;
        }

        /// <summary>
        /// The Process
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        /// The StartInfo
        /// </summary>
        public System.Diagnostics.ProcessStartInfo StartInfo { get; private set; }
    }
}
