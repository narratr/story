namespace Story.Core.Handlers
{
    public abstract class StoryHandlerBase : IStoryHandler
    {
        public StoryHandlerBase(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        public virtual void OnStart(IStory story)
        {
        }

        public virtual void OnStop(IStory story)
        {
        }
    }
}