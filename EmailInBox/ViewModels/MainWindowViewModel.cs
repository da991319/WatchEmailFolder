
using Catel.Data;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using EmailInBox.Task.Emails;

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
        public bool trueExit;

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
            QuitMenuItemClickCommand = new Command(OnQuitMenuItemClickCommandExecute);
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

        public Command QuitMenuItemClickCommand { get; private set; }

        private void OnQuitMenuItemClickCommandExecute()
        {
            trueExit = true;
            EmailListToFileTask.SaveEmailListToFile(homeViewModel.Messages);
            Application.Current.MainWindow.Close();
        }
        
        
        #endregion
        #region Methods

        protected override void OnViewModelCommandExecuted(IViewModel viewModel, ICatelCommand command, object commandParameter)
        {
            
        }
        
        private void ExitAppWrapper(object sender, EventArgs e)
        {
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
