namespace Story.Core
{
    using System.Threading;

    /// <summary>
    /// Holds state that is specific to root stories and shared across the entire story hierarchy.
    /// This includes rotation tracking (descendant count, log count) and rotation metadata.
    /// </summary>
    internal class RootStoryState
    {
        // Thread-safe counters for rotation tracking
        private int _descendantCount = 0;
        private int _totalLogCount = 0;

        /// <summary>
        /// Gets the current descendant count.
        /// </summary>
        public int DescendantCount
        {
            get { return Interlocked.CompareExchange(ref _descendantCount, 0, 0); }
        }

        /// <summary>
        /// Gets the current total log count.
        /// </summary>
        public int TotalLogCount
        {
            get { return Interlocked.CompareExchange(ref _totalLogCount, 0, 0); }
        }

        /// <summary>
        /// Gets or sets the rotation information. Null for original stories, populated for continuation stories.
        /// </summary>
        public StoryRotationInfo RotationInfo { get; set; }

        /// <summary>
        /// Increments the descendant count atomically.
        /// </summary>
        public void IncrementDescendantCount()
        {
            Interlocked.Increment(ref _descendantCount);
        }

        /// <summary>
        /// Increments the total log count atomically.
        /// </summary>
        public void IncrementLogCount()
        {
            Interlocked.Increment(ref _totalLogCount);
        }
    }
}
