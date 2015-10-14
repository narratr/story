using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Story.Core.Handlers
{
    public abstract class BufferedHandler : IStoryHandler
    {
        private readonly Subject<IStory> _storiesSubject;

        public BufferedHandler(TimeSpan timeDelay, int batchSize)
        {
            _storiesSubject = new Subject<IStory>();
            _storiesSubject.Buffer(timeDelay, batchSize)
            .Where(buffer => buffer.Any())
            .Subscribe(OnStoriesComplete, OnComplete);
        }

        protected abstract void OnStoriesComplete(IList<IStory> stories);

        private void OnComplete()
        {
            throw new InvalidOperationException("subject should never complete");
        }

        public virtual void OnStart(IStory story)
        {
        }

        public void OnStop(IStory story, Task task)
        {
            // TODO: what to do with the task here
            // consider removing task from OnStop and adding the important task information to the story itself
            _storiesSubject.OnNext(story);
        }
    }
}
