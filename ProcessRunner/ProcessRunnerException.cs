using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TwoPS.Processes
{
    public class ProcessRunnerException : ApplicationException
    {
        public ProcessRunnerException()
        {
        }

        public ProcessRunnerException(string message)
            : base(message)
        {
        }

        public ProcessRunnerException(string format, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, format, args))
        {
        }

        public ProcessRunnerException(string message, Exception exception)
            : base(message, exception)
        {
        }

        protected ProcessRunnerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
