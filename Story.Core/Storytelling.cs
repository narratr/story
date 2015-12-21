using Story.Core.Handlers;
using Story.Core.Rules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Story.Core
{
    public static class Storytelling
    {
        static Storytelling()
        {
            var ruleset = new Ruleset<IStory, IStoryHandler>();

            ruleset.Rules.Add(
                new PredicateRule(
                    story => story.IsRoot(),
                    story => StoryHandlers.DefaultTraceHandler));

            Factory = new BasicStoryFactory(ruleset);
        }

        public static IStory Current
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

                //if (!this.ignoreEmptyStory)
                {
                    throw new InvalidOperationException("No story in context");
                }

                //return new ContextStory.DummyStory();
            }
        }

        public static IStoryFactory Factory { get; set; }

        public static IStory IgnoreEmptyStory
        {
            get
            {
                return Story.FromContext() as IStory ?? new DummyStory();
            }
        }

        /// <summary>
        /// Write to the log of the story with debug log severity
        /// </summary>
        public static void Debug(string format, params object[] args)
        {
            Current.Log.Debug(format, args);
        }

        /// <summary>
        /// Write to the log of the story with information log severity
        /// </summary>
        public static void Info(string format, params object[] args)
        {
            Current.Log.Info(format, args);
        }

        /// <summary>
        /// Write to the log of the story with warning log severity
        /// </summary>
        public static void Warn(string format, params object[] args)
        {
            Current.Log.Warn(format, args);
        }

        /// <summary>
        /// Write to the log of the story with error log severity
        /// </summary>
        public static void Error(string format, params object[] args)
        {
            Current.Log.Error(format, args);
        }

        /// <summary>
        /// Write to the log of the story with critical log severity
        /// </summary>
        public static void Critical(string format, params object[] args)
        {
            Current.Log.Critical(format, args);
        }

        public static IStoryData Data
        {
            get { return Current.Data; }
        }

        public static IStory StartNew([CallerMemberName]string name = "")
        {
            return Factory.StartNew(name);
        }

        /// <summary>
        /// Invokes the task to be observed by this story.
        /// </summary>
        public static T StartNew<T>(Func<T> func, [CallerMemberName]string name = "")
        {
            return Factory.StartNew(name, func);
        }

        public static T StartNew<T>(string name, Func<T> func)
        {
            return Factory.StartNew(name, func);
        }

        /// <summary>
        /// Invokes the task to be observed by this story.
        /// </summary>
        public static void StartNew(string name, Action action)
        {
            Factory.StartNew(name, action);
        }

        public static void StartNew(Action action, [CallerMemberName]string name = "")
        {
            Factory.StartNew(name, action);
        }

        /// <summary>
        /// Invokes the task to be observed by this story.
        /// </summary>
        public static Task<T> StartNewAsync<T>(string name, Func<Task<T>> func)
        {
            return Factory.StartNewAsync(name, func);
        }

        public static Task<T> StartNewAsync<T>(Func<Task<T>> func, [CallerMemberName]string name = "")
        {
            return Factory.StartNewAsync(name, func);
        }

        /// <summary>
        /// Invokes the task to be observed by this story.
        /// </summary>
        public static Task StartNewAsync(string name, Func<Task> func)
        {
            return Factory.StartNewAsync(name, func);
        }

        public static Task StartNewAsync(Func<Task> func, [CallerMemberName]string name = "")
        {
            return Factory.StartNewAsync(name, func);
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

            public IEnumerable<IStory> Children
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

            public Task Task { get; set; }

            public void Start()
            {
            }

            public void Stop()
            {
            }

            public void Dispose()
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
