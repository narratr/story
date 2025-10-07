using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Story.Ext.Handlers
{
    /// <summary>
    /// Custom JSON converter for DateTime to ISO 8601 format
    /// </summary>
    public class IsoDateTimeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                var dateTime = (DateTime)value;
                writer.WriteValue(dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            }
            else
            {
                writer.WriteValue(value);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return DateTime.Parse(reader.Value.ToString());
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }
    }


    /// <summary>
    /// Request object for batch story ingestion
    /// </summary>
    public class StoryBatchRequest
    {
        [JsonProperty("stories")]
        public List<StoryDto> Stories { get; set; }
        
        [JsonProperty("logEntries")]
        public List<LogEntryDto> LogEntries { get; set; }
        
        [JsonProperty("metadata")]
        public BatchMetadata Metadata { get; set; }

        public StoryBatchRequest()
        {
            Stories = new List<StoryDto>();
            LogEntries = new List<LogEntryDto>();
            Metadata = new BatchMetadata();
        }
    }

    /// <summary>
    /// Story DTO matching the API format
    /// </summary>
    public class StoryDto
    {
        [JsonProperty("instance_id")]
        public string InstanceId { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("started_at")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime StartedAt { get; set; }
        
        [JsonProperty("duration_ms")]
        public long? DurationMs { get; set; }
        
        [JsonProperty("parent_instance_id")]
        public string ParentInstanceId { get; set; }
        
        [JsonProperty("data")]
        public Dictionary<string, object> Data { get; set; }
        
        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Story.Core.StoryStatus Status { get; set; }
        
        [JsonProperty("thread_name")]
        public string ThreadName { get; set; }
        
        [JsonProperty("file_path")]
        public string FilePath { get; set; }
        
        [JsonProperty("method_name")]
        public string MethodName { get; set; }
        
        [JsonProperty("line_number")]
        public int? LineNumber { get; set; }
    }

    /// <summary>
    /// Log entry DTO matching the API format
    /// </summary>
    public class LogEntryDto
    {
        [JsonProperty("storyInstanceId")]
        public string StoryInstanceId { get; set; }
        
        [JsonProperty("severity")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Story.Core.LogSeverity Severity { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("logged_at")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime LoggedAt { get; set; }
        
        [JsonProperty("offset_ms")]
        public long OffsetMs { get; set; }
        
        [JsonProperty("thread_name")]
        public string ThreadName { get; set; }
        
        [JsonProperty("file_path")]
        public string FilePath { get; set; }
        
        [JsonProperty("method_name")]
        public string MethodName { get; set; }
        
        [JsonProperty("line_number")]
        public int? LineNumber { get; set; }
    }

    /// <summary>
    /// Batch metadata
    /// </summary>
    public class BatchMetadata
    {
        [JsonProperty("clientVersion")]
        public string ClientVersion { get; set; }
        
        [JsonProperty("rulesetVersion")]
        public int RulesetVersion { get; set; }

        public BatchMetadata()
        {
            RulesetVersion = 1;
        }
    }

    /// <summary>
    /// Result type for SendStoriesAsync operations
    /// </summary>
    public class SendStoriesResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        private static readonly SendStoriesResult _success = new SendStoriesResult { IsSuccess = true };

        public static SendStoriesResult Success()
        {
            return _success;
        }

        public static SendStoriesResult Error(string message)
        {
            return new SendStoriesResult 
            { 
                IsSuccess = false,
                ErrorMessage = message
            };
        }
    }
}
