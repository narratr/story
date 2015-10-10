namespace Story.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IStoryHandler
    {
        void OnStart(IStory story);

        void OnStop(IStory story, Task task);
    }
}
