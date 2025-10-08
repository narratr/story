namespace Story.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Utils;

    [Serializable]
    public class StoryLog : IStoryLog
    {
        private readonly List<IStoryLogEntry> entries = new List<IStoryLogEntry>();

        public StoryLog(IStory story)
        {
            Ensure.ArgumentNotNull(story, "story");
            this.Story = story;
        }

        protected IStory Story { get; private set; }

        public void Debug(
            string format,
            object[] args = null,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
            [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null)
        {
            this.Add(LogSeverity.Debug, format, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Info(
            string format,
            object[] args = null,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
            [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null)
        {
            this.Add(LogSeverity.Info, format, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Warn(
            string format,
            object[] args = null,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
            [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null)
        {
            this.Add(LogSeverity.Warning, format, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Error(
            string format,
            object[] args = null,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
            [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null)
        {
            this.Add(LogSeverity.Error, format, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Critical(
            string format,
            object[] args = null,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
            [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null)
        {
            this.Add(LogSeverity.Critical, format, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        public virtual void Add(
            LogSeverity severity,
            string format,
            object[] args = null,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
            [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null)
        {
            string text = args != null && args.Length > 0 ? string.Format(format, args) : format;
            this.entries.Add(new StoryLogEntry(severity, text, callerMemberName, callerFilePath, callerLineNumber));
        }

        IEnumerator<IStoryLogEntry> IEnumerable<IStoryLogEntry>.GetEnumerator()
        {
            return this.entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.entries.GetEnumerator();
        }
    }
}
