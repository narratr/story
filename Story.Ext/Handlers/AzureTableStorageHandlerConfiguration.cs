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
    }
}
