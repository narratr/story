namespace Story.Core.Handlers
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;

    using Utils;

    public class ConsoleHandler2 : ConsoleHandler
    {
        public override void OnStop(IStory story, Task task)
        {
            try
            {
                StringBuilder str = new StringBuilder();
                str.AppendFormat("{0}\n  Story {1}\n", story.StartDateTime, story.Name);

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

                Console.WriteLine(str);
                Trace.WriteLine(str); // TODO: remove
            }
            catch (Exception ex)
            {
                var c = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ForegroundColor = c;
            }
        }
    }

    public class LinesOnlyConsoleHandler : IStoryHandler
    {
        public void OnStart(IStory story)
        {
        }

        public void OnStop(IStory story, Task task)
        {
            foreach (var line in story.Log)
            {
                Console.WriteLine("{0} {1} {2}", line.DateTime, line.Severity, line.Text/*, line.Origin*/);
            }
        }
    }

    [Serializable]
    public class ConsoleHandler : IStoryHandler
    {
        public ConsoleHandler()
        {
            this.SeverityThreshold = LogSeverity.Debug;
        }

        public LogSeverity SeverityThreshold { get; set; }

        public void OnStart(IStory story)
        {
        }

        public virtual void OnStop(IStory story, Task task)
        {
            Ensure.ArgumentNotNull(story, "story");
            Ensure.ArgumentNotNull(task, "task");

            foreach (var entry in story.Log)
            {
                if (entry.Severity < this.SeverityThreshold)
                {
                    continue;
                }

                var formatted = string.Format("{0}|{1}|{2}|{3}", entry.DateTime, entry.Severity, entry.Elapsed, entry.Text);
                System.Console.WriteLine(formatted);
            }
        }
    }
}
