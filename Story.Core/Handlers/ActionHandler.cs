namespace Story.Core.Handlers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class ActionHandler : IStoryHandler
    {
        private readonly Action<IStory> startAction;
        private readonly Action<IStory, Task> stopAction;

        public ActionHandler(Action<IStory, Task> stopAction)
            : this(null, stopAction)
        {
        }

        public ActionHandler(Action<IStory> startAction, Action<IStory, Task> stopAction)
        {
            this.startAction = startAction;
            this.stopAction = stopAction;
        }

        public void OnStart(IStory story)
        {
            if (this.startAction != null)
            {
                this.startAction(story);
            }
        }

        public void OnStop(IStory story, Task task)
        {
            if (this.stopAction != null)
            {
                this.stopAction(story, task);
            }
        }
    }
}
