namespace Story.Core.Handlers
{
    public static class StoryFormatters
    {
        public static readonly BasicStoryFormatter BasicStoryFormatter = new BasicStoryFormatter();
        public static readonly DelimiterStoryFormatter DelimiterStoryFormatter = new DelimiterStoryFormatter(LogSeverity.Debug);
    }
}
