using Newtonsoft.Json;

namespace Story.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public enum StoryStatus
    {
        /// <summary>
        /// The story is currently executing or in progress.
        /// </summary>
        Running,

        /// <summary>
        /// The story has finished executing successfully.
        /// </summary>
        Completed,

        /// <summary>
        /// The story has failed due to an error or exception.
        /// </summary>
        Failed,

        /// <summary>
        /// The story has timed out and was terminated due to exceeding the maximum allowed execution time.
        /// </summary>
        Timeout,

        /// <summary>
        /// The story was cancelled before completion, either by user request or system intervention.
        /// </summary>
        Cancelled
    }

    public interface IStory : IDisposable
    {
        /// <summary>
        /// Gets the friendly-name of the story.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the instance identifier of the story.
        /// </summary>
        string InstanceId { get; }

        /// <summary>
        /// Gets or sets the task observed by this story.
        /// </summary>
        Task Task { get; set; }

        /// <summary>
        /// Starts this story and invokes the start handlers.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this story and invokes the stop handlers.
        /// </summary>
        void Stop();

        /// <summary>
        /// Report a failure for this story.
        /// </summary>
        void ReportFailed();

        /// <summary>
        /// Update the status for this story.
        /// </summary>
        void UpdateStatus(StoryStatus status);

        /// <summary>
        /// Gets the elapsed time-span.
        /// </summary>
        TimeSpan Elapsed { get; }

        /// <summary>
        /// Gets the start date time.
        /// </summary>
        DateTime StartDateTime { get; }

        /// <summary>
        /// Gets the story data.
        /// </summary>
        IStoryData Data { get; }

        /// <summary>
        /// Gets the story log.
        /// </summary>
        IStoryLog Log { get; }

        /// <summary>
        /// Gets the story parent or null or this is a root story.
        /// </summary>
        [JsonIgnore]
        IStory Parent { get; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        IEnumerable<IStory> Children { get; }

        /// <summary>
        /// Gets the story handler provider.
        /// </summary>
        IRuleset<IStory, IStoryHandler> HandlerProvider { get; }

        /// <summary>
        /// Gets the current status of the story.
        /// </summary>
        StoryStatus Status { get; }
    }
}
