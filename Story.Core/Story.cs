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

        // Direct reference to root story for O(1) access
        private Story _rootStory;

        // Shared state for root story tracking (rotation counters and metadata)
        // All stories in a hierarchy share the same RootStoryState instance
        internal RootStoryState _rootState;

        public Story(string name, IRuleset<IStory, IStoryHandler> handlerProvider, bool notInContext = false,
            string callerMemberName = null, string callerFilePath = null, int? callerLineNumber = null)
        {
            Ensure.ArgumentNotEmpty(name, "name");
            Ensure.ArgumentNotNull(handlerProvider, "handlerProvider");

            this.HandlerProvider = handlerProvider;
            this.notInContext = notInContext;
            this.stopWatch = new Stopwatch();
            this.log = new StoryLog(this);
            this.data = new StoryData(this);

            this.Name = name;
            this.MethodName = callerMemberName;
            this.LineNumber = callerLineNumber;

            // Store the full file path
            this.FilePath = callerFilePath;

            // Initially, this story is its own root (will be updated if attached to parent)
            this._rootStory = this;

            // Allocate root state for this story (will be shared with children when they attach)
            this._rootState = new RootStoryState();
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

        public StoryStatus Status
        {
            get;
            private set;
        }

        public string ThreadName
        {
            get;
            private set;
        }

        public string FilePath
        {
            get;
            private set;
        }

        public string MethodName
        {
            get;
            private set;
        }

        public int? LineNumber
        {
            get;
            private set;
        }

        [JsonIgnore]
        public StoryRotationInfo RotationInfo
        {
            get { return this._rootState?.RotationInfo; }
        }

        /// <summary>
        /// Gets the current descendant count for this story's hierarchy.
        /// </summary>
        public int DescendantCount
        {
            get { return this._rootState?.DescendantCount ?? 0; }
        }

        /// <summary>
        /// Gets the current total log count for this story's hierarchy.
        /// </summary>
        public int TotalLogCount
        {
            get { return this._rootState?.TotalLogCount ?? 0; }
        }

        /// <summary>
        /// Increments the descendant count atomically on the root story state.
        /// </summary>
        internal void IncrementDescendantCount()
        {
            this._rootState?.IncrementDescendantCount();
        }

        /// <summary>
        /// Increments the total log count atomically on the root story state.
        /// </summary>
        internal void IncrementLogCount()
        {
            this._rootState?.IncrementLogCount();
        }

        /// <summary>
        /// Gets the root story in the hierarchy.
        /// </summary>
        internal Story GetRootStory()
        {
            return this._rootStory;
        }

        /// <summary>
        /// Sets the root story reference and shares the root state. Called when attaching to a parent.
        /// </summary>
        internal void SetRootStory(Story rootStory)
        {
            this._rootStory = rootStory;

            // Share the root story's state (for counters and rotation info)
            this._rootState = rootStory._rootState;
        }

        public void Start()
        {
            if (!this.notInContext)
            {
                this.Attach();
            }

            // Capture thread name
            var currentThread = System.Threading.Thread.CurrentThread;
            this.ThreadName = !string.IsNullOrEmpty(currentThread.Name)
                ? currentThread.Name
                : currentThread.ManagedThreadId.ToString();

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

            if (this.Status == StoryStatus.Running)
            {
                this.Status = StoryStatus.Completed;
            }

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

        public void ReportFailed()
        {
            this.UpdateStatus(StoryStatus.Failed);
        }

        public void UpdateStatus(StoryStatus status)
        {
            this.Status = status;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
