using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwoPS.Processes
{
    public class StartInfoCreatedEventArgs : EventArgs
    {
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
