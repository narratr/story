namespace Story.Core.Handlers
{
    using System;
    using System.Diagnostics;
    using Utils;

    [Serializable]
    public class TraceHandler : StoryHandlerBase
    {
        private readonly IStoryFormatter storyFormatter;

        public TraceHandler(string name, IStoryFormatter storyFormatter = null)
            : base(name)
        {
            this.storyFormatter = storyFormatter ?? StoryFormatters.GetDelimiterStoryFormatter();
        }

        public override void OnStop(IStory story)
        {
            Ensure.ArgumentNotNull(story, "story");

            string str = this.storyFormatter.FormatStory(story, this.Name);
            Trace.TraceInformation(str);
        }
    }
}
