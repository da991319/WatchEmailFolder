
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Windows;
using Catel.Data;
using Catel.Windows.Interactivity;
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
            notifyService = new NotifyService(new RoutedEventHandler(SetBallonClickWrapper), IconLeftClickCommand);
            ClickBalloonCommand = new Command<RoutedEventArgs>(OnClickBalloonCommandExecute, OnClickBalloonCommandCanExecute);
            HiddenAppCommand = new Command<CancelEventArgs>(OnHiddenAppCommandExecute);
            
        }

        #endregion

        #region Properties

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public Visibility Visibility
        {
            get { return GetValue<Visibility>(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        /// <summary>
        /// Register the Visibility property so it is known in the class.
        /// </summary>
        public static readonly PropertyData VisibilityProperty = RegisterProperty("Visibility", typeof(Visibility), null);
        
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public WindowViewModelBase CurrentViewModel
        {
            get { return GetValue<WindowViewModelBase>(CurrentViewModelProperty); }
            set { SetValue(CurrentViewModelProperty, value); }
        }

        /// <summary>
        /// Register the name property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CurrentViewModelProperty = RegisterProperty("Messages", typeof(WindowViewModelBase), null);
        
        #endregion

        #region Commands

        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
        
        /// <summary>
        /// Gets the name command.
        /// </summary>
        public Command<CancelEventArgs> HiddenAppCommand { get; private set; }

        // TODO: Move code below to constructor
        
        // TODO: Move code above to constructor

        /// <summary>
        /// Method to invoke when the name command is executed.
        /// </summary>
        private void OnHiddenAppCommandExecute(CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }
        
        /// <summary>
        /// Gets the name command.
        /// </summary>
        public Command IconLeftClickCommand { get; private set; }

        // TODO: Move code below to constructor
        // TODO: Move code above to constructor

        /// <summary>
        /// Method to invoke when the name command is executed.
        /// </summary>
        private void OnIconLeftClickCommandExecute()
        {
            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Gets the name command.
        /// </summary>
        public Command<RoutedEventArgs> ClickBalloonCommand { get; private set; }

        // TODO: Move code below to constructor
        
        // TODO: Move code above to constructor

        /// <summary>
        /// Method to check whether the name command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool OnClickBalloonCommandCanExecute(RoutedEventArgs e)
        {
            return true;
        }

        /// <summary>
        /// Method to invoke when the name command is executed.
        /// </summary>
        private void OnClickBalloonCommandExecute(RoutedEventArgs args)
        {
            Process.Start(newMessage.Path);

        }
        #endregion

        #region Methods

        /// <summary>
        /// Called when a command for a view model type that the current view model is interested in has been executed. This can
        /// be accomplished by decorating the view model with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="command">The command that has been executed.</param>
        /// <param name="commandParameter">The command parameter used during the execution.</param>
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
        #endregion
    }
}
