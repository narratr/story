namespace Story.Core
{
    using System;
    using System.Linq;

    /// <summary>
    /// An interface describing a log entry.
    /// </summary>
    public interface IStoryLogEntry
    {
        /// <summary>
        /// Severity of log entry.
        /// </summary>
        LogSeverity Severity { get; }

        /// <summary>
        /// Text of log entry.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Time elapsed since Story was started.
        /// </summary>
        TimeSpan Elapsed { get; }
    }

    public enum LogSeverity
    {
        /// <summary>
        /// Unspecified log level.
        /// </summary>
        Unspecified,

        /// <summary>
        /// Debug log level.
        /// </summary>
        Debug,

        /// <summary>
        /// Information log level.
        /// </summary>
        Info,

        /// <summary>
        /// Warning log level.
        /// </summary>
        Warning,

        /// <summary>
        /// Error log level.
        /// </summary>
        Error,

        /// <summary>
        /// Critical log level.
        /// </summary>
        Critical,
    }
}
