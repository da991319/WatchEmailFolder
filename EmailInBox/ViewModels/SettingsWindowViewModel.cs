using System;
using System.Windows.Forms;
using Catel.Data;
using Catel.MVVM.Services;
using EmailInBox.Properties;

namespace EmailInBox.ViewModels
{
    using Catel.MVVM;

    /// <summary>
    /// UserControl view model.
    /// </summary>
    public class SettingsWindowViewModel : WindowViewModelBase
    {
        public Command BrowseFolderCommand { get; private set; }
        public Command SaveSettingsCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindowViewModel"/> class.
        /// </summary>
        public SettingsWindowViewModel()
        {
            FolderToWatch = Settings.Default.FolderToWatch;
            FileNumber = Settings.Default.NumberOfEmails;
            SaveSettingsCommand = new Command(OnSaveSettingsCommandExecute,null, "saveSettings");
            BrowseFolderCommand = new Command(OnBrowseFolderCommandExecute);
        }


        public override string Title { get { return "Settings"; } }

        public string FolderToWatch
        {
            get { return GetValue<string>(FolderToWatchProperty); }
            set { SetValue(FolderToWatchProperty, value); }
        }

        public static readonly PropertyData FolderToWatchProperty = RegisterProperty("FolderToWatch", typeof(string), null);

        public int FileNumber
        {
            get { return GetValue<int>(FileNumberProperty); }
            set { SetValue(FileNumberProperty, value); }
        }

        public static readonly PropertyData FileNumberProperty = RegisterProperty("FileNumber", typeof(int), null);

        #region implement command
        private void OnSaveSettingsCommandExecute()
        {
            Settings.Default.FolderToWatch = FolderToWatch;
            Settings.Default.NumberOfEmails = FileNumber;
            Settings.Default.Save();
        }

        private void OnBrowseFolderCommandExecute()
        {
            var folderBrowser = new FolderBrowserDialog {RootFolder = Environment.SpecialFolder.Desktop, ShowNewFolderButton = false};
            var result = folderBrowser.ShowDialog();

            if (result.Equals(DialogResult.OK))
            {
                FolderToWatch = folderBrowser.SelectedPath;
            }
        }

        #endregion
    }
}
