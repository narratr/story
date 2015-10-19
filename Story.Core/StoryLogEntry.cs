namespace Story.Core
{
    using System;

    [Serializable]
    public class StoryLogEntry : IStoryLogEntry
    {
        private readonly DateTime dateTime;
        private readonly LogSeverity severity;
        private readonly string text;

        public StoryLogEntry(LogSeverity severity, string text)
        {
            this.dateTime = DateTime.UtcNow;
            this.severity = severity;
            this.text = text;
        }

        public LogSeverity Severity
        {
            get { return this.severity; }
        }

        public string Text
        {
            get { return this.text; }
        }

        public DateTime DateTime
        {
            get { return this.dateTime; }
        }
    }
}
