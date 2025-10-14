using Azure;
using Azure.Data.Tables;
using Story.Core;
using Story.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Story.Ext.Handlers
{
    public class AzureTableStorageHandler : BufferedHandler
    {
        private readonly AzureTableStorageHandlerConfiguration configuration;
        private TableClient storiesTable;

        public AzureTableStorageHandler(string name, AzureTableStorageHandlerConfiguration configuration)
            : base(name, configuration.BatchTimeDelay, configuration.BatchSize, configuration.MinimumLogSeverity)
        {
            this.configuration = configuration;

            Initialize();
        }

        private void Initialize()
        {
            if (this.configuration.ConnectionString == null)
            {
                throw new InvalidOperationException("Missing StoryTableStorage connection string");
            }

            this.storiesTable = new TableClient(this.configuration.ConnectionString, this.configuration.TableName);
            this.storiesTable.CreateIfNotExists();
        }

        protected override void OnStoriesComplete(IList<IStory> stories)
        {
            PersistStoriesAsync(stories);
        }

        private void PersistStoriesAsync(IList<IStory> stories)
        {
            var tableEntities = new List<StoryTableEntity>();
            foreach (var story in stories)
            {
                // TODO: Utilize FilterLogsBySeverity to filter out logs below the minimum log severity
                tableEntities.Add(StoryTableEntity.ToStoryTableEntity(story));
            }

            try
            {
                Retrier.Retry<object>(() =>
                {
                    this.storiesTable.SubmitTransaction(tableEntities.Select(entity => new TableTransactionAction(TableTransactionActionType.Add, entity)).ToList());
                    return null;
                });
            }
            catch (RequestFailedException)
            {
            }
        }
    }
}
