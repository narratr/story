namespace Story.Core
{
    using System;

    [Serializable]
    public class StoryLogEntry : IStoryLogEntry
    {
        private readonly TimeSpan elapsed;
        private readonly LogSeverity severity;
        private readonly string text;

        public StoryLogEntry(LogSeverity severity, string text, TimeSpan elapsed)
        {
            this.severity = severity;
            this.text = text;
            this.elapsed = elapsed;
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
    }
}
