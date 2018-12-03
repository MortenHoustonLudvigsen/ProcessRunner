namespace TwoPS.Processes
{
    /// <summary>
    /// The types of process events
    /// </summary>
    public enum ProcessEventType
    {
        /// <summary>
        /// An event is raise when a process is started
        /// </summary>
        Started,

        /// <summary>
        /// An event is raised when a line of text is read from thes process' standard input
        /// </summary>
        StandardOutputRead,

        /// <summary>
        /// An event is raised when a line of text is read from thes process' standard error
        /// </summary>
        StandardErrorRead
    }
}
