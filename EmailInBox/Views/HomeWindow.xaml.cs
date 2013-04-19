namespace EmailInBox.Views
{
    using Catel.Windows.Controls;

    /// <summary>
    /// Interaction logic for HomeWindow.xaml.
    /// </summary>
    public partial class HomeWindow : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeWindow"/> class.
        /// </summary>
        public HomeWindow()
        {
            InitializeComponent();
            CloseViewModelOnUnloaded = false;
        }
    }
}
