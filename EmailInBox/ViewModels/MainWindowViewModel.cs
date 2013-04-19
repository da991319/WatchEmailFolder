
using System.Collections.ObjectModel;
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
    [InterestedIn(typeof(SettingsWindowViewModel))]
    public class MainWindowViewModel : WindowViewModelBase
    {
        public bool trueExit;

        #region Fields
        private HomeWindowViewModel homeViewModel = new HomeWindowViewModel();
        private SettingsWindowViewModel settingsViewModel = new SettingsWindowViewModel();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            Visibility = Visibility.Visible;
            //CurrentViewModel = homeViewModel;
            IconLeftClickCommand = new Command(OnIconLeftClickCommandExecute);
            QuitMenuItemClickCommand = new Command(OnQuitMenuItemClickCommandExecute);
            HiddenAppCommand = new Command<CancelEventArgs>(OnHiddenAppCommandExecute);
            //Tabs = new ObservableCollection<WindowViewModelBase>{homeViewModel,settingsViewModel};
            //FolderToWatch = settingsViewModel.FolderToWatch;
            //FileNumber = settingsViewModel.FileNumber;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public ObservableCollection<WindowViewModelBase> Tabs
        {
            get { return GetValue<ObservableCollection<WindowViewModelBase>>(TabsProperty); }
            set { SetValue(TabsProperty, value); }
        }

        /// <summary>
        /// Register the name property so it is known in the class.
        /// </summary>
        public static readonly PropertyData TabsProperty = RegisterProperty("Tabs", typeof(ObservableCollection<WindowViewModelBase>), null);

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

        public static readonly PropertyData CurrentViewModelProperty = RegisterProperty("CurrentViewModel", typeof(WindowViewModelBase), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public string FolderToWatch
        {
            get { return GetValue<string>(FolderToWatchProperty); }
            set { SetValue(FolderToWatchProperty, value); }
        }

        /// <summary>
        /// Register the FolderPath property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FolderToWatchProperty = RegisterProperty("FolderToWatch", typeof(string), null);


        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public int FileNumber
        {
            get { return GetValue<int>(FileNumberProperty); }
            set { SetValue(FileNumberProperty, value); }
        }

        /// <summary>
        /// Register the NumberOfFiles property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FileNumberProperty = RegisterProperty("FileNumber", typeof(int), null);
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
            if (command.Tag.Equals("saveSettings"))
            {
                FolderToWatch = settingsViewModel.FolderToWatch;
                FileNumber = settingsViewModel.FileNumber;
            }


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
