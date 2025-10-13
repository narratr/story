using Newtonsoft.Json;
using Story.Core;
using Story.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Story.Ext.Handlers
{
    /// <summary>
    /// Handler that batches and sends stories to the Story Log Store API
    /// </summary>
    public class StoryLogStoreHandler : BufferedHandler
    {
        private readonly StoryLogStoreHandlerConfiguration configuration;
        private readonly HttpClient httpClient;
        private readonly string url;

        public StoryLogStoreHandler(string name, StoryLogStoreHandlerConfiguration configuration)
            : base(name, configuration.BatchTimeDelay, configuration.BatchSize)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (string.IsNullOrEmpty(configuration.ApiBaseUrl))
            {
                throw new InvalidOperationException("ApiBaseUrl is required in StoryLogStoreHandlerConfiguration");
            }

            if (string.IsNullOrEmpty(configuration.ApiKey))
            {
                throw new InvalidOperationException("ApiKey is required in StoryLogStoreHandlerConfiguration");
            }

            this.configuration = configuration;
            this.url = this.configuration.ApiBaseUrl.TrimEnd('/') + "/api/v1/stories/batch";

            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Add("X-API-Key", configuration.ApiKey);
        }

        public override void OnStop(IStory story)
        {
            if (story != null && story.IsRoot())
            {
                base.OnStop(story);
            }
        }

        protected override void OnStoriesComplete(IList<IStory> stories)
        {
            // Fire and forget - don't block the calling thread
            Task.Run(async () =>
            {
                var result = await SendStoriesAsync(stories);
                if (!result.IsSuccess)
                {
                    // Log error but don't throw - we don't want to crash the application
                    System.Diagnostics.Trace.TraceWarning(result.ErrorMessage);
                }
            });
        }

        private async Task<SendStoriesResult> SendStoriesAsync(IList<IStory> stories)
        {
            try
            {
                var batchRequest = CreateBatchRequest(stories);

                if (batchRequest.Stories.Count == 0)
                {
                    return SendStoriesResult.Success(); // Nothing to send
                }

                var json = JsonConvert.SerializeObject(batchRequest);

                // Use retry logic
                return await Retrier.RetryAsync<SendStoriesResult>(async () =>
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(this.url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        return SendStoriesResult.Success();
                    }

                    if ((int)response.StatusCode >= 500 || (int)response.StatusCode == 429)
                    {
                        // This will throw an exception to trigger retry
                        response.EnsureSuccessStatusCode();
                    }

                    // For non-retryable errors, throw exception to exit retry loop
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var errorMessage = string.Format("HTTP {0}: {1}", (int)response.StatusCode, responseContent);
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        errorMessage += string.Format(" - {0}", responseContent);
                    }

                    return SendStoriesResult.Error(errorMessage);
                }, configuration.RetryCount, configuration.RetryTimeout);
            }
            catch (Exception ex)
            {
                return SendStoriesResult.Error(ex.ToString());
            }
        }

        private StoryBatchRequest CreateBatchRequest(IList<IStory> stories)
        {
            var request = new StoryBatchRequest
            {
                BatchData = configuration.BatchData,
                Metadata = new BatchMetadata
                {
                    ClientVersion = configuration.ClientVersion,
                    RulesetVersion = configuration.RulesetVersion
                }
            };

            // Flatten all stories (root + all children) with parent tracking
            var allStoriesWithParents = new List<(IStory story, IStory parent)>();
            foreach (var story in stories)
            {
                allStoriesWithParents.AddRange(FlattenStoryWithParent(story, null));
            }

            // Convert stories to DTOs
            foreach (var (story, parent) in allStoriesWithParents)
            {
                request.Stories.Add(ConvertToStoryDto(story, parent));

                // Convert log entries to DTOs
                foreach (var logEntry in story.Log)
                {
                    request.LogEntries.Add(ConvertToLogEntryDto(story, logEntry));
                }
            }

            return request;
        }

        private IEnumerable<IStory> FlattenStory(IStory story)
        {
            yield return story;

            if (story.Children != null)
            {
                foreach (var child in story.Children)
                {
                    foreach (var descendant in FlattenStory(child))
                    {
                        yield return descendant;
                    }
                }
            }
        }

        private IEnumerable<(IStory story, IStory parent)> FlattenStoryWithParent(IStory story, IStory parent)
        {
            yield return (story, parent);

            if (story.Children != null)
            {
                foreach (var child in story.Children)
                {
                    foreach (var descendant in FlattenStoryWithParent(child, story))
                    {
                        yield return descendant;
                    }
                }
            }
        }

        // Parent is passed since in the story it'll be null once the story is stopped
        private StoryDto ConvertToStoryDto(IStory story, IStory parent)
        {
            var dto = new StoryDto
            {
                InstanceId = story.InstanceId,
                Name = story.Name,
                StartedAt = story.StartDateTime,
                DurationMs = (long?)story.Elapsed.TotalMilliseconds,
                ParentInstanceId = parent != null ? parent.InstanceId : null,
                Status = story.Status,
                ThreadName = story.ThreadName,
                FilePath = story.FilePath,
                MethodName = story.MethodName,
                LineNumber = story.LineNumber,
                Data = new Dictionary<string, object>()
            };

            // Convert story data to dictionary
            foreach (var kvp in story.Data)
            {
                try
                {
                    if (kvp.Value != null)
                    {
                        dto.Data[kvp.Key] = SerializeValue(kvp.Value);
                    }
                }
                catch
                {
                    // Skip non-serializable data
                }
            }

            return dto;
        }

        private LogEntryDto ConvertToLogEntryDto(IStory story, IStoryLogEntry logEntry)
        {
            // Calculate offset from story start
            var offset = logEntry.DateTime - story.StartDateTime;

            return new LogEntryDto
            {
                StoryInstanceId = story.InstanceId,
                Severity = logEntry.Severity,
                Message = logEntry.Text,
                LoggedAt = logEntry.DateTime,
                OffsetMs = (long)offset.TotalMilliseconds,
                ThreadName = logEntry.ThreadName,
                FilePath = logEntry.FilePath,
                MethodName = logEntry.MethodName,
                LineNumber = logEntry.LineNumber
            };
        }

        private object SerializeValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            var type = value.GetType();

            // Handle primitive types and strings directly
            if (type.IsPrimitive)
            {
                return value;
            }

            return value.ToString();
        }

    }
}
