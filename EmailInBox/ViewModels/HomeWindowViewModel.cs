using System.ComponentModel;
using Catel.Data;
using Catel.Logging;
using Catel.Messaging;
using Catel.MVVM;
using EmailInBox.Models;
using EmailInBox.Utils;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace EmailInBox.ViewModels
{

    /// <summary>
    /// UserControl view model.
    /// </summary>
    /// 
    /// 
    [InterestedIn(typeof (SettingsWindowViewModel))]
    [InterestedIn(typeof(MainWindowViewModel))]
    public class HomeWindowViewModel : WindowViewModelBase
    {
        private FileSystemWatcher watcher;
        private IMessageMediator mediator = MessageMediator.Default;
        private string folderToWatch ;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeWindowViewModel"/> class.
        /// </summary>
        public HomeWindowViewModel():base()
        {
            LogManager.RegisterDebugListener();
            //InitializeWatcher();
            RowDoubleClick = new Command<MouseButtonEventArgs>(OnRowDoubleClickExecute, OnRowDoubleClickCanExecute);
            OnFileCreatedCmd = new Command<FileSystemEventArgs>(OnFileCreatedCmdExecute,null,"FileCreatedCommand");
            CheckMessagesCommand = new Command(OnCheckMessagesCommandExecute);
            Messages = new InitialLoadCommand().Load();
            CheckMessagesCommand.Execute();
        }

        public override string Title { get { return "Home"; } }

        private void InitializeWatcher()
        {
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

        private void ChangeWatcherSettings()
        {
            if (watcher != null)
                watcher.Path = folderToWatch;
            else
                InitializeWatcher();
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

        
        public Command<MouseButtonEventArgs> RowDoubleClick { get; private set; }
        
        private bool OnRowDoubleClickCanExecute(MouseButtonEventArgs e)
        {
            return true;
        }

        private void OnRowDoubleClickExecute(MouseButtonEventArgs e)
        {
            var source = e.Source as ListView;

            if (source.SelectedItem == null) return;
            MessageModel selectedMessage = source.SelectedItem as MessageModel;
            Messages.ToList().Find(m => m == selectedMessage).NewEmail = false;
            Process.Start(selectedMessage.Path);
        }

        public Command<FileSystemEventArgs> OnFileCreatedCmd { get; private set; }

        private void OnFileCreatedCmdExecute(FileSystemEventArgs e)
        {
            CheckMessagesCommand.Execute();
        }

        public Command CheckMessagesCommand { get; private set; }

        private void OnCheckMessagesCommandExecute()
        {
            var referenceDate = Messages.Count > 0 ? Messages.Max(x => x.DateReceived) : DateTime.Now;

            Messages = new ObservableCollection<MessageModel>(new UpdateMessagesListTask().UpdateMessageList(Messages.ToList()));

            MessageModel message = Messages.FirstOrDefault(m => m.NewEmail && m.DateReceived > referenceDate);
            
            if (message != null)
            {
                mediator.SendMessage<MessageModel>(message, "New Message");
            }
        }

        protected override void OnViewModelCommandExecuted(IViewModel viewModel, ICatelCommand command, object commandParameter)
        {
            base.OnViewModelCommandExecuted(viewModel, command, commandParameter);

            if (viewModel != null && command.Tag != null)
            {
                switch (command.Tag.ToString())
                {
                    case "saveSettings":
                        var settingsViewModel = viewModel as SettingsWindowViewModel;
                        folderToWatch = settingsViewModel.FolderToWatch;
                        ChangeWatcherSettings();
                        CheckMessagesCommand.Execute();
                        break;
                    case "quitting":
                        var param = commandParameter as CancelEventArgs;
                        if (!param.Cancel)
                        {
                            new FinalSaveCommand().Save(Messages);
                        }
                        
                        break;
                }
                
            }
        }
    }
}
