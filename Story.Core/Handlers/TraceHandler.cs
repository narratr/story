namespace Story.Core.Handlers
{
    using System;
    using System.Diagnostics;
    using Utils;

    [Serializable]
    public class TraceHandler : IStoryHandler
    {
        private readonly IStoryFormatter storyFormatter;

        public TraceHandler(IStoryFormatter storyFormatter = null)
        {
            this.storyFormatter = storyFormatter ?? new DelimiterStoryFormatter(LogSeverity.Debug);
        }

        public void OnStart(IStory story)
        {
        }

        public virtual void OnStop(IStory story)
        {
            Ensure.ArgumentNotNull(story, "story");

            string str = this.storyFormatter.FormatStory(story);
            Trace.TraceInformation(str);
        }
    }
}
