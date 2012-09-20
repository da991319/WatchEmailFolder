
using System;
using System.IO;
using Catel.Data;
using EmailInBox.Utils;

namespace EmailInBox.ViewModels
{
    using Catel.MVVM;

    /// <summary>
    /// MainWindow view model.
    /// </summary>
    /// 
    [InterestedIn(typeof(HomeWindowViewModel))]
    public class MainWindowViewModel : WindowViewModelBase
    {
        private readonly INotifyService notifyService = new NotifyService();

        #region Fields
        private static HomeWindowViewModel homeViewModel = new HomeWindowViewModel();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            CurrentViewModel = homeViewModel;
            //notifyService.ChangeIconSource("/Icons/email.ico");
        }

        #endregion

        #region Properties

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public WindowViewModelBase CurrentViewModel
        {
            get { return GetValue<WindowViewModelBase>(CurrentViewModelProperty); }
            set { SetValue(CurrentViewModelProperty, value); }
        }

        /// <summary>
        /// Register the name property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CurrentViewModelProperty = RegisterProperty("Messages", typeof(WindowViewModelBase), null);
        
        #endregion

        #region Commands
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
        #endregion

        #region Methods

        /// <summary>
        /// Called when a command for a view model type that the current view model is interested in has been executed. This can
        /// be accomplished by decorating the view model with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="command">The command that has been executed.</param>
        /// <param name="commandParameter">The command parameter used during the execution.</param>
        protected override void OnViewModelCommandExecuted(IViewModel viewModel, ICatelCommand command, object commandParameter)
        {
            int t = 2;
            notifyService.ChangeIconSource(@"/Icons/new_email.ico");
            notifyService.Notify("test");
        }
        #endregion
    }
}
