using Story.Core;
using Story.Core.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Story.Ext.Handlers
{
    public abstract class BufferedHandler : StoryHandlerBase
    {
        private readonly Subject<IStory> _storiesSubject;

        public BufferedHandler(string name, TimeSpan timeDelay, int batchSize)
            : base(name)
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

        public override void OnStop(IStory story)
        {
            _storiesSubject.OnNext(story);
        }
    }
}
