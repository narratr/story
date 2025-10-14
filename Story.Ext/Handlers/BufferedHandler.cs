using Story.Core;
using Story.Core.Handlers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Story.Ext.Handlers
{
    public abstract class BufferedHandler : StoryHandlerBase
    {
        private readonly Subject<IStory> _storiesSubject;

        protected BufferedHandler(string name, TimeSpan timeDelay, int batchSize, LogSeverity minimumLogSeverity = LogSeverity.Debug)
            : base(name)
        {
            MinimumLogSeverity = minimumLogSeverity;
            _storiesSubject = new Subject<IStory>();
            _storiesSubject.Buffer(timeDelay, batchSize)
                           .Where(buffer => buffer.Any())
                           .Subscribe(OnStoriesComplete, OnComplete);
        }

        /// <summary>
        /// Gets the minimum log severity level for logs to be included.
        /// Logs below this severity will be filtered out.
        /// Default is Debug (all logs included).
        /// </summary>
        protected LogSeverity MinimumLogSeverity { get; private set; }

        /// <summary>
        /// Filters log entries by severity level.
        /// Only returns logs with severity greater than or equal to MinimumLogSeverity.
        /// </summary>
        /// <param name="logs">The log entries to filter</param>
        /// <returns>Filtered log entries</returns>
        protected IEnumerable<IStoryLogEntry> FilterLogsBySeverity(IEnumerable<IStoryLogEntry> logs)
        {
            return logs.Where(log => log.Severity >= MinimumLogSeverity);
        }

        protected abstract void OnStoriesComplete(IList<IStory> stories);

        private void OnComplete()
        {
            throw new InvalidOperationException("subject should never complete");
        }

        public override void OnStop(IStory story)
        {
            _storiesSubject.OnNext(story);
        }
    }
}
