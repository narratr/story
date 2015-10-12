namespace Story.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IStoryLog : IEnumerable<IStoryLogEntry>
    {
        void Debug(string format, params object[] args);

        void Info(string format, params object[] args);

        void Warn(string format, params object[] args);

        void Error(string format, params object[] args);

        void Critical(string format, params object[] args);

        void Add(LogSeverity severity, string format, params object[] args);
    }
}
