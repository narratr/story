namespace Story.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Utils;

    [Serializable]
    public class StoryLog : IStoryLog
    {
        private readonly List<IStoryLogEntry> entries = new List<IStoryLogEntry>();

        public StoryLog(IStory story)
        {
            Ensure.ArgumentNotNull(story, "story");
            this.Story = story;
        }

        protected IStory Story { get; private set; }

        public void Debug(
            string format,
            object[] args = null,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
            [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null)
        {
            this.Add(LogSeverity.Debug, format, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Info(
            string format,
            object[] args = null,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
            [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null)
        {
            this.Add(LogSeverity.Info, format, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Warn(
            string format,
            object[] args = null,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
            [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null)
        {
            this.Add(LogSeverity.Warning, format, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Error(
            string format,
            object[] args = null,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
            [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null)
        {
            this.Add(LogSeverity.Error, format, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Critical(
            string format,
            object[] args = null,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
            [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null)
        {
            this.Add(LogSeverity.Critical, format, args, callerMemberName, callerFilePath, callerLineNumber);
        }

        public virtual void Add(
            LogSeverity severity,
            string format,
            object[] args = null,
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null,
            [System.Runtime.CompilerServices.CallerLineNumber] int? callerLineNumber = null)
        {
            string text = args != null && args.Length > 0 ? string.Format(format, args) : format;

            // Get the current story and check for rotation
            var story = this.Story as Story;
            if (story != null)
            {
                // Get the root story
                var rootStory = story.GetRootStory();
                if (rootStory != null)
                {
                    // Increment log count on root
                    rootStory.IncrementLogCount();

                    // Only check for rotation if we're not already rotating
                    // This prevents infinite recursion when logging during rotation
                    if (!Utils.StoryRotationHelper.IsRotating)
                    {
                        // Check if rotation is needed
                        var limits = Storytelling.Limits;
                        string reason;
                        if (Utils.StoryRotationHelper.ShouldRotate(rootStory, limits, out reason))
                        {
                            // Rotate with the reason
                            var newRootStory = Utils.StoryRotationHelper.RotateStory(rootStory, reason);

                            // Add the log entry to the new story instead
                            if (newRootStory != null)
                            {
                                newRootStory.Log.Add(severity, format, args, callerMemberName, callerFilePath, callerLineNumber);
                                return;
                            }
                        }
                    }
                }
            }

            // No rotation needed or story is not a Story type - add normally
            this.entries.Add(new StoryLogEntry(severity, text, callerMemberName, callerFilePath, callerLineNumber));
        }

        IEnumerator<IStoryLogEntry> IEnumerable<IStoryLogEntry>.GetEnumerator()
        {
            return this.entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.entries.GetEnumerator();
        }
    }
}
