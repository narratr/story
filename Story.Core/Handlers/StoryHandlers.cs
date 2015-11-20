namespace Story.Core.Handlers
{
    public static class StoryHandlers
    {
        public static readonly ConsoleHandler DefaultConsoleHandler = new ConsoleHandler("Console");
        public static readonly TraceHandler DefaultTraceHandler = new TraceHandler("Trace");
    }
}
