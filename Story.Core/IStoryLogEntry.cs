namespace Story.Core
{
    using System;
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
    public interface IStoryLogEntry
    {
        /// <summary>
        ///
        /// </summary>
        LogSeverity Severity { get; }

        /// <summary>
        ///
        /// </summary>
        string Text { get; }

        /// <summary>
        ///
        /// </summary>
        TimeSpan Offset { get; }

        /// <summary>
        ///
        /// </summary>
        DateTime DateTime { get; }
    }

    public enum LogSeverity
    {
        /// <summary>
        ///
        /// </summary>
        Unspecified,

        /// <summary>
        ///
        /// </summary>
        Debug,

        /// <summary>
        ///
        /// </summary>
        Info,

        /// <summary>
        ///
        /// </summary>
        Warning,

        /// <summary>
        ///
        /// </summary>
        Error,

        /// <summary>
        ///
        /// </summary>
        Critical,
    }
}
