using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace TwoPS.Processes
{
    /// <summary>
    /// The exception that is thrown when a process error occurs.
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    public class ProcessRunnerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRunnerException"/> class.
        /// </summary>
        public ProcessRunnerException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRunnerException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public ProcessRunnerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRunnerException"/> class
        /// with a specified error message formated using <see cref="String.Format(string, object[])"/>.
        /// </summary>
        /// <param name="format">A composite format string for the message.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public ProcessRunnerException(string format, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, format, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRunnerException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of
        /// this exception.        
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="exception">
        /// The exception that is the cause of the current exception, or a null reference
        /// if no inner exception is specified.
        /// </param>
        public ProcessRunnerException(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRunnerException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected ProcessRunnerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
