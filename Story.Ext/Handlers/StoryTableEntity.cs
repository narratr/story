using Microsoft.WindowsAzure.Storage.Table;
using Story.Core;
using Story.Core.Utils;
using System;

namespace Story.Ext.Handlers
{
    public class StoryTableEntity : TableEntity
    {
        public static StoryTableEntity ToStoryTableEntity(IStory story)
        {
            if (story == null)
            {
                return null;
            }

            var storyTableEntity = new StoryTableEntity()
            {
                Name = story.Name,
                Elapsed = story.Elapsed,
                StartDateTime = story.StartDateTime,
                InstanceId = story.InstanceId,
                Json = story.Serialize()
            };

            storyTableEntity.UpdateKeys(story);

            return storyTableEntity;
        }

        public string Name { get; set; }

        public string InstanceId { get; set; }

        public DateTime StartDateTime { get; set; }

        public TimeSpan Elapsed { get; set; }

        public string Json { get; set; }

        private void UpdateKeys(IStory story)
        {
            PartitionKey = String.Format(story.StartDateTime.ToString("yyyyMMddHH"));
            RowKey = story.StartDateTime.ToString("mmssffff") + story.InstanceId;
        }
    }
}
