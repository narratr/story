namespace Story.Core.Handlers
{
    using System;
    using System.Linq;

    using Utils;

    [Serializable]
    public class ActionHandler : StoryHandlerBase
    {
        private readonly Action<IStory> startAction;
        private readonly Action<IStory> stopAction;

        public ActionHandler(string name, Action<IStory> stopAction)
            : this(name, null, stopAction)
        {
        }

        public ActionHandler(string name, Action<IStory> startAction, Action<IStory> stopAction)
            : base(name)
        {
            this.startAction = startAction;
            this.stopAction = stopAction;
        }

        public override void OnStart(IStory story)
        {
            Ensure.ArgumentNotNull(story, "story");

            if (this.startAction != null)
            {
                this.startAction(story);
            }
        }

        public override void OnStop(IStory story)
        {
            Ensure.ArgumentNotNull(story, "story");

            if (this.stopAction != null)
            {
                this.stopAction(story);
            }
        }
    }
}
