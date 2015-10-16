namespace Story.Core.Utils
{
    using System;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// File watcher calls callback when the watched file is updated with the content of the file
    /// </summary>
    public class FileWatcher : IDisposable
    {
        private const int TimeoutUntilMakingChanges = 5 * 1000;

        private Timer makeChangesTimer;
        private FileSystemWatcher fileSystemWatcher;

        private readonly string watchedPath;
        private readonly Action<string> onFileChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatcher"/> class.
        /// Will invoke callback with current file content
        /// </summary>
        /// <param name="watchedPath">The watched path.</param>
        /// <param name="onFileChanged">The on file changed callback.</param>
        public FileWatcher(string watchedPath, Action<string> onFileChanged)
        {
            this.watchedPath = watchedPath;
            this.onFileChanged = onFileChanged;

            this.makeChangesTimer = new Timer(OnFileUpdated, null, 0, Timeout.Infinite);

            StartWatcher();
        }

        private void OnFileUpdated(object state)
        {
            // TODO: handle exception
            string fileContent = File.ReadAllText(this.watchedPath);
            onFileChanged(fileContent);
        }

        private void StartWatcher()
        {
            // Start file system watcher
            this.fileSystemWatcher = new FileSystemWatcher();
            this.fileSystemWatcher.Path = Path.GetDirectoryName(watchedPath);
            this.fileSystemWatcher.Filter = Path.GetFileName(watchedPath);
            this.fileSystemWatcher.Created += OnChanged;
            this.fileSystemWatcher.Changed += OnChanged;
            //this.fileSystemWatcher.Deleted += OnChanged;
            this.fileSystemWatcher.Error += OnError;
            this.fileSystemWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite;
            this.fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Exception ex = e.GetException();
            ResetWatcher();
        }

        private void ResetWatcher()
        {
            DisposeWatcher();
            StartWatcher();
        }

        private void DisposeWatcher()
        {
            if (this.fileSystemWatcher != null)
            {
                this.fileSystemWatcher.EnableRaisingEvents = false;
                this.fileSystemWatcher.Dispose();
                this.fileSystemWatcher = null;
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            this.makeChangesTimer.Change(TimeoutUntilMakingChanges, Timeout.Infinite);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeWatcher();

                if (this.makeChangesTimer != null)
                {
                    this.makeChangesTimer.Dispose();
                    this.makeChangesTimer = null;
                }
            }
        }
    }
}
