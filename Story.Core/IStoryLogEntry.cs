namespace Story.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public interface IStoryLogEntry
    {
        /// <summary>
        /// 
        /// </summary>
        LogSeverity Severity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        TimeSpan Offset { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        DateTime DateTime { get; set; }
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
