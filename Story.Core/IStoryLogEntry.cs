namespace Story.Core
{
    using System;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime.Serialization;

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
        /// Date and Time of log entry.
        /// </summary>
        DateTime DateTime { get; }

        /// <summary>
        /// Thread name when the log entry was created.
        /// </summary>
        string ThreadName { get; }

        /// <summary>
        /// File path where the log entry was created.
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Method name where the log entry was created.
        /// </summary>
        string MethodName { get; }

        /// <summary>
        /// Line number where the log entry was created.
        /// </summary>
        int? LineNumber { get; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogSeverity
    {
        /// <summary>
        /// Debug log level.
        /// </summary>
        [EnumMember(Value = "debug")]
        Debug,

        /// <summary>
        /// Information log level.
        /// </summary>
        [EnumMember(Value = "info")]
        Info,

        /// <summary>
        /// Warning log level.
        /// </summary>
        [EnumMember(Value = "warn")]
        Warning,

        /// <summary>
        /// Error log level.
        /// </summary>
        [EnumMember(Value = "error")]
        Error,

        /// <summary>
        /// Critical log level.
        /// </summary>
        [EnumMember(Value = "critical")]
        Critical,
    }
}
