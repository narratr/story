namespace Story.Core
{
    using System;

    [Serializable]
    public class StoryLogEntry : IStoryLogEntry
    {
        private TimeSpan elapsed;

        public StoryLogEntry(LogSeverity severity, string text, TimeSpan elapsed)
        {
            this.Severity = severity;
            this.Text = text;
            this.elapsed = elapsed;
            DateTime = DateTime.UtcNow;
        }

        public LogSeverity Severity { get; private set; }

        public string Text { get; private set; }

        public TimeSpan Offset { get; private set; }

        public DateTime DateTime { get; private set; }
    }
}
