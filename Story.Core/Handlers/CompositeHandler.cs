using System.Collections.Generic;

namespace Story.Core.Handlers
{
    public class CompositeHandler : StoryHandlerBase
    {
        private readonly IList<IStoryHandler> storyHandlers;

        public CompositeHandler(string name, params IStoryHandler[] storyHandlers)
            : base(name)
        {
            this.storyHandlers = new List<IStoryHandler>(storyHandlers);
        }

        public override void OnStart(IStory story)
        {
            foreach (var storyHandler in this.storyHandlers)
            {
                storyHandler.OnStart(story);
            }
        }

        public override void OnStop(IStory story)
        {
            foreach (var storyHandler in this.storyHandlers)
            {
                storyHandler.OnStop(story);
            }
        }

        public void AddHandler(IStoryHandler storyHandler)
        {
            this.storyHandlers.Add(storyHandler);
        }
    }
}
