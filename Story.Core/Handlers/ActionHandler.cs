namespace Story.Core.Handlers
{
    using System;
    using System.Linq;

    using Utils;

    [Serializable]
    public class ActionHandler : IStoryHandler
    {
        private readonly Action<IStory> startAction;
        private readonly Action<IStory> stopAction;

        public ActionHandler(Action<IStory> stopAction)
            : this(null, stopAction)
        {
        }

        public ActionHandler(Action<IStory> startAction, Action<IStory> stopAction)
        {
            this.startAction = startAction;
            this.stopAction = stopAction;
        }

        public void OnStart(IStory story)
        {
            Ensure.ArgumentNotNull(story, "story");

            if (this.startAction != null)
            {
                this.startAction(story);
            }
        }

        public void OnStop(IStory story)
        {
            Ensure.ArgumentNotNull(story, "story");

            if (this.stopAction != null)
            {
                this.stopAction(story);
            }
        }
    }
}
