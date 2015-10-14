namespace Story.Core.Handlers
{
    using System;

    using Utils;

    [Serializable]
    public class ConsoleHandler : IStoryHandler
    {
        public static readonly IStoryHandler DefaultConsoleHandler = new ConsoleHandler();
        public static readonly IStoryHandler BasicConsoleHandler = new ConsoleHandler(new BasicStoryFormatter());

        private readonly IStoryFormatter storyFormatter;

        public ConsoleHandler(IStoryFormatter storyFormatter = null)
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
            Console.WriteLine(str);
        }
    }
}
