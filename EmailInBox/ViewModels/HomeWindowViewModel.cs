using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Catel.Data;
using Catel.Logging;
using EmailInBox.Models;
using EmailInBox.Properties;

namespace EmailInBox.ViewModels
{
    using Catel.MVVM;

    /// <summary>
    /// UserControl view model.
    /// </summary>
    public class HomeWindowViewModel : ViewModelBase
    {
        private FileSystemWatcher watcher;
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeWindowViewModel"/> class.
        /// </summary>
        public HomeWindowViewModel()
        {
            FolderToWatch = Settings.Default.FolderToWatch;
            FileNumber = Settings.Default.NumberOfEmails;
            LogManager.RegisterDebugListener();
            InitializeWatcher();
        }

        private void InitializeWatcher()
        {
            watcher = new FileSystemWatcher(FolderToWatch, "*.eml")
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
            CheckMessages();
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            CheckMessages();
        }

        private void CheckMessages()
        {
            Messages = new ObservableCollection<MessageModel>(Utils.FilesRetrieve.RetrieveEmail(FolderToWatch, FileNumber));
        }

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public ObservableCollection<MessageModel> Messages
        {
            get { return GetValue<ObservableCollection<MessageModel>>(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        /// <summary>
        /// Register the name property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MessagesProperty = RegisterProperty("Messages", typeof(ObservableCollection<MessageModel>), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public string FolderToWatch
        {
            get { return GetValue<string>(FolderToWatchProperty); }
            set { SetValue(FolderToWatchProperty, value); }
        }

        /// <summary>
        /// Register the FolderToWatch property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FolderToWatchProperty = RegisterProperty("FolderToWatch", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public int FileNumber
        {
            get { return GetValue<int>(FileNumberProperty); }
            set
            {
                SetValue(FileNumberProperty, value);
                Messages = new ObservableCollection<MessageModel>(Utils.FilesRetrieve.RetrieveEmail(FolderToWatch, FileNumber));
            }
        }

        /// <summary>
        /// Register the FileNumber property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FileNumberProperty = RegisterProperty("FileNumber", typeof(int), null);
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

    }
}
