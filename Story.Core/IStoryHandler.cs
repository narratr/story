namespace Story.Core
{
    using System;
    using System.Linq;

    public interface IStoryHandler
    {
        void OnStart(IStory story);

        void OnStop(IStory story);
    }
}
