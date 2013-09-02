using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ProcessRunner
{
    public class BatchRunner
    {
        private TextWriter _logger = TextWriter.Null;
        public TextWriter Logger
        {
            get { return _logger; }
            set { _logger = value ?? TextWriter.Null; }
        }

        protected void Log(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                Log(line);
            }
        }

        protected void Log(string line)
        {
            Logger.WriteLine(line);
        }

        protected void Log(string line, params object[] args)
        {
            Logger.WriteLine(string.Format(line, args));
        }

        public event EventHandler<ProcessEventArgs> ProcessStarted;
        public event EventHandler<ProcessEventArgs> StandardOutputRead;
        public event EventHandler<ProcessEventArgs> StandardErrorRead;

        protected void RegisterProcessRunners(params ProcessRunner[] processRunners)
        {
            foreach (var processRunner in processRunners)
            {
                processRunner.Started += new EventHandler<ProcessEventArgs>(processRunner_Started);
                processRunner.StandardOutputRead += new EventHandler<ProcessEventArgs>(processRunner_StandardOutputRead);
                processRunner.StandardErrorRead += new EventHandler<ProcessEventArgs>(processRunner_StandardErrorRead);
            }
        }

        protected void OnEvent(EventHandler<ProcessEventArgs> eventHandler, ProcessEventArgs eventArgs)
        {
            if (eventHandler != null)
            {
                eventHandler(this, eventArgs);
            }
        }

        private void processRunner_Started(object sender, ProcessEventArgs e)
        {
            Log(e.Process.Options.CommandLine);
            OnEvent(ProcessStarted, e);
        }

        private void processRunner_StandardOutputRead(object sender, ProcessEventArgs e)
        {
            if (e.Process.Options.LogStandardOutput)
            {
                Log("  OUT: {0}", e.Line);
            }
            OnEvent(StandardOutputRead, e);
        }

        private void processRunner_StandardErrorRead(object sender, ProcessEventArgs e)
        {
            if (e.Process.Options.LogStandardError)
            {
                Log("  ERR: {0}", e.Line);
            }
            OnEvent(StandardErrorRead, e);
        }
    }
}
