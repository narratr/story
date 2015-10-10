namespace Story.Core
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    [Serializable]
    public class Story : ContextBoundObject<Story>, IStory
    {
        private readonly Stopwatch stopWatch;
        private readonly IRuleset<IStory, IStoryHandler> handlerProvider;

        public Story(string name, IRuleset<IStory, IStoryHandler> handlerProvider) : base(Guid.NewGuid().ToString())
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name");
            }

            if (handlerProvider == null)
            {
                throw new ArgumentNullException("handlerFactory");
            }

            this.handlerProvider = handlerProvider;
            this.stopWatch = Stopwatch.StartNew();

            this.Name = name;
            this.Log = new StoryLog(this);
            this.Data = new StoryData(this);
        }

        public string Name { get; private set; }

        public string InstanceId
        {
            get { return base.InstanceId; }
        }

        public IStory Parent
        {
            get { return (IStory)base.Parent; }
        }

        public TimeSpan Elapsed { get { return this.stopWatch.Elapsed; } }

        public long ElapsedTicks { get { return this.stopWatch.ElapsedTicks; } }

        public long ElapsedMilliseconds { get { return this.stopWatch.ElapsedMilliseconds; } }

        public IStoryData Data { get; private set; }

        public IStoryLog Log { get; private set; }

        public void Start()
        {
            try
            {
                foreach (var handler in this.handlerProvider.Fire(this))
                {
                    handler.OnStart(this);
                }
            }
            catch
            {
                throw;
            }
        }

        public void Stop(Task task)
        {
            try
            {
                foreach (var handler in this.handlerProvider.Fire(this))
                {
                    handler.OnStop(this, task);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
