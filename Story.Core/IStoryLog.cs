namespace Story.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IStoryLog : IEnumerable<IStoryLogEntry>
    {
        void Debug(string format, object[] args = null, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null, [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null, [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null);

        void Info(string format, object[] args = null, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null, [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null, [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null);

        void Warn(string format, object[] args = null, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null, [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null, [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null);

        void Error(string format, object[] args = null, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null, [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null, [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null);

        void Critical(string format, object[] args = null, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null, [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null, [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null);

        void Add(
            LogSeverity severity,
            string format,
            object[] args = null,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
            [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null);
    }
}
