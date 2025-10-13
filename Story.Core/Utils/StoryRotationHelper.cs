using System;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Web;

namespace Story.Core.Utils
{
    /// <summary>
    /// Helper class for managing automatic story rotation to prevent unbounded memory growth.
    /// </summary>
    internal static class StoryRotationHelper
    {
        // Thread-safe flag to ensure only one rotation happens at a time
        private static int _isRotating = 0;

        /// <summary>
        /// Gets whether a rotation is currently in progress.
        /// </summary>
        public static bool IsRotating
        {
            get { return System.Threading.Interlocked.CompareExchange(ref _isRotating, 0, 0) != 0; }
        }

        /// <summary>
        /// Checks if the given root story should be rotated based on configured limits.
        /// </summary>
        /// <param name="rootStory">The root story to check.</param>
        /// <param name="limits">The configured limits.</param>
        /// <param name="reason">Output parameter containing the reason for rotation if true is returned.</param>
        /// <returns>True if rotation should occur, false otherwise.</returns>
        public static bool ShouldRotate(Story rootStory, StoryLimits limits, out string reason)
        {
            reason = null;

            if (rootStory == null || limits == null || !limits.EnableAutoRotation)
            {
                return false;
            }

            // Check descendant count
            if (rootStory.DescendantCount >= limits.MaxDescendantCount)
            {
                reason = "MaxDescendants";
                return true;
            }

            // Check log entry count
            if (rootStory.TotalLogCount >= limits.MaxLogEntriesCount)
            {
                reason = "MaxLogs";
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates a story by stopping the old one and creating a continuation story.
        /// This method is thread-safe and only allows one rotation at a time.
        /// </summary>
        /// <param name="oldStory">The story to rotate.</param>
        /// <param name="reason">The reason for rotation (e.g., "MaxDescendants", "MaxLogs").</param>
        /// <returns>The new continuation story, or the old story if rotation couldn't occur.</returns>
        public static Story RotateStory(Story oldStory, string reason)
        {
            if (oldStory == null)
            {
                return null;
            }

            // Thread-safe check: only one thread should perform rotation
            if (Interlocked.CompareExchange(ref _isRotating, 1, 0) != 0)
            {
                // Another thread is already rotating, return the old story
                return oldStory;
            }

            try
            {
                // Verify this is still relevant to rotate
                var contextStory = Story.FromContext() as Story;

                // Allow rotation if context is the old story OR a child of it (being attached)
                bool isOldStoryOrChild = contextStory == oldStory ||
                    (contextStory != null && contextStory.GetRootStory() == oldStory);

                if (!isOldStoryOrChild)
                {
                    // Context has changed to an unrelated story, no need to rotate
                    return contextStory ?? oldStory;
                }

                // Stop the old story (this will call handlers and cleanup)
                oldStory.Stop();

                // Determine sequence and root name
                var oldRotationInfo = oldStory.RotationInfo;
                var newSequence = (oldRotationInfo?.Sequence ?? 0) + 1;
                var rootStoryName = oldRotationInfo?.RootStoryName ?? oldStory.Name;
                var originInstanceId = oldRotationInfo?.OriginInstanceId ?? oldStory.InstanceId;

                // Create the new continuation story
                var continuationName = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} [-> {1}]",
                    rootStoryName,
                    newSequence);

                // Create new story with notInContext: true to prevent it from attaching to itself
                var newStory = new Story(
                    continuationName,
                    oldStory.HandlerProvider,
                    notInContext: true,
                    callerMemberName: oldStory.MethodName,
                    callerFilePath: oldStory.FilePath,
                    callerLineNumber: oldStory.LineNumber);

                // Set rotation info with the provided reason on the root state
                newStory._rootState.RotationInfo = new StoryRotationInfo(
                    newSequence,
                    originInstanceId,
                    reason,
                    rootStoryName);

                // Replace the old story in the context with the new one
                ReplaceInContext(newStory);

                // Start the new story
                newStory.Start();

                // Log the rotation event
                newStory.Log.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Story rotated from '{0}' ({1}) due to {2}. -> #{3}.",
                        oldStory.Name,
                        oldStory.InstanceId,
                        reason ?? "limits exceeded",
                        newSequence));

                return newStory;
            }
            finally
            {
                // Reset the rotating flag
                Interlocked.Exchange(ref _isRotating, 0);
            }
        }

        /// <summary>
        /// Replaces the current story in the context (CallContext or HttpContext).
        /// </summary>
        private static void ReplaceInContext(Story newStory)
        {
            var contextKey = ContextBoundObject<IStory>.ContextKey;

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[contextKey] = newStory;
            }
            else
            {
                CallContext.LogicalSetData(contextKey, newStory);
            }
        }
    }
}
