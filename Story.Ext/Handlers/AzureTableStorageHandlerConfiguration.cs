using Story.Core;
using System;
using System.Configuration;

namespace Story.Ext.Handlers
{
    public class AzureTableStorageHandlerConfiguration
    {
        public AzureTableStorageHandlerConfiguration()
        {
            BatchSize = 50;

            BatchTimeDelay = TimeSpan.FromSeconds(5);

            var conn = ConfigurationManager.ConnectionStrings["StoryTableStorage"];
            if (conn != null)
            {
                ConnectionString = conn.ConnectionString;
            }

            TableName = "Stories";
        }

        public int BatchSize { get; set; }

        public TimeSpan BatchTimeDelay { get; set; }

        public string ConnectionString { get; set; }

        public string TableName { get; set; }

        /// <summary>
        /// Minimum log severity level for logs to be stored in Azure Table Storage.
        /// Logs below this severity will be filtered out.
        /// Default is Debug (all logs are stored).
        /// </summary>
        public LogSeverity MinimumLogSeverity { get; set; } = LogSeverity.Debug;
    }
}
