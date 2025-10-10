using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Story.Core.Utils;

namespace Story.Core.Handlers
{
    /// <summary>
    /// Compact story formatter that provides compact output with logs.
    /// Supports hierarchical stories and splits large log sets into manageable parts.
    /// </summary>
    public class CompactStoryFormatter : IStoryFormatter
    {
        private readonly LogSeverity severityThreshold;
        private readonly bool includeHierarchy;
        private readonly bool includeData;
        private readonly bool includeLogs;
        private readonly int maxEventsPerPart;

        public CompactStoryFormatter(LogSeverity severityThreshold = LogSeverity.Info, 
                                    bool includeHierarchy = true, 
                                    bool includeData = true, 
                                    bool includeLogs = true,
                                    int maxEventsPerPart = 10)
        {
            this.severityThreshold = severityThreshold;
            this.includeHierarchy = includeHierarchy;
            this.includeData = includeData;
            this.includeLogs = includeLogs;
            this.maxEventsPerPart = maxEventsPerPart;
        }

        public string FormatStory(IStory story, string handlerName)
        {
            var result = new StringBuilder();

            // Create a unified list of events (logs and child events)
            var events = new List<IEvent>();

            // Add log entries as events
            if (includeLogs)
            {
                foreach (var entry in story.GetLogs())
                {
                    if (entry.Severity >= severityThreshold)
                    {
                        events.Add(new LogEvent(entry));
                    }
                }
            }

            // Add substory events and their logs if hierarchy is included
            if (includeHierarchy && story.Children.Any())
            {
                foreach (var child in story.Children)
                {
                    // Calculate child timing relative to parent start
                    var childStartMs = (long)(child.StartDateTime - story.StartDateTime).TotalMilliseconds;

                    // Add the substory event (single event with duration and data)
                    events.Add(new SubstoryEvent(child, includeData));

                    // Add child's log entries
                    if (includeLogs)
                    {
                        foreach (var childEntry in child.GetLogs())
                        {
                            if (childEntry.Severity >= severityThreshold)
                            {
                                // Calculate the child log timing relative to parent start
                                var childLogMs = (long)(childEntry.DateTime - story.StartDateTime).TotalMilliseconds;
                                events.Add(new ChildLogEvent(childEntry, child.Name));
                            }
                        }
                    }
                }
            }

            // If no events, return compact format
            if (!includeLogs || !events.Any())
            {
                return FormatAsCompact(story);
            }

            // Sort events by elapsed time
            events.Sort((a, b) => a.GetElapsedMs(story).CompareTo(b.GetElapsedMs(story)));

            // Split events into parts
            var totalEvents = events.Count;
            var parts = (totalEvents + maxEventsPerPart - 1) / maxEventsPerPart; // Ceiling division

            for (int part = 0; part < parts; part++)
            {
                if (part > 0)
                {
                    result.AppendLine(); // Separate parts with newline
                }

                // Add header using structured format style
                var headerParts = new List<string>
                {
                    $"[STORY] {story.Name}{(parts > 1 ? $" ({part + 1}/{parts})" : "")}",
                    $"ID: {story.InstanceId}"
                };

                if (part == 0)
                {
                    headerParts.Add($"Duration: {story.Elapsed.TotalMilliseconds:F0}ms");
                    if (includeData && story.Data.Any())
                    {
                        var dataString = string.Join(", ", story.Data.Select(d => $"{d.Key}={d.Value.TruncateWithEllipsis(20)}"));
                        headerParts.Add($"Data: {dataString}");
                    }
                }

                result.AppendLine(string.Join(" | ", headerParts));

                // Add events for this part (up to maxEventsPerPart)
                var startIdx = part * maxEventsPerPart;
                var endIdx = Math.Min(startIdx + maxEventsPerPart, totalEvents);

                for (int i = startIdx; i < endIdx; i++)
                {
                    var eventItem = events[i];
                    result.AppendLine($"  {eventItem.Format(story)}");
                }
            }

            return result.ToString();
        }

        private string FormatAsCompact(IStory story)
        {
            var highestSeverity = GetHighestSeverity(story);
            var status = highestSeverity >= LogSeverity.Error ? "ERROR" : "OK";

            return $"[{status}] {story.Name} ({(long)story.Elapsed.TotalMilliseconds}ms) {story.InstanceId}";
        }

        private LogSeverity GetHighestSeverity(IStory story)
        {
            var highest = LogSeverity.Debug;
            foreach (var entry in story.GetLogs())
            {
                if (entry.Severity > highest)
                {
                    highest = entry.Severity;
                }
            }
            return highest;
        }

        // Inner interfaces and classes for event handling
        private interface IEvent
        {
            long GetElapsedMs(IStory story);
            string Format(IStory story);
        }

        private class LogEvent : IEvent
        {
            private readonly IStoryLogEntry entry;

            public LogEvent(IStoryLogEntry entry)
            {
                this.entry = entry;
            }

            public long GetElapsedMs(IStory story)
            {
                return (long)(entry.DateTime - story.StartDateTime).TotalMilliseconds;
            }

            public string Format(IStory story)
            {
                return $"[{GetElapsedMs(story)}ms] {entry.Severity}: {entry.Text}";
            }
        }

        private class SubstoryEvent : IEvent
        {
            private readonly IStory child;
            private readonly bool includeData;

            public SubstoryEvent(IStory child, bool includeData)
            {
                this.child = child;
                this.includeData = includeData;
            }

            public long GetElapsedMs(IStory story)
            {
                return (long)(child.StartDateTime - story.StartDateTime).TotalMilliseconds;
            }

            public string Format(IStory story)
            {
                var sb = new StringBuilder();
                sb.Append($"[{GetElapsedMs(story)}ms] [SUBSTORY] {child.Name} ({(long)child.Elapsed.TotalMilliseconds}ms) {child.InstanceId}");

                if (includeData && child.Data.Any())
                {
                    var dataString = string.Join(", ", child.Data.Select(d => $"{d.Key}={d.Value.TruncateWithEllipsis(20)}"));
                    sb.Append($" Data: {dataString}");
                }

                return sb.ToString();
            }
        }

        private class ChildLogEvent : IEvent
        {
            private readonly IStoryLogEntry entry;
            private readonly string childName;

            public ChildLogEvent(IStoryLogEntry entry, string childName)
            {
                this.entry = entry;
                this.childName = childName;
            }

            public long GetElapsedMs(IStory story)
            {
                return (long)(entry.DateTime - story.StartDateTime).TotalMilliseconds;
            }

            public string Format(IStory story)
            {
                return $"[{GetElapsedMs(story)}ms]   [{childName}] {entry.Severity}: {entry.Text}";
            }
        }
    }
}
