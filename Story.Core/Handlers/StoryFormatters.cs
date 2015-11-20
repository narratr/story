namespace Story.Core.Handlers
{
    public static class StoryFormatters
    {
        public static BasicStoryFormatter GetBasicStoryFormatter(LogSeverity severityThreshold = LogSeverity.Info)
        {
            return new BasicStoryFormatter(severityThreshold);
        }

        public static DelimiterStoryFormatter GetDelimiterStoryFormatter(LogSeverity severityThreshold = LogSeverity.Info)
        {
            return new DelimiterStoryFormatter(severityThreshold);
        }
    }
}
