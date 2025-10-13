namespace Story.Core
{
    using System;

    /// <summary>
    /// Contains information about story rotation/continuation.
    /// This object is null for original stories and populated for continuation stories.
    /// </summary>
    [Serializable]
    public class StoryRotationInfo
    {
        /// <summary>
        /// Gets the continuation sequence number. 1 for first continuation, 2 for second, etc.
        /// </summary>
        public int Sequence { get; internal set; }

        /// <summary>
        /// Gets the InstanceId of the original root story (before any rotations).
        /// </summary>
        public string OriginInstanceId { get; internal set; }

        /// <summary>
        /// Gets the reason for rotation.
        /// Possible values: "MaxDescendants", "MaxLogs".
        /// </summary>
        public string Reason { get; internal set; }

        /// <summary>
        /// Gets the original story name without continuation suffix.
        /// </summary>
        public string RootStoryName { get; internal set; }

        /// <summary>
        /// Creates a new instance of StoryRotationInfo.
        /// </summary>
        /// <param name="sequence">The continuation sequence number.</param>
        /// <param name="originInstanceId">The original story's instance ID.</param>
        /// <param name="reason">The reason for rotation.</param>
        /// <param name="rootStoryName">The original story name.</param>
        public StoryRotationInfo(int sequence, string originInstanceId, string reason, string rootStoryName)
        {
            this.Sequence = sequence;
            this.OriginInstanceId = originInstanceId;
            this.Reason = reason;
            this.RootStoryName = rootStoryName;
        }
    }
}
