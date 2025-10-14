using Story.Core;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Story.Ext.Handlers
{
    /// <summary>
    /// Configuration for the Story Log Store Handler
    /// </summary>
    public class StoryLogStoreHandlerConfiguration
    {
        public StoryLogStoreHandlerConfiguration()
        {
            // Default batch settings
            BatchSize = 200;
            BatchTimeDelay = TimeSpan.FromSeconds(20);

            // Default retry settings
            RetryCount = 3;
            RetryTimeout = 500; // milliseconds

            // Try to load API URL from configuration
            var apiUrl = ConfigurationManager.AppSettings["StoryLogStoreApiUrl"];
            if (!string.IsNullOrEmpty(apiUrl))
            {
                ApiBaseUrl = apiUrl;
            }

            // Try to load API key from configuration
            var apiKey = ConfigurationManager.AppSettings["StoryLogStoreApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                ApiKey = apiKey;
            }

            // Default metadata
            ClientVersion = "1.0.0";
            RulesetVersion = 1;
        }

        /// <summary>
        /// Number of stories to buffer before sending (max 1000)
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Time to buffer stories before sending them
        /// </summary>
        public TimeSpan BatchTimeDelay { get; set; }

        /// <summary>
        /// The Story Log Store API base URL
        /// </summary>
        public string ApiBaseUrl { get; set; }

        /// <summary>
        /// The API key for authentication
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Number of retry attempts on failure
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// Timeout between retries in milliseconds
        /// </summary>
        public int RetryTimeout { get; set; }

        /// <summary>
        /// Client version string
        /// </summary>
        public string ClientVersion { get; set; }

        /// <summary>
        /// Ruleset version number
        /// </summary>
        public int RulesetVersion { get; set; }

        /// <summary>
        /// Common data dictionary to be merged into all root stories in the batch.
        /// Only applied to root stories (stories without a parent).
        /// Individual story data values take precedence over batch data values in case of key conflicts.
        /// Useful for adding common context like host, environment, region, or deployment information.
        /// </summary>
        public Dictionary<string, object> BatchData { get; set; }

        /// <summary>
        /// Minimum log severity level for logs to be sent to Story Log Store.
        /// Logs below this severity will be filtered out.
        /// Default is Debug (all logs are sent).
        /// </summary>
        public LogSeverity MinimumLogSeverity { get; set; } = LogSeverity.Debug;
    }
}
