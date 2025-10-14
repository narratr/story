namespace Story.Core
{
    /// <summary>
    /// Configuration class that defines global settings for stories including
    /// rotation limits to prevent unbounded memory growth and log filtering.
    /// When a root story exceeds rotation limits, it will automatically rotate to a new continuation story.
    /// </summary>
    public class StoryConfiguration
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
        /// Gets or sets the minimum log severity level for logs to be created globally.
        /// Logs below this severity will not be created at all, saving memory and processing.
        /// Default is Debug (all logs are created).
        /// This provides the first tier of filtering before handler-level filtering.
        /// </summary>
        public LogSeverity MinimumLogSeverity { get; set; }

        /// <summary>
        /// Creates a new instance with default configuration values.
        /// </summary>
        public StoryConfiguration()
        {
            MaxDescendantCount = 200;
            MaxLogEntriesCount = 1000;
            EnableAutoRotation = true;
            MinimumLogSeverity = LogSeverity.Debug;
        }
    }
}
