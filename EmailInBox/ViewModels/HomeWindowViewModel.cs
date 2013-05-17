using Catel.Data;
using Catel.Messaging;
using Catel.MVVM;
using Catel.MVVM.Services;
using Catel.Windows.Threading;
using EmailInBox.Models;
using EmailInBox.Properties;
using EmailInBox.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private readonly IPleaseWaitService pleaseWaitService;
        private readonly IUpdateMessagesListTask updateMessagesListTask;
        private FileSystemWatcher watcher;
        private IMessageMediator mediator = MessageMediator.Default;
        private string folderToWatch ;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeWindowViewModel"/> class.
        /// </summary>
        public HomeWindowViewModel(IUpdateMessagesListTask updateMessagesListTask, IPleaseWaitService pleaseWaitService)
        {
            this.updateMessagesListTask = updateMessagesListTask;
            this.pleaseWaitService = pleaseWaitService;
            InitializeWatcher();
            RowDoubleClick = new Command<MouseButtonEventArgs>(OnRowDoubleClickExecute, OnRowDoubleClickCanExecute);
            OnFileCreatedCmd = new Command<FileSystemEventArgs>(OnFileCreatedCmdExecute, OnFileCreatedCmdCanExecute, "FileCreatedCommand");
            CheckMessagesCommand = new AsynchronousCommand(OnCheckMessagesCommandExecute, () => !CheckMessagesCommand.IsExecuting);
            MarkAsReadCommand = new Command<MessageModel>(OnMarkAsReadCommandExecute);
            Messages = new InitialLoadCommand().Load();
            CheckMessagesWithWaiting();
        }

        public override string Title { get { return "Home"; } }

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

        private void ChangeWatcherSettings()
        {
            if (watcher != null)
                watcher.Path = folderToWatch;
            else
                InitializeWatcher();
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            CheckMessage(); 
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
            var selectedMessage = source.SelectedItem as MessageModel;
            MarkedAsRead(selectedMessage);
            Process.Start(selectedMessage.Path);
        }

        [MessageRecipient(Tag = "Balloon Clicked")]
        private void MarkedAsRead(MessageModel selectedMessage)
        {
            DispatcherHelper.CurrentDispatcher.Invoke((Action)(() =>
                                                                   {
                                                                       Messages.First(m => m == selectedMessage).NewEmail = false;
                                                                       //there must be a way to notify the change to the list without having to recreate it
                                                                       Messages = new ObservableCollection<MessageModel>(Messages);
                                                                   }));
        }

        public Command<FileSystemEventArgs> OnFileCreatedCmd { get; private set; }

        private void OnFileCreatedCmdExecute(FileSystemEventArgs e)
        {
            CheckMessage();
        }

        private bool OnFileCreatedCmdCanExecute(FileSystemEventArgs e)
        {
            return true;
        }

        /// <summary>
        /// Gets the name command.
        /// </summary>
        public Command<MessageModel> MarkAsReadCommand { get; private set; }

        /// <summary>
        /// Method to invoke when the name command is executed.
        /// </summary>
        private void OnMarkAsReadCommandExecute(MessageModel e)
        {
            MarkedAsRead(e);
        }

        public AsynchronousCommand CheckMessagesCommand { get; private set; }

        private void OnCheckMessagesCommandExecute()
        {
            CheckMessagesWithWaiting();
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
                        CheckMessagesWithWaiting();
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

        private void CheckMessagesWithWaiting()
        {
            DispatcherHelper.CurrentDispatcher.Invoke((Action) (
                                                                   () => pleaseWaitService.Show(CheckMessage) ));
        }

        private void CheckMessage()
        {
            var referenceDate = Messages.Count > 0 ? Messages.Max(x => x.DateReceived) : DateTime.Now;

            Messages = new ObservableCollection<MessageModel>( updateMessagesListTask
                                                                  .UpdateMessageList(
                                                                      Messages.ToList()));

            var message = Messages.FirstOrDefault(m => m.NewEmail && m.DateReceived >= referenceDate);

            if (message != null)
            {
                mediator.SendMessage(message, "New Message");
            }
        }
    }
}
