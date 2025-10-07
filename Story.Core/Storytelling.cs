using Story.Core.Handlers;
using Story.Core.Rules;
using Story.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                return new DummyStory();

                //return new OneTimeStory(Factory.GetRuleset());

                //if (!this.ignoreEmptyStory)
                //{
                //    throw new InvalidOperationException("No story in context");
                //}

                //return new ContextStory.DummyStory();
            }
        }

        public static IStoryFactory Factory { get; set; }

        /// <summary>
        /// Checks if there is a current story available in the context.
        /// </summary>
        public static bool HasCurrentStory
        {
            get
            {
                return Story.FromContext() as IStory != null;
            }
        }

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
            GetStoryForLogging().Log.Debug(format, args);
        }

        /// <summary>
        /// Write to the log of the story with information log severity
        /// </summary>
        public static void Info(string format, params object[] args)
        {
            GetStoryForLogging().Log.Info(format, args);
        }

        /// <summary>
        /// Write to the log of the story with warning log severity
        /// </summary>
        public static void Warn(string format, params object[] args)
        {
            GetStoryForLogging().Log.Warn(format, args);
        }

        /// <summary>
        /// Write to the log of the story with error log severity
        /// </summary>
        public static void Error(string format, params object[] args)
        {
            GetStoryForLogging().Log.Error(format, args);
        }

        /// <summary>
        /// Write to the log of the story with critical log severity
        /// </summary>
        public static void Critical(string format, params object[] args)
        {
            GetStoryForLogging().Log.Critical(format, args);
        }

        public static IStoryData Data
        {
            get { return GetStoryForData().Data; }
        }

        /// <summary>
        /// Gets a story for logging operations. Returns the current story if it exists,
        /// otherwise creates a OneTimeStory that will auto-stop after the first log entry.
        /// </summary>
        private static IStory GetStoryForLogging()
        {
            var story = Story.FromContext() as IStory;
            if (story != null)
            {
                return story;
            }

            // Create a OneTimeStory for logging when no story exists
            return new OneTimeStory(Factory.GetRuleset());
        }

        /// <summary>
        /// Gets a story for data operations. Returns the current story if it exists,
        /// otherwise creates a OneTimeStory that will auto-stop after the first data entry.
        /// </summary>
        private static IStory GetStoryForData()
        {
            var story = Story.FromContext() as IStory;
            if (story != null)
            {
                return story;
            }

            // Create a OneTimeStory for data when no story exists
            return new OneTimeStory(Factory.GetRuleset());
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

        private class OneTimeStory : IStory
        {
            private readonly IRuleset<IStory, IStoryHandler> handlerProvider;
            private readonly Stopwatch stopWatch;
            private readonly IStoryLog log;
            private readonly IStoryData data;
            private bool hasLogged;
            private bool isDisposed;

            public OneTimeStory(IRuleset<IStory, IStoryHandler> handlerProvider)
            {
                Ensure.ArgumentNotNull(handlerProvider, "handlerProvider");
                
                this.handlerProvider = handlerProvider;
                this.stopWatch = new Stopwatch();
                this.log = new OneTimeStoryLog(this);
                this.data = new OneTimeStoryData(this);
                this.Name = "OneTimeStory";
                this.InstanceId = Guid.NewGuid().ToString();
                this.StartDateTime = DateTime.UtcNow;
                this.Status = StoryStatus.Running;
                this.ThreadName = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
                
                // Start the story immediately
                this.stopWatch.Start();
                this.Start();
            }

            public string Name { get; private set; }
            public string InstanceId { get; private set; }
            public Task Task { get; set; }
            public DateTime StartDateTime { get; private set; }
            public TimeSpan Elapsed => this.stopWatch.Elapsed;
            public StoryStatus Status { get; private set; }
            public string ThreadName { get; private set; }
            public string FilePath { get; set; }
            public string MethodName { get; set; }
            public int? LineNumber { get; set; }

            public IRuleset<IStory, IStoryHandler> HandlerProvider => this.handlerProvider;
            public IStory Parent => null;
            public IEnumerable<IStory> Children => Enumerable.Empty<IStory>();
            public IStoryData Data => this.data;
            public IStoryLog Log => this.log;

            public void Start()
            {
                if (this.isDisposed) return;

                foreach (var handler in this.handlerProvider.Fire(this))
                {
                    handler.OnStart(this);
                }
            }

            public void Stop()
            {
                if (this.isDisposed) return;

                this.stopWatch.Stop();
                this.Status = StoryStatus.Completed;

                foreach (var handler in this.handlerProvider.Fire(this))
                {
                    handler.OnStop(this);
                }

                this.isDisposed = true;
            }

            public void ReportFailed()
            {
            }

            public void UpdateStatus(StoryStatus status)
            {
            }

            public void Dispose()
            {
                if (this.isDisposed) return;

                if (!this.hasLogged)
                {
                    this.hasLogged = true;
                    this.Stop();
                }
            }

            private class OneTimeStoryData : IStoryData
            {
                private readonly OneTimeStory story;

                public OneTimeStoryData(OneTimeStory story)
                {
                    this.story = story;
                }

                public object this[string key]
                {
                    get
                    {
                        return null;
                    }
                    set
                    {
                        // Log the data addition and auto-stop after first data entry
                        this.story.Log.Info("Added key '{0}' with value '{1}' to data.", key, value);
                    }
                }
                
                IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
                {
                    return (IEnumerator<KeyValuePair<string, object>>)Array.Empty<KeyValuePair<string, object>>().GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return Array.Empty<KeyValuePair<string, object>>().GetEnumerator();
                }
            }

            private class OneTimeStoryLog : StoryLog
            {
                private readonly OneTimeStory story;

                public OneTimeStoryLog(OneTimeStory story) : base(story)
                {
                    this.story = story;
                }

                public override void Add(LogSeverity severity, string format,
                    [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
                    [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
                    [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null,
                    params object[] args)
                {
                    // Auto-stop after first log entry
                    if (!this.story.hasLogged)
                    {
                        base.Add(severity, format, callerMemberName, callerFilePath, callerLineNumber, args);
                        this.story.hasLogged = true;
                        this.story.Stop();
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

            public StoryStatus Status { get; set; }

            public string ThreadName { get; set; }

            public string FilePath { get; set; }

            public string MethodName { get; set; }

            public int? LineNumber { get; set; }

            public void Start()
            {
            }

            public void Stop()
            {
            }

            public void Dispose()
            {
            }

            public void ReportFailed()
            {
            }

            public void UpdateStatus(StoryStatus status)
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

                public void Add(LogSeverity severity, string format,
                    [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
                    [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
                    [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null,
                    params object[] args)
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
