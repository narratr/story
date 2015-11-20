using Story.Core.Utils;
using System.Text;

namespace Story.Core.Handlers
{
    public interface IStoryFormatter
    {
        string FormatStory(IStory story, string handlerName);
    }

    public class BasicStoryFormatter : IStoryFormatter
    {
        private readonly LogSeverity severityThreshold;

        public BasicStoryFormatter(LogSeverity severityThreshold)
        {
            this.severityThreshold = severityThreshold;
        }

        public string FormatStory(IStory story, string handlerName)
        {
            StringBuilder str = new StringBuilder();
            str.AppendFormat("{0}\n  Story {1} ({2}) on rule {3}\n", story.StartDateTime, story.Name, story.InstanceId, handlerName);

            foreach (var item in story.GetDataValues())
            {
                if (item.Value != null)
                {
                    str.AppendFormat("  {0} - {1}\n", item.Key, item.Value.GetType().IsValueType ? item.Value : item.Value.Serialize());
                }
            }
            str.Append('\n');

            foreach (var line in story.GetLogs())
            {
                if (line.Severity < this.severityThreshold)
                {
                    continue;
                }

                str.AppendFormat("  +{0} ms {1} {2}\n", (line.DateTime - story.StartDateTime).TotalMilliseconds, line.Severity, line.Text/*, line.Origin*/);
            }
            str.Append('\n');

            AddStory(story, str, 1);
            str.Append('\n');

            return str.ToString();
        }

        public void AddStory(IStory story, StringBuilder str, int level)
        {
            var spaces = new StringBuilder();
            spaces.Append(' ', level * 2);
            str.AppendFormat("{0}- Story \"{1}\" took {2} ms\n", spaces, story.Name, story.Elapsed.Milliseconds);
            foreach (var childStory in story.Children)
            {
                AddStory(childStory, str, level + 1);
            }
        }
    }

    public class DelimiterStoryFormatter : IStoryFormatter
    {
        private readonly LogSeverity severityThreshold;

        public DelimiterStoryFormatter(LogSeverity severityThreshold)
        {
            this.severityThreshold = severityThreshold;
        }

        public string FormatStory(IStory story, string handlerName)
        {
            StringBuilder str = new StringBuilder();

            foreach (var entry in story.GetLogs())
            {
                if (entry.Severity < this.severityThreshold)
                {
                    continue;
                }

                str.AppendFormat("{0}|{1}|{2}|{3}\n", entry.DateTime, entry.Severity, (entry.DateTime - story.StartDateTime), entry.Text);
            }

            return str.ToString();
        }
    }
}
