namespace Story.Core
{
    /// <summary>
    /// Configuration class that defines limits for story rotation to prevent unbounded memory growth.
    /// When a root story exceeds these limits, it will automatically rotate to a new continuation story.
    /// </summary>
    public class StoryLimits
    {
        /// <summary>
        /// Gets or sets the maximum number of descendant stories (children, grandchildren, etc.)
        /// allowed before automatic rotation occurs. Default is 200.
        /// </summary>
        public int MaxDescendantCount { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of log entries (across the entire story tree)
        /// allowed before automatic rotation occurs. Default is 1000.
        /// </summary>
        public int MaxLogEntriesCount { get; set; }

        /// <summary>
        /// Gets or sets whether automatic story rotation is enabled. Default is true.
        /// When disabled, stories will grow unbounded which may lead to memory issues.
        /// </summary>
        public bool EnableAutoRotation { get; set; }

        /// <summary>
        /// Creates a new instance with default limits.
        /// </summary>
        public StoryLimits()
        {
            MaxDescendantCount = 200;
            MaxLogEntriesCount = 1000;
            EnableAutoRotation = true;
        }
    }
}
