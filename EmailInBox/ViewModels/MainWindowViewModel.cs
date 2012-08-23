
using Catel.Data;

namespace EmailInBox.ViewModels
{
    using Catel.MVVM;

    /// <summary>
    /// MainWindow view model.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private static HomeWindowViewModel homeViewModel = new HomeWindowViewModel();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
            : base()
        {
            CurrentViewModel = homeViewModel;
        }
        #endregion

        #region Properties

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public ViewModelBase CurrentViewModel
        {
            get { return GetValue<ViewModelBase>(CurrentViewModelProperty); }
            set { SetValue(CurrentViewModelProperty, value); }
        }

        /// <summary>
        /// Register the name property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CurrentViewModelProperty = RegisterProperty("Messages", typeof(ViewModelBase), null);

        #endregion

        #region Commands
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
        #endregion

        #region Methods
        // TODO: Create your methods here
        #endregion
    }
}
