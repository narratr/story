namespace Story.Core
{
    using System;
    using System.Linq;

    public interface IStoryHandler
    {
        string Name { get; }

        void OnStart(IStory story);

        void OnStop(IStory story);
    }
}
