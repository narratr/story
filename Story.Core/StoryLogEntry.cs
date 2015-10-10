namespace Story.Core
{
    using System;

    [Serializable]
    public class StoryLogEntry : IStoryLogEntry
    {
        public LogSeverity Severity { get; set; }

        public string Text { get; set; }

        public TimeSpan Offset { get; set; }

        public DateTime DateTime { get; set; }
    }
}
