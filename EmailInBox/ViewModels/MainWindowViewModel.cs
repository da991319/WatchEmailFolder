
using Catel.Data;
using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.Windows;
using System.Windows.Controls;

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

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            Visibility = Visibility.Visible;
            IconLeftClickCommand = new Command(OnIconLeftClickCommandExecute);
            QuitMenuItemClickCommand = new Command(OnQuitMenuItemClickCommandExecute);
            HiddenAppCommand = new Command<CancelEventArgs>(OnHiddenAppCommandExecute,null, "quitting");
            AppVersion = GetAppVersion();
        }

        #endregion

        #region Properties
        public Visibility Visibility
        {
            get { return GetValue<Visibility>(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        public static readonly PropertyData VisibilityProperty = RegisterProperty("Visibility", typeof(Visibility), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public string AppVersion
        {
            get { return GetValue<string>(AppVersionProperty); }
            set { SetValue(AppVersionProperty, value); }
        }

        /// <summary>
        /// Register the AppVersion property so it is known in the class.
        /// </summary>
        public static readonly PropertyData AppVersionProperty = RegisterProperty("AppVersion", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public int SelectedIndexTab
        {
            get { return GetValue<int>(SelectedIndexTabProperty); }
            set { SetValue(SelectedIndexTabProperty, value); }
        }

        /// <summary>
        /// Register the SelectedIndexTab property so it is known in the class.
        /// </summary>
        public static readonly PropertyData SelectedIndexTabProperty = RegisterProperty("SelectedIndexTab", typeof(int), null);

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
            Application.Current.MainWindow.Close();
        }

        #endregion
        #region Methods
       
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

        private string GetAppVersion()
        {
            var version = ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion : System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return String.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        protected override void OnViewModelCommandExecuted(IViewModel viewModel, ICatelCommand command,
                                                           object commandParameter)
        {
            if (command.Tag != null && command.Tag.ToString().Equals("saveSettings"))
            {
                SelectedIndexTab = 0;
            }
        }

        #endregion
    }
}
