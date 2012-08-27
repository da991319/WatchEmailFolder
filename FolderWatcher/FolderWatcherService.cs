using System;
using System.IO;
using System.ServiceProcess;

namespace FolderWatcher
{
    public partial class FolderWatcherService : ServiceBase
    {
        private string folderPath;
        private FileSystemWatcher watcher;

        public FolderWatcherService()
        {
            InitializeComponent();
        }

        public FolderWatcherService(string folderPath)
        {
            folderPath = folderPath;
        }

        protected override void OnStart(string[] args)
        {
            watcher = new FileSystemWatcher(folderPath, "*.eml")
                {
                    NotifyFilter = NotifyFilters.LastAccess
                                   | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName
                                   | NotifyFilters.DirectoryName
                };

            //Watch for changes in LastAccess and LastWrite times, and
            //the renaming of files or directories.

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        protected override void OnStop()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();

            //LogEvent("Monitoring Stopped");
        }

        void OnChanged(object sender, FileSystemEventArgs e)
        {
            string mgs = string.Format("File {0} | {1}",
                                       e.FullPath, e.ChangeType);
            //LogEvent(msg);
        }

        void OnRenamed(object sender, RenamedEventArgs e)
        {
            string log = string.Format("{0} | Renamed from {1}",
                                       e.FullPath, e.OldName);
            //LogEvent(log);
        }
    }
}
