using Story.Core;
using Story.Core.Handlers;
using System;
using System.Collections.Generic;

namespace Story.Ext.Handlers
{
    public class StoryTransportHandler : BufferedHandler
    {
        private readonly IStoryTransport storyTransport;

        public StoryTransportHandler(string name, IStoryTransport storyTransport, TimeSpan timeDelay, int batchSize)
            : base(name, timeDelay, batchSize)
        {
            this.storyTransport = storyTransport;
        }

        protected override void OnStoriesComplete(IList<IStory> stories)
        {
            // TODO: async
            this.storyTransport.SendAsync(stories);
        }
    }
}
