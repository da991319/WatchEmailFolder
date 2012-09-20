using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Catel.Data;
using Catel.Logging;
using EmailInBox.Models;
using EmailInBox.Properties;
using Catel.MVVM;
using EmailInBox.Utils;

namespace EmailInBox.ViewModels
{

    /// <summary>
    /// UserControl view model.
    /// </summary>
    public class HomeWindowViewModel : WindowViewModelBase
    {
        private FileSystemWatcher watcher;
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeWindowViewModel"/> class.
        /// </summary>
        public HomeWindowViewModel():base()
        {
            FolderToWatch = Settings.Default.FolderToWatch;
            FileNumber = Settings.Default.NumberOfEmails;
            LogManager.RegisterDebugListener();
            InitializeWatcher();
            IconPath = "/Icons/email.ico";
            RowDoubleClick = new Command<MouseButtonEventArgs>(OnRowDoubleClickExecute, OnRowDoubleClickCanExecute);
            OnFileCreatedCmd = new Command(OnFileCreatedCmdExecute);
            //OnFileDeletedCmd = new Command(OnFileDeletedCmdExecute);
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
            //OnFileDeletedCmd.Execute(); 
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            //CheckMessages();
            //IconPath = "/Icons/new_email.ico";
            OnFileCreatedCmd.Execute();
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
        /// <summary>
        /// Gets the name command.
        /// </summary>
        public Command<MouseButtonEventArgs> RowDoubleClick { get; private set; }
        
        /// <summary>
        /// Method to check whether the name command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnRowDoubleClickCanExecute(MouseButtonEventArgs e)
        {
            return true;
        }

        /// <summary>
        /// Method to invoke when the name command is executed.
        /// </summary>
        private void OnRowDoubleClickExecute(MouseButtonEventArgs e)
        {
            DataGrid source = e.Source as DataGrid;

            if (source.SelectedItem != null)
            {
                MessageModel selectedMessage = source.SelectedItem as MessageModel;
                Process.Start(selectedMessage.Path);
            }
            

            int t = 2;
        }

        /// <summary>
        /// Gets the name command.
        /// </summary>
        public Command OnFileCreatedCmd { get; private set; }

        /// <summary>
        /// Method to invoke when the name command is executed.
        /// </summary>
        private void OnFileCreatedCmdExecute()
        {
            CheckMessages();
        }

        ///// <summary>
        ///// Gets the name command.
        ///// </summary>
        //public Command OnFileDeletedCmd { get; private set; }

        ///// <summary>
        ///// Method to invoke when the name command is executed.
        ///// </summary>
        //private void OnFileDeletedCmdExecute()
        //{
        //    CheckMessages();
        //}
    }
}
