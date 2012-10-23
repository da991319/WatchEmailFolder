﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Catel.Data;
using Catel.Logging;
using Catel.Messaging;
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
        private IMessageMediator mediator = MessageMediator.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeWindowViewModel"/> class.
        /// </summary>
        public HomeWindowViewModel():base()
        {
            FolderToWatch = Settings.Default.FolderToWatch;
            FileNumber = Settings.Default.NumberOfEmails;
            LogManager.RegisterDebugListener();
            InitializeWatcher();
            RowDoubleClick = new Command<MouseButtonEventArgs>(OnRowDoubleClickExecute, OnRowDoubleClickCanExecute);
            OnFileCreatedCmd = new Command<FileSystemEventArgs>(OnFileCreatedCmdExecute,null,"FileCreatedCommand");
            CheckMessagesCommand = new Command(OnCheckMessagesCommandExecute);
            CheckMessagesCommand.Execute();
            
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
            CheckMessagesCommand.Execute(); 
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            OnFileCreatedCmd.Execute(e);
        }

        public ObservableCollection<MessageModel> Messages
        {
            get { return GetValue<ObservableCollection<MessageModel>>(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        public static readonly PropertyData MessagesProperty = RegisterProperty("Messages", typeof(ObservableCollection<MessageModel>), null);

        public string FolderToWatch
        {
            get { return GetValue<string>(FolderToWatchProperty); }
            set { SetValue(FolderToWatchProperty, value); }
        }

        public static readonly PropertyData FolderToWatchProperty = RegisterProperty("FolderToWatch", typeof(string), null);

        public int FileNumber
        {
            get { return GetValue<int>(FileNumberProperty); }
            set { SetValue(FileNumberProperty, value); }
        }

        public static readonly PropertyData FileNumberProperty = RegisterProperty("FileNumber", typeof(int), null);
        
        public Command<MouseButtonEventArgs> RowDoubleClick { get; private set; }
        
        private bool OnRowDoubleClickCanExecute(MouseButtonEventArgs e)
        {
            return true;
        }

        private void OnRowDoubleClickExecute(MouseButtonEventArgs e)
        {
            var source = e.Source as DataGrid;

            if (source.SelectedItem != null)
            {
                MessageModel selectedMessage = source.SelectedItem as MessageModel;
                Process.Start(selectedMessage.Path);
            }
        }

        public Command<FileSystemEventArgs> OnFileCreatedCmd { get; private set; }

        private void OnFileCreatedCmdExecute(FileSystemEventArgs e)
        {
            CheckMessagesCommand.Execute();
        }

        public Command CheckMessagesCommand { get; private set; }

        private void OnCheckMessagesCommandExecute()
        {
            DateTime reference;
            MessageModel message = (Messages ?? new ObservableCollection<MessageModel>()).FirstOrDefault();
            reference = message != null ? message.DateReceived : DateTime.Now;
            //var mediator = GetService<IMessageMediator>();
            if (message != null)
            {
                mediator.SendMessage<MessageModel>(message, "New Message");
            }
            
            Messages = new ObservableCollection<MessageModel>(Utils.FilesRetrieve.RetrieveEmails(FolderToWatch, FileNumber, reference)); 
        }
    }
}
