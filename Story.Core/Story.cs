using Newtonsoft.Json;

namespace Story.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Utils;

    [Serializable]
    public class Story : ContextBoundObject<IStory>, IStory
    {
        private readonly bool notInContext;
        private readonly Stopwatch stopWatch;
        private readonly IStoryLog log;
        private readonly IStoryData data;

        public Story(string name, IRuleset<IStory, IStoryHandler> handlerProvider, bool notInContext = false)
        {
            Ensure.ArgumentNotEmpty(name, "name");
            Ensure.ArgumentNotNull(handlerProvider, "handlerProvider");

            this.HandlerProvider = handlerProvider;
            this.notInContext = notInContext;
            this.stopWatch = new Stopwatch();
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

        public string Name
        {
            get;
            private set;
        }

        [JsonIgnore]
        public IRuleset<IStory, IStoryHandler> HandlerProvider
        {
            get;
            private set;
        }

        [JsonIgnore]
        public new IStory Parent
        {
            get { return (IStory)base.Parent; }
        }

        public new IEnumerable<IStory> Children
        {
            get { return base.Children.Cast<IStory>(); }
        }

        public IStoryData Data
        {
            get { return this.data; }
        }

        public IStoryLog Log
        {
            get { return this.log; }
        }

        public TimeSpan Elapsed
        {
            get { return this.stopWatch.Elapsed; }
        }

        public DateTime StartDateTime
        {
            get;
            private set;
        }

        [JsonIgnore]
        public Task Task
        {
            get;
            set;
        }

        public void Start()
        {
            if (!this.notInContext)
            {
                this.Attach();
            }

            this.StartDateTime = DateTime.UtcNow;
            this.stopWatch.Start();

            foreach (var handler in this.HandlerProvider.Fire(this))
            {
                handler.OnStart(this);
            }
        }

        public void Stop()
        {
            this.stopWatch.Stop();

            try
            {
                foreach (var handler in this.HandlerProvider.Fire(this))
                {
                    handler.OnStop(this);
                }
            }
            finally
            {
                if (!this.notInContext)
                {
                    base.Detach();
                }
            }
        }
    }
}
