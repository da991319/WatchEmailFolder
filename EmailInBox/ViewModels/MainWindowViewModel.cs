
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

        public Command<CancelEventArgs> HiddenAppCommand { get; private set; }
        public Command IconLeftClickCommand { get; private set; }
        public Command QuitMenuItemClickCommand { get; private set; }

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

        public Visibility Visibility
        {
            get { return GetValue<Visibility>(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        public static readonly PropertyData VisibilityProperty = RegisterProperty("Visibility", typeof(Visibility), null);

        public string AppVersion
        {
            get { return GetValue<string>(AppVersionProperty); }
            set { SetValue(AppVersionProperty, value); }
        }

        public static readonly PropertyData AppVersionProperty = RegisterProperty("AppVersion", typeof(string), null);

        public int SelectedIndexTab
        {
            get { return GetValue<int>(SelectedIndexTabProperty); }
            set { SetValue(SelectedIndexTabProperty, value); }
        }

        public static readonly PropertyData SelectedIndexTabProperty = RegisterProperty("SelectedIndexTab", typeof(int), null);

        #region implement command

        private void OnHiddenAppCommandExecute(EventArgs e)
        {
            if (!trueExit)
            {
                (e as CancelEventArgs).Cancel = true;
                Visibility = Visibility.Hidden;
            }
        }
        
        private void OnIconLeftClickCommandExecute()
        {
            Visibility = Visibility.Visible;
        }

        private void OnQuitMenuItemClickCommandExecute()
        {
            trueExit = true;
            Application.Current.MainWindow.Close();
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

        #region private method
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
        #endregion
    }
}
