namespace Story.Core.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ActionHandler : IStoryHandler
    {
        private readonly Action<IStory> startAction;
        private readonly Action<IStory, Task> stopAction;

        public ActionHandler(Action<IStory> startAction, Action<IStory, Task> stopAction)
        {
            this.startAction = startAction;
            this.stopAction = stopAction;
        }

        public void OnStart(IStory story)
        {
            this.startAction(story);
        }

        public void OnStop(IStory story, Task task)
        {
            this.stopAction(story, task);
        }
    }
}
