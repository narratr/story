namespace Story.Core.Handlers
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
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

        public virtual void OnStop(IStory story, Task task)
        {
            Ensure.ArgumentNotNull(story, "story");
            Ensure.ArgumentNotNull(task, "task");

            string str = this.storyFormatter.FormatStory(story);
            Trace.TraceInformation(str);
        }
    }
}
