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
        private readonly IStoryLog log;
        private readonly IStoryData data;

        public Story(string name, IRuleset<IStory, IStoryHandler> handlerProvider) : base(Guid.NewGuid().ToString())
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("name");
                }

                if (handlerProvider == null)
                {
                    throw new ArgumentNullException("handlerProvider");
                }

                this.handlerProvider = handlerProvider;
                this.stopWatch = Stopwatch.StartNew();
                this.log = new StoryLog(this);
                this.data = new StoryData(this);

                if (this.Parent == null)
                {
                    this.Name = name;
                }
                else
                {
                    this.Name = this.Parent.Name + "/" + name;
                }
            }
            catch
            {
                base.Detach();
                throw;
            }
        }

        public string Name { get; private set; }

        public new IStory Parent
        {
            get { return (IStory)base.Parent; }
        }

        public IStoryData Data
        {
            get { return this.data; }
        }

        public IStoryLog Log
        {
            get { return this.log; }
        }

        public TimeSpan Elapsed { get { return this.stopWatch.Elapsed; } }

        public long ElapsedTicks { get { return this.stopWatch.ElapsedTicks; } }

        public long ElapsedMilliseconds { get { return this.stopWatch.ElapsedMilliseconds; } }

        public void Start()
        {
            foreach (var handler in this.handlerProvider.Fire(this))
            {
                handler.OnStart(this);
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
            finally
            {
                base.Detach();
            }
        }

        public Task<T> Run<T>(Func<IStory, Task<T>> func)
        {
            this.Start();

            Task<T> result = func(this);
            result.ContinueWith(this.Stop, TaskContinuationOptions.ExecuteSynchronously);

            return result;
        }

        public Task Run(Func<IStory, Task> func)
        {
            this.Start();

            Task result = func(this);
            result.ContinueWith(this.Stop, TaskContinuationOptions.ExecuteSynchronously);

            return result;
        }
    }
}
