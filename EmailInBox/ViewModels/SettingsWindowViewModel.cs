using Catel.Data;
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
    }
}
