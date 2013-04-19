namespace EmailInBox.Views
{
    using Catel.Windows.Controls;

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml.
    /// </summary>
    public partial class SettingsWindow : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindow"/> class.
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
            CloseViewModelOnUnloaded = false;
        }
    }
}
