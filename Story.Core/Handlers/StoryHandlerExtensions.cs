namespace Story.Core.Handlers
{
    public static class StoryHandlerExtensions
    {
        // Usage: new Handler("myhandler").Compose(new Handler("myhandler2"))
        public static IStoryHandler Compose(this IStoryHandler storyHandler, IStoryHandler storyHandler2)
        {
            if (storyHandler is CompositeHandler)
            {
                ((CompositeHandler)storyHandler).AddHandler(storyHandler2);
                return storyHandler;
            }

            return new CompositeHandler(null, storyHandler, storyHandler2);
        }
    }
}
