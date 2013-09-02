using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessRunner
{
    /// <summary>
    /// Represents options for starting a process
    /// </summary>
    public class ProcessOptions
    {
        private List<string> _options = new List<string>();

        /// <summary>
        /// Creates a new instance of the ProcessOptions class
        /// </summary>
        public ProcessOptions()
        {
            Timeout = 0;
            LogStandardOutput = true;
            LogStandardError = true;
        }

        /// <summary>
        /// Creates a new instance of the ProcessOptions class
        /// </summary>
        /// <param name="fileName">The path to the executable file to be started</param>
        /// <param name="arguments">A number of arguments to add to the command line</param>
        public ProcessOptions(string fileName, params string[] arguments)
            : this(fileName, (IEnumerable<string>)arguments, 0)
        {
        }

        /// <summary>
        /// Creates a new instance of the ProcessOptions class
        /// </summary>
        /// <param name="fileName">The path to the executable file to be started</param>
        /// <param name="arguments">A number of arguments to add to the command line</param>
        public ProcessOptions(string fileName, IEnumerable<string> arguments)
            : this(fileName, arguments, 0)
        {
        }

        /// <summary>
        /// Creates a new instance of the ProcessOptions class
        /// </summary>
        /// <param name="fileName">The path to the executable file to be started</param>
        /// <param name="arguments">A number of arguments to add to the command line</param>
        /// <param name="timeout">The timeout for the process</param>
        public ProcessOptions(string fileName, IEnumerable<string> arguments, int timeout)
        {
            FileName = fileName;
            foreach (string argument in arguments)
            {
                Add(argument);
            }
            Timeout = timeout;
            LogStandardOutput = true;
            LogStandardError = true;
        }

        /// <summary>
        /// Specifies whether to log standard output from the process
        /// </summary>
        public bool LogStandardOutput { get; set; }

        /// <summary>
        /// Specifies whether to log standard error from the process
        /// </summary>
        public bool LogStandardError { get; set; }

        /// <summary>
        /// Specifies the encoding of standard output from the process
        /// </summary>
        public Encoding StandardOutputEncoding { get; set; }

        /// <summary>
        /// Specifies the encoding of standard error from the process
        /// </summary>
        public Encoding StandardErrorEncoding { get; set; }

        /// <summary>
        /// The path to the executable file to be started
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The timeout for the process
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// The working directory for the process
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// The command line arguments
        /// </summary>
        public string Arguments
        {
            get { return string.Join(" ", _options.ToArray()); }
        }

        /// <summary>
        /// The full command line for the process
        /// </summary>
        public string CommandLine
        {
            get { return ("\"" + FileName + "\" " + Arguments).Trim(); }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            string str = CommandLine;
            if (Timeout > 0)
            {
                str += string.Format(" (Timeout: {0})", Timeout);
            }
            return base.ToString();
        }


        private System.Text.StringBuilder _standardInput = new System.Text.StringBuilder();
        /// <summary>
        /// Append text to be sent to the process via standard input
        /// </summary>
        /// <param name="text">The text</param>
        public void StandardInputAppend(string text)
        {
            _standardInput.Append(text);
        }

        /// <summary>
        /// Append lines to be sent to the process via standard input
        /// </summary>
        /// <param name="lines">The lines</param>
        public void StandardInputAppendLines(IEnumerable<string> lines)
        {
            foreach (string line in lines)
            {
                _standardInput.AppendLine(line);
            }
        }

        /// <summary>
        /// Append lines to be sent to the process via standard input
        /// </summary>
        /// <param name="lines">The lines</param>
        public void StandardInputAppendLines(params string[] lines)
        {
            StandardInputAppendLines((IEnumerable<string>)lines);
        }

        /// <summary>
        /// The text to be sent to the process via standard input
        /// </summary>
        public string StandardInput
        {
            get { return _standardInput.ToString(); }
        }

        /// <summary>
        /// Specifies whether there is any text to be sent to the process via standard input
        /// </summary>
        public bool HasStandardInput
        {
            get { return _standardInput.Length > 0; }
        }

        /// <summary>
        /// Add a number of command line arguments
        /// </summary>
        /// <param name="arguments">The command line arguments to be added</param>
        /// <returns>True if at least one argument was added</returns>
        /// <remarks>
        /// <para>An argument will not be added if it is null or an empty string.</para>
        /// <para>If an argument contains a space it will be quoted.</para>
        /// </remarks>
        public bool Add(params string[] arguments)
        {
            return Add(arguments as IEnumerable<string>);
        }

        /// <summary>
        /// Add a number of command line arguments
        /// </summary>
        /// <param name="arguments">The command line arguments to be added</param>
        /// <returns>True if at least one argument was added</returns>
        /// <remarks>
        /// <para>An argument will not be added if it is null or an empty string.</para>
        /// <para>If an argument contains a space it will be quoted.</para>
        /// </remarks>
        public bool Add(IEnumerable<string> arguments)
        {
            bool result = false;
            foreach (string argument in arguments)
            {
                result = Add(argument) || result;
            }
            return result;
        }

        /// <summary>
        /// Add a command line argument
        /// </summary>
        /// <param name="value">The command line argument to be added</param>
        /// <returns>True if the argument was added</returns>
        /// <remarks>
        /// <para>The argument will not be added if <paramref name="value"/> is null or converts to an empty string.</para>
        /// <para>If <paramref name="value"/> contains a space it will be quoted.</para>
        /// </remarks>
        public bool Add(object value)
        {
            return value != null ? Add(value.ToString()) : false;
        }

        /// <summary>
        /// Add a command line argument
        /// </summary>
        /// <param name="value">The command line argument to be added</param>
        /// <returns>True if the argument was added</returns>
        /// <remarks>
        /// <para>The argument will not be added if <paramref name="value"/> is null or an empty string.</para>
        /// <para>If <paramref name="value"/> contains a space it will be quoted.</para>
        /// </remarks>
        public bool Add(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains(" "))
                {
                    value = "\"" + value + "\"";
                }
                _options.Add(value);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Add two command line arguments: <paramref name="option"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="option">The command line option to be added</param>
        /// <param name="value">The option value</param>
        /// <returns>True if the option was added</returns>
        /// <remarks>
        /// <para>The option will not be added if either <paramref name="option"/> or <paramref name="value"/> is null or an empty string.</para>
        /// <para>If <paramref name="option"/> contains a space it will be quoted.</para>
        /// <para>If <paramref name="value"/> contains a space it will be quoted.</para>
        /// </remarks>
        public bool Add(string option, object value)
        {
            return value != null ? Add(option, value.ToString()) : false;
        }

        /// <summary>
        /// Add two command line arguments: <paramref name="option"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="option">The command line option to be added</param>
        /// <param name="value">The option value</param>
        /// <returns>True if the option was added</returns>
        /// <remarks>
        /// <para>The option will not be added if either <paramref name="option"/> or <paramref name="value"/> is null or an empty string.</para>
        /// <para>If <paramref name="option"/> contains a space it will be quoted.</para>
        /// <para>If <paramref name="value"/> contains a space it will be quoted.</para>
        /// </remarks>
        public bool Add(string option, string value)
        {
            if (!string.IsNullOrEmpty(option) && !string.IsNullOrEmpty(value))
            {
                Add(option);
                Add(value);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Add a command line option if the <paramref name="value"/> is true.
        /// </summary>
        /// <param name="option">The command line option to be added</param>
        /// <param name="value">Specifies whether to add the option</param>
        /// <returns>True if the option was added</returns>
        /// <remarks>
        /// <para>If <paramref name="option"/> contains a space it will be quoted.</para>
        /// </remarks>
        public bool Add(string option, bool? value)
        {
            if (value.HasValue && value.Value)
            {
                return Add(option);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Add a command line arguments composed of <paramref name="option"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="option">The command line option to be added</param>
        /// <param name="value">The option value</param>
        /// <returns>True if the option was added</returns>
        /// <remarks>The option will not be added if either <paramref name="option"/> or <paramref name="value"/> is null or an empty string.</remarks>
        /// <remarks>
        /// <para>The option will not be added if either <paramref name="option"/> or <paramref name="value"/> is null or an empty string.</para>
        /// <para>If <paramref name="option"/> or <paramref name="value"/> contains a space the argument will be quoted.</para>
        /// </remarks>
        public bool AddSwitch(string option, string value)
        {
            if (!string.IsNullOrEmpty(option) && !string.IsNullOrEmpty(value))
            {
                return Add(option + value);
            }
            else
            {
                return false;
            }
        }
    }
}
