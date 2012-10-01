
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Catel.Data;
using EmailInBox.Models;
using EmailInBox.Utils;

namespace EmailInBox.ViewModels
{
    using Catel.MVVM;

    /// <summary>
    /// MainWindow view model.
    /// </summary>
    /// 
    [InterestedIn(typeof(HomeWindowViewModel))]
    public class MainWindowViewModel : WindowViewModelBase
    {
        private readonly INotifyService notifyService;
        private MessageModel newMessage;
        private bool trueExit;
        #region Fields
        private static HomeWindowViewModel homeViewModel = new HomeWindowViewModel();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            Visibility = Visibility.Visible;
            CurrentViewModel = homeViewModel;
            IconLeftClickCommand = new Command(OnIconLeftClickCommandExecute);
            //notifyService.ChangeIconSource("/Icons/email.ico");
            notifyService = new NotifyService(new RoutedEventHandler(SetBallonClickWrapper), IconLeftClickCommand, CreateContextMenu());
            ClickBalloonCommand = new Command<RoutedEventArgs>(OnClickBalloonCommandExecute, OnClickBalloonCommandCanExecute);
            HiddenAppCommand = new Command<CancelEventArgs>(OnHiddenAppCommandExecute);
        }

        #endregion

        #region Properties

        public Visibility Visibility
        {
            get { return GetValue<Visibility>(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        public static readonly PropertyData VisibilityProperty = RegisterProperty("Visibility", typeof(Visibility), null);
        
        public WindowViewModelBase CurrentViewModel
        {
            get { return GetValue<WindowViewModelBase>(CurrentViewModelProperty); }
            set { SetValue(CurrentViewModelProperty, value); }
        }

        public static readonly PropertyData CurrentViewModelProperty = RegisterProperty("Messages", typeof(WindowViewModelBase), null);
        
        #endregion

        #region Commands

        public Command<CancelEventArgs> HiddenAppCommand { get; private set; }

        private void OnHiddenAppCommandExecute(EventArgs e)
        {
            if (!trueExit)
            {
                (e as CancelEventArgs).Cancel = true;
                Visibility = Visibility.Hidden;
            }
        }
        
        public Command IconLeftClickCommand { get; private set; }

        private void OnIconLeftClickCommandExecute()
        {
            Visibility = Visibility.Visible;
        }

        public Command<RoutedEventArgs> ClickBalloonCommand { get; private set; }
        
        private bool OnClickBalloonCommandCanExecute(RoutedEventArgs e)
        {
            return true;
        }

        private void OnClickBalloonCommandExecute(RoutedEventArgs args)
        {
            Process.Start(newMessage.Path);
        }
        #endregion
        #region Methods

        protected override void OnViewModelCommandExecuted(IViewModel viewModel, ICatelCommand command, object commandParameter)
        {
            if (command.Tag.Equals("FileCreatedCommand"))
            {
                newMessage =
                    Utils.FilesRetrieve.RetreiveEmail((commandParameter as FileSystemEventArgs).FullPath);
                notifyService.ChangeIconSource(@"/Icons/new_email.ico");
                notifyService.NotifyNewMessage(newMessage);
            }
        }

        private void SetBallonClickWrapper(object sender, RoutedEventArgs e)
        {
            ClickBalloonCommand.Execute(e);
        }

        private void ExitAppWrapper(object sender, EventArgs e)
        {
            int t = 2;
            trueExit = true;
            Application.Current.MainWindow.Close();
        }

        private ContextMenu CreateContextMenu()
        {
            var tempMenu = new ContextMenu();
            var menuItemTemp = new MenuItem { Header = "Quit" };
            menuItemTemp.Click += ExitAppWrapper;

            tempMenu.Items.Add(menuItemTemp);

            return tempMenu;
        }
        #endregion
    }
}
