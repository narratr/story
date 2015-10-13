namespace Story.Core.Handlers
{
    using System;
    using System.Threading.Tasks;

    using Utils;

    [Serializable]
    public class ConsoleHandler : IStoryHandler
    {
        public ConsoleHandler()
        {
            this.SeverityThreshold = LogSeverity.Warning;
        }

        public LogSeverity SeverityThreshold { get; set; }

        public void OnStart(IStory story)
        {
        }

        public void OnStop(IStory story, Task task)
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
