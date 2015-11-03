using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Story.Core;
using Story.Core.Utils;
using System.Collections.Generic;

namespace Story.Ext.Handlers
{
    public class AzureTableStorageHandler : BufferedHandler
    {
        private readonly AzureTableStorageHandlerConfiguration _configuration;
        private CloudTable _storiesTable;

        public AzureTableStorageHandler(string name, AzureTableStorageHandlerConfiguration configuration)
            : base(name, configuration.BatchTimeDelay, configuration.BatchSize)
        {
            _configuration = configuration;

            Initialize();
        }

        private void Initialize()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(_configuration.ConnectionString);
            var tableClient = account.CreateCloudTableClient();
            _storiesTable = tableClient.GetTableReference(_configuration.TableName);
            _storiesTable.CreateIfNotExists();
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
                    _storiesTable.ExecuteBatch(tableBatchOperation);
                    return null;
                });
            }
            catch (StorageException)
            {
            }
        }
    }
}
