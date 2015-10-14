namespace Story.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    // TODO: name
    public class ContextStory : IStory
    {
        private readonly bool ignoreEmptyStory;

        private ContextStory(bool ignoreEmptyStory)
        {
            this.ignoreEmptyStory = ignoreEmptyStory;
        }

        // TODO: name
        public static IStory GetUpdatedCurrentStory(bool ignoreEmptyStory = false)
        {
            return new ContextStory(ignoreEmptyStory);
        }

        private IStory CurrentStory
        {
            get
            {
                var story = Story.FromContext() as IStory;
                if (story != null)
                {
                    // TODO: handle thread safety for story
                    return story;
                }

                // When there is no story in the context there are 3 options:
                // 1. Return a story that does nothing and trace a warning
                // 2. Return a story that after a single usage calls the story handlers
                // 3. Fail

                // var stackTrace = new StackTrace(1);
                // Trace.TraceWarning("No story in context, caller call stack is {0}", stackTrace);
                // return new DummyStory();

                // return new OneTimeStory(DefaultStoryRulesetProvider.GetRuleset());

                if (!this.ignoreEmptyStory)
                {
                    throw new InvalidOperationException("No story in context");
                }

                return new DummyStory();
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
            throw new InvalidOperationException("Context story should not invoke Start");
        }

        void IStory.Stop(Task task)
        {
            throw new InvalidOperationException("Context story should not invoke Stop");
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
