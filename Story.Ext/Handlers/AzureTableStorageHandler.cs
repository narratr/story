using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Story.Core;
using Story.Core.Utils;
using System;
using System.Collections.Generic;

namespace Story.Ext.Handlers
{
    public class AzureTableStorageHandler : BufferedHandler
    {
        private readonly AzureTableStorageHandlerConfiguration configuration;
        private CloudTable storiesTable;

        public AzureTableStorageHandler(string name, AzureTableStorageHandlerConfiguration configuration)
            : base(name, configuration.BatchTimeDelay, configuration.BatchSize)
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

            CloudStorageAccount account = CloudStorageAccount.Parse(this.configuration.ConnectionString);
            var tableClient = account.CreateCloudTableClient();
            this.storiesTable = tableClient.GetTableReference(this.configuration.TableName);
            this.storiesTable.CreateIfNotExists();
        }

        protected override void OnStoriesComplete(IList<IStory> stories)
        {
            PersistStoriesAsync(stories);
        }

        private void PersistStoriesAsync(IList<IStory> stories)
        {
            var tableBatchOperation = new TableBatchOperation();
            foreach (var story in stories)
            {
                tableBatchOperation.Add(TableOperation.Insert(StoryTableEntity.ToStoryTableEntity(story)));
            }

            try
            {
                Retrier.Retry<object>(() =>
                {
                    this.storiesTable.ExecuteBatch(tableBatchOperation);
                    return null;
                });
            }
            catch (StorageException)
            {
            }
        }
    }
}
