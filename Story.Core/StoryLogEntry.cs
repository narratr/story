namespace Story.Core
{
    using System;

    [Serializable]
    public class StoryLogEntry : IStoryLogEntry
    {
        private readonly TimeSpan elapsed;
        private readonly LogSeverity severity;
        private readonly string text;
        private readonly DateTime dateTime;

        public StoryLogEntry(LogSeverity severity, string text, TimeSpan elapsed)
        {
            this.severity = severity;
            this.text = text;
            this.elapsed = elapsed;
            this.dateTime = DateTime.UtcNow;
        }

        public LogSeverity Severity
        {
            get { return this.severity; }
        }

        public string Text
        {
            get { return this.text; }
        }

        public TimeSpan Elapsed
        {
            get { return this.elapsed; }
        }

        public DateTime DateTime
        {
            get { return this.dateTime; }
        }
    }
}
