namespace Story.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IStoryLogEntry
    {
        LogSeverity Severity { get; set; }

        string Text { get; set; }

        TimeSpan Offset { get; set; }
        
        DateTime DateTime { get; set; }
    }

    public enum LogSeverity
    {
        Unspecified,
        Debug,
        Info,
        Warning,
        Error,
        Critical,
    }
}
