
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Catel.Messaging;
using EmailInBox.Properties;

namespace EmailInBox.Utils
{
    public interface IFolderWatcher
    {
        void ChangeWatcherFolder(string folderToWatch);
    }

    public class FolderWatcher : IFolderWatcher
    {
        private readonly IMessageMediator mediator;
        private FileSystemWatcher watcher;
        private string folderToWatch ;

        public FolderWatcher(IMessageMediator mediator)
        {
            this.mediator = mediator;
            InitializeWatcher();
        }

        private void InitializeWatcher()
        {
            if (String.IsNullOrWhiteSpace(folderToWatch))
                folderToWatch = Settings.Default.FolderToWatch;

            watcher = new FileSystemWatcher(folderToWatch, "*.eml")
            {
                NotifyFilter = NotifyFilters.LastAccess
                     | NotifyFilters.LastWrite
                     | NotifyFilters.FileName
                     | NotifyFilters.DirectoryName
            };

            var switchThreadForFsEvent = (Func<FileSystemEventHandler, FileSystemEventHandler>)(
        (FileSystemEventHandler handler) =>
                (object obj, FileSystemEventArgs e) =>
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                        handler(obj, e))));

            watcher.Created += switchThreadForFsEvent(OnFileCreated);
            watcher.Deleted += switchThreadForFsEvent(OnFileDeleted);

            watcher.EnableRaisingEvents = true;
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            mediator.SendMessage("File Deleted", "folderModified");
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            mediator.SendMessage("File Added", "folderModified");
        }

        public void ChangeWatcherFolder(string folderToWatch)
        {
            if (watcher != null)
                watcher.Path = folderToWatch;
            else
                InitializeWatcher();
        }
    }
}
