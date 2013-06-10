using Catel.Data;
using Catel.Messaging;
using Catel.MVVM;
using Catel.MVVM.Services;
using Catel.Windows.Threading;
using EmailInBox.Models;
using EmailInBox.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EmailInBox.ViewModels
{
    [InterestedIn(typeof (SettingsWindowViewModel))]
    [InterestedIn(typeof(MainWindowViewModel))]
    public class HomeWindowViewModel : WindowViewModelBase
    {
        private readonly IPleaseWaitService pleaseWaitService;
        private readonly IFolderWatcher folderWatcher;
        private readonly ITryFindParent tryFindParent;
        private readonly IOpenEmailFile openEmailFile;
        private readonly IUpdateMessagesListTask updateMessagesListTask;
        private readonly IMessageMediator mediator;
        private string folderToWatch ;

        public override string Title { get { return "Home"; } }
        public Command<MouseButtonEventArgs> RowDoubleClick { get; private set; }
        public Command<MessageModel> ImageSingleClick { get; private set; }
        public Command<MessageModel> MarkAsReadCommand { get; private set; }
        public AsynchronousCommand CheckMessagesCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeWindowViewModel"/> class.
        /// </summary>
        public HomeWindowViewModel(IUpdateMessagesListTask updateMessagesListTask,IMessageMediator mediator, IPleaseWaitService pleaseWaitService, IFolderWatcher folderWatcher, ITryFindParent tryFindParent, IOpenEmailFile openEmailFile)
        {
            this.updateMessagesListTask = updateMessagesListTask;
            this.mediator = mediator;
            this.pleaseWaitService = pleaseWaitService;
            this.folderWatcher = folderWatcher;
            this.tryFindParent = tryFindParent;
            this.openEmailFile = openEmailFile;
            RowDoubleClick = new Command<MouseButtonEventArgs>(OnRowDoubleClickExecute, OnRowDoubleClickCanExecute);
            CheckMessagesCommand = new AsynchronousCommand(OnCheckMessagesCommandExecute, () => !CheckMessagesCommand.IsExecuting);
            MarkAsReadCommand = new Command<MessageModel>(OnMarkAsReadCommandExecute);
            ImageSingleClick = new Command<MessageModel>(OnImageSingleClickExecute);
            Messages = new InitialLoadCommand().Load();
            CheckMessagesWithWaiting();
        }
        
        public ObservableCollection<MessageModel> Messages
        {
            get { return GetValue<ObservableCollection<MessageModel>>(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        public static readonly PropertyData MessagesProperty = RegisterProperty("Messages", typeof(ObservableCollection<MessageModel>), null);

        #region Command Implementation

        private bool OnRowDoubleClickCanExecute(MouseButtonEventArgs e)
        {
            return true;
        }

        private void OnRowDoubleClickExecute(MouseButtonEventArgs e)
        {
            ReadAndOpenMessage(e.OriginalSource as DependencyObject, e.Source as ListView);
        }

        private void OnImageSingleClickExecute(MessageModel message)
        {
            MarkedAsRead(message);
            openEmailFile.Execute(message.Path);
        }

        private void OnMarkAsReadCommandExecute(MessageModel e)
        {
            MarkedAsRead(e);
        }

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
                        folderWatcher.ChangeWatcherFolder(folderToWatch);
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


        #endregion

        #region private method
        [MessageRecipient(Tag = "balloonClicked")]
        public void MarkedAsRead(MessageModel selectedMessage)
        {
            DispatcherHelper.CurrentDispatcher.Invoke((Action)(() =>
            {
                Messages.First(m => m == selectedMessage).NewEmail = false;
                //there must be a way to notify the change to the list without having to recreate it
                Messages = new ObservableCollection<MessageModel>(Messages);
            }));
        }

        private void CheckMessagesWithWaiting()
        {
            DispatcherHelper.CurrentDispatcher.Invoke((Action) (
                                                                   () => pleaseWaitService.Show(CheckMessage) ));
        }

        private void CheckMessage()
        {
            CheckMessage("");
        }

        [MessageRecipient(Tag = "folderModified")]
        private void CheckMessage(string updateString)
        {
            var referenceDate = Messages.Count > 0 ? Messages.Max(x => x.DateReceived) : DateTime.Now;

            Messages = new ObservableCollection<MessageModel>( updateMessagesListTask
                                                                  .UpdateMessageList(
                                                                      Messages.ToList()));

            var message = Messages.FirstOrDefault(m => m.NewEmail && m.DateReceived >= referenceDate);

            if (message != null)
            {
                mediator.SendMessage(message, "newMessage");
            }
        }

        public void ReadAndOpenMessage(DependencyObject originalSource, ListView source)
        {
            if (tryFindParent.Search<GridViewColumnHeader>(originalSource) == null)
            {
                if (source.SelectedItem == null) return;
                var selectedMessage = source.SelectedItem as MessageModel;
                MarkedAsRead(selectedMessage);
                openEmailFile.Execute(selectedMessage.Path);
            }
        }

        #endregion
    }
}
