namespace Story.Core.Handlers
{
    using System;

    using Utils;

    [Serializable]
    public class ConsoleHandler : StoryHandlerBase
    {
        private readonly IStoryFormatter storyFormatter;

        public ConsoleHandler(string name, IStoryFormatter storyFormatter = null)
            : base(name)
        {
            this.storyFormatter = storyFormatter ?? StoryFormatters.GetDelimiterStoryFormatter();
        }

        public override void OnStop(IStory story)
        {
            Ensure.ArgumentNotNull(story, "story");

            string str = this.storyFormatter.FormatStory(story, this.Name);
            Console.WriteLine(str);
        }
    }
}
