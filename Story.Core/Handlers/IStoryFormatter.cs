using Story.Core.Utils;
using System.Text;

namespace Story.Core.Handlers
{
    public interface IStoryFormatter
    {
        string FormatStory(IStory story);
    }

    public class BasicStoryFormatter : IStoryFormatter
    {
        public string FormatStory(IStory story)
        {
            StringBuilder str = new StringBuilder();
            str.AppendFormat("{0}\n  Story {1} took {2} ms\n", story.StartDateTime, story.Name, story.Elapsed.TotalMilliseconds);

            foreach (var item in story.Data)
            {
                if (item.Value != null)
                {
                    str.AppendFormat("  {0} - {1}\n", item.Key, item.Value.GetType().IsValueType ? item.Value : item.Value.Serialize());
                }
            }
            str.Append('\n');

            foreach (var line in story.Log)
            {
                str.AppendFormat("  +{0} ms {1} {2}\n", (line.DateTime - story.StartDateTime).TotalMilliseconds, line.Severity, line.Text/*, line.Origin*/);
            }

            return str.ToString();
        }
    }

    public class DelimiterStoryFormatter : IStoryFormatter
    {
        private readonly LogSeverity severityThreshold;

        public DelimiterStoryFormatter(LogSeverity SeverityThreshold)
        {
            this.severityThreshold = LogSeverity.Debug;
        }

        public string FormatStory(IStory story)
        {
            StringBuilder str = new StringBuilder();

            foreach (var entry in story.Log)
            {
                if (entry.Severity < this.severityThreshold)
                {
                    continue;
                }

                str.AppendFormat("{0}|{1}|{2}|{3}\n", entry.DateTime, entry.Severity, entry.Elapsed, entry.Text);
            }

            return str.ToString();
        }
    }
}
