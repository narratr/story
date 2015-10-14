namespace Story.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ContextStoryProvider : BasicStoryProvider, IStory
    {
        public static IStoryRulesetProvider DefaultStoryRulesetProvider { get; set; }

        // TODO: name
        public static IStory GetUpdatedCurrentStory(IStoryRulesetProvider storyRulesetProvider = null)
        {
            return new ContextStoryProvider(storyRulesetProvider ?? DefaultStoryRulesetProvider);
        }

        public ContextStoryProvider(IStoryRulesetProvider storyRulesetProvider) : base(storyRulesetProvider)
        {
        }

        public bool UseParentRulesetProvider { get; set; }

        public override IStory CreateStory(string name)
        {
            if (UseParentRulesetProvider)
            {
                var story = Story.FromContext() as IStory;
                if (story != null)
                {
                    return new Story(name, story.HandlerProvider);
                }
            }

            return base.CreateStory(name);
        }

        public IStory CurrentStory
        {
            get
            {
                var story = Story.FromContext() as IStory;
                if (story != null)
                {
                    // TODO: handle thread safety for story
                    return story;
                }

                // When there is no story in the context there are 2 options:
                // 1. Return a story that does nothing and trace a warning
                // 2. Return a story that after a single usage calls the story handlers

                // var stackTrace = new StackTrace(1);
                // Trace.TraceWarning("No story in context, caller call stack is {0}", stackTrace);
                // return new DummyStory();

                return new OneTimeStory(DefaultStoryRulesetProvider.GetRuleset());
            }
        }

        string IStory.Name
        {
            get
            {
                return CurrentStory.Name;
            }
        }

        string IStory.InstanceId
        {
            get
            {
                return CurrentStory.InstanceId;
            }
        }

        TimeSpan IStory.Elapsed
        {
            get
            {
                return CurrentStory.Elapsed;
            }
        }

        IStoryData IStory.Data
        {
            get
            {
                return CurrentStory.Data;
            }
        }

        IStoryLog IStory.Log
        {
            get
            {
                return CurrentStory.Log;
            }
        }

        IStory IStory.Parent
        {
            get
            {
                return CurrentStory.Parent;
            }
        }

        IRuleset<IStory, IStoryHandler> IStory.HandlerProvider
        {
            get
            {
                return CurrentStory.HandlerProvider;
            }
        }

        DateTime IStory.StartDateTime
        {
            get
            {
                return DateTime.MinValue;
            }
        }

        void IStory.Start()
        {
        }

        void IStory.Stop(Task task)
        {
        }

        /// <summary>
        /// One time story will invoke handlers immediately after a single use of either the log or data
        /// </summary>
        private class OneTimeStory : IStory
        {
            private readonly IStoryLog log;
            private readonly IStoryData data;

            public OneTimeStory(IRuleset<IStory, IStoryHandler> handlerProvider)
            {
                this.log = new OneTimeStoryLog(this);
                this.data = new OneTimeStoryData(this);
                this.HandlerProvider = handlerProvider;
                this.StartDateTime = DateTime.UtcNow;
            }

            public IRuleset<IStory, IStoryHandler> HandlerProvider { get; private set; }

            public TimeSpan Elapsed
            {
                get
                {
                    return TimeSpan.Zero;
                }
            }

            public string InstanceId
            {
                get
                {
                    return String.Empty;
                }
            }

            public IStoryData Data
            {
                get { return this.data; }
            }

            public IStoryLog Log
            {
                get { return this.log; }
            }

            public string Name
            {
                get
                {
                    return null;
                }
            }

            public IStory Parent
            {
                get
                {
                    return null;
                }
            }

            public DateTime StartDateTime { get; private set; }

            private void InvokeHandlers()
            {
                foreach (var handler in this.HandlerProvider.Fire(this))
                {
                    handler.OnStop(this, Task.FromResult(true));
                }
            }

            public void Start()
            {
            }

            public void Stop(Task task)
            {
            }

            private class OneTimeStoryLog : StoryLog
            {
                public OneTimeStoryLog(OneTimeStory story) : base(story)
                {
                }

                public override void Add(LogSeverity severity, string format, params object[] args)
                {
                    base.Add(severity, format, args);
                    ((OneTimeStory)Story).InvokeHandlers();
                }
            }

            private class OneTimeStoryData : StoryData
            {
                public OneTimeStoryData(OneTimeStory story) : base(story)
                {
                }

                public override object this[string key]
                {
                    set
                    {
                        base[key] = value;
                        ((OneTimeStory)Story).InvokeHandlers();
                    }
                }
            }
        }

        private class DummyStory : IStory
        {
            private static readonly IStoryData StoryData = new EmptyStoryData();
            private static readonly IStoryLog StoryLog = new EmptyStoryLog();

            public IStoryData Data
            {
                get
                {
                    return StoryData;
                }
            }

            public TimeSpan Elapsed
            {
                get
                {
                    return TimeSpan.Zero;
                }
            }

            public string InstanceId
            {
                get
                {
                    return String.Empty;
                }
            }

            public IStoryLog Log
            {
                get
                {
                    return StoryLog;
                }
            }

            public string Name
            {
                get
                {
                    return String.Empty;
                }
            }

            public IRuleset<IStory, IStoryHandler> HandlerProvider
            {
                get
                {
                    return null;
                }
            }

            public IStory Parent
            {
                get
                {
                    return null;
                }
            }

            public DateTime StartDateTime
            {
                get
                {
                    return DateTime.MinValue;
                }
            }

            public void Start()
            {
            }

            public void Stop(Task task)
            {
            }

            private class EmptyStoryData : IStoryData
            {
                private Dictionary<string, object> dictionary = new Dictionary<string, object>();

                public object this[string key]
                {
                    set
                    {
                    }
                    get
                    {
                        return null;
                    }
                }

                public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
                {
                    return dictionary.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return dictionary.GetEnumerator();
                }
            }

            private class EmptyStoryLog : IStoryLog
            {
                private List<IStoryLogEntry> emptyList = new List<IStoryLogEntry>();

                public void Add(LogSeverity severity, string format, params object[] args)
                {
                }

                public void Critical(string format, params object[] args)
                {
                }

                public void Debug(string format, params object[] args)
                {
                }

                public void Error(string format, params object[] args)
                {
                }

                public IEnumerator<IStoryLogEntry> GetEnumerator()
                {
                    return emptyList.GetEnumerator();
                }

                public void Info(string format, params object[] args)
                {
                }

                public void Warn(string format, params object[] args)
                {
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return emptyList.GetEnumerator();
                }
            }
        }
    }
}
