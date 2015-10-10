namespace Story.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    [Serializable]
    public class StoryLog : IStoryLog
    {
        private readonly List<IStoryLogEntry> entries = new List<IStoryLogEntry>();
        private readonly IStory story;

        public StoryLog(IStory story)
        {
            if (story == null)
            {
                throw new ArgumentNullException("story");
            }

            this.story = story;
        }

        public void Debug(string format, params object[] args)
        {
            this.Log(LogSeverity.Debug, format, args);
        }

        public void Info(string format, params object[] args)
        {
            this.Log(LogSeverity.Info, format, args);
        }

        public void Warn(string format, params object[] args)
        {
            this.Log(LogSeverity.Warning, format, args);
        }

        public void Error(string format, params object[] args)
        {
            this.Log(LogSeverity.Error, format, args);
        }

        public void Critical(string format, params object[] args)
        {
            this.Log(LogSeverity.Critical, format, args);
        }

        IEnumerator<IStoryLogEntry> IEnumerable<IStoryLogEntry>.GetEnumerator()
        {
            return this.entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.entries.GetEnumerator();
        }

        private void Log(LogSeverity severity, string format, params object[] args)
        {
            string text = args.Length > 0 ? string.Format(format, args) : format;
            this.entries.Add(new StoryLogEntry(severity, text, this.story.Elapsed));
        }
    }
}
