using System.Threading;
using System.Threading.Tasks;

namespace Story.Core
{
    public class StoryWorker
    {
        private readonly IStoryTransport storyTransport;
        private readonly IRuleset<IStory, IStoryHandler> handlerProvider;

        public StoryWorker(IStoryTransport storyTransport, IRuleset<IStory, IStoryHandler> handlerProvider)
        {
            this.storyTransport = storyTransport;
            this.handlerProvider = handlerProvider;
        }

        public Task RunAsync()
        {
            return RunAsync(CancellationToken.None);
        }

        public async Task RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var story = await this.storyTransport.GetNextAsync(token);
                if (story != null)
                {
                    Process(story);
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }

        private void Process(IStory story)
        {
            foreach (var handler in this.handlerProvider.Fire(story))
            {
                handler.OnStop(story);
            }
        }
    }
}
