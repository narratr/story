namespace Story.Core
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class StoryLogEntry : IStoryLogEntry
    {
        private readonly DateTime dateTime;
        private readonly LogSeverity severity;
        private readonly string text;

        public StoryLogEntry(LogSeverity severity, string text, 
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int? callerLineNumber = null)
        {
            this.dateTime = DateTime.UtcNow;
            this.severity = severity;
            this.text = text;
            
            // Capture thread name
            var currentThread = System.Threading.Thread.CurrentThread;
            this.ThreadName = !string.IsNullOrEmpty(currentThread.Name)
                ? currentThread.Name
                : currentThread.ManagedThreadId.ToString();
            
            // Capture caller information
            this.MethodName = callerMemberName;
            this.LineNumber = callerLineNumber;
            
            // Store the full file path
            this.FilePath = callerFilePath;
        }

        public LogSeverity Severity
        {
            get { return this.severity; }
        }

        public string Text
        {
            get { return this.text; }
        }

        public DateTime DateTime
        {
            get { return this.dateTime; }
        }

        public string ThreadName { get; private set; }

        public string FilePath { get; private set; }

        public string MethodName { get; private set; }

        public int? LineNumber { get; private set; }
    }
}
