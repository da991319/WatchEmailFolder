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

        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
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
        
        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
        /// <summary>
        /// Gets the SaveSettingsCommand command.
        /// </summary>
        
        public Command SaveSettingsCommand { get; private set; }
        
        /// <summary>
        /// Method to invoke when the SaveSettingsCommand command is executed.
        /// </summary>
        private void OnSaveSettingsCommandExecute()
        {
            Settings.Default.FolderToWatch = FolderToWatch;
            Settings.Default.NumberOfEmails = FileNumber;
            Settings.Default.Save();
        }

        /// <summary>
        /// Gets the name command.
        /// </summary>
        public Command BrowseFolderCommand { get; private set; }

        /// <summary>
        /// Method to invoke when the name command is executed.
        /// </summary>
        private void OnBrowseFolderCommandExecute()
        {
            var folderBrowser = new FolderBrowserDialog {RootFolder = Environment.SpecialFolder.Desktop, ShowNewFolderButton = false};
            var result = folderBrowser.ShowDialog();

            if (result.Equals(DialogResult.OK))
            {
                FolderToWatch = folderBrowser.SelectedPath;
            }
        }
    }
}
