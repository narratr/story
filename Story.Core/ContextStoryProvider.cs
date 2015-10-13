namespace Story.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ContextStoryProvider : BasicStoryProvider
    {
        public static BasicStoryRulesetProvider DefaultStoryRulesetProvider { get; set; }

        public static ContextStoryProvider GetStoryProvider(IStoryRulesetProvider storyRulesetProvider = null)
        {
            return new ContextStoryProvider(storyRulesetProvider ?? DefaultStoryRulesetProvider);
        }

        public ContextStoryProvider(IStoryRulesetProvider storyRulesetProvider) : base(storyRulesetProvider) { }

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

        private IStory GetCurrentStory()
        {
            var story = Story.FromContext() as IStory;
            if (story != null)
            {
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

        /// <summary>
        /// One time story will invoke handlers immediately after a single use of either the log or data
        /// </summary>
        private class OneTimeStory : IStory
        {
            private readonly IRuleset<IStory, IStoryHandler> handlerProvider;

            // the reason for wrapping these interfaces instead of inheritance
            // is to make sure interface changes are reflected and handled
            private readonly IStoryLog log;
            private readonly IStoryData data;

            public OneTimeStory(IRuleset<IStory, IStoryHandler> handlerProvider)
            {
                this.log = new OneTimeStoryLog(this);
                this.data = new OneTimeStoryData(this);
                this.handlerProvider = handlerProvider;
            }

            public TimeSpan Elapsed
            {
                get
                {
                    return TimeSpan.Zero;
                }
            }

            public long ElapsedMilliseconds
            {
                get
                {
                    return 0;
                }
            }

            public long ElapsedTicks
            {
                get
                {
                    return 0;
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

            private void InvokeHandlers()
            {
                foreach (var handler in this.handlerProvider.Fire(this))
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
                public OneTimeStoryLog(OneTimeStory story) : base(story) { }

                public override void Add(LogSeverity severity, string format, params object[] args)
                {
                    base.Add(severity, format, args);
                    ((OneTimeStory)Story).InvokeHandlers();
                }
            }

            private class OneTimeStoryData : StoryData
            {
                public OneTimeStoryData(OneTimeStory story) : base(story) { }

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

            public long ElapsedMilliseconds
            {
                get
                {
                    return 0;
                }
            }

            public long ElapsedTicks
            {
                get
                {
                    return 0;
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

            public IStory Parent
            {
                get
                {
                    return null;
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
