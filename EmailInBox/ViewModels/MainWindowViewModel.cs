
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
        public string AppVersion
        {
            get { return GetValue<string>(AppVersionProperty); }
            set { SetValue(AppVersionProperty, value); }
        }

        /// <summary>
        /// Register the AppVersion property so it is known in the class.
        /// </summary>
        public static readonly PropertyData AppVersionProperty = RegisterProperty("AppVersion", typeof(string), null);

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
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            return String.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Revision);
        }
        #endregion
    }
}
