﻿
using Catel.IoC;
using Catel.Messaging;
using EmailInBox.Utils;

namespace EmailInBox
{
    using System.Windows;

    using Catel.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            StyleHelper.CreateStyleForwardersForDefaultStyles();

            // TODO: Using a custom IoC container like Unity? Register it here:
            // Catel.IoC.ServiceLocator.Instance.RegisterExternalContainer(MyUnityContainer);
            ServiceLocator.Default.RegisterType<IUpdateMessagesListTask, UpdateMessagesListTask>(RegistrationType.Transient);
            ServiceLocator.Default.RegisterType<IFolderWatcher, FolderWatcher>(RegistrationType.Singleton);
            ServiceLocator.Default.RegisterType < ITryFindParent, TryFindParent>(RegistrationType.Transient);
            ServiceLocator.Default.RegisterType<IUpdateMessagesListTask, UpdateMessagesListTask>(RegistrationType.Transient);
            ServiceLocator.Default.RegisterType<IOpenEmailFile, OpenEmailFile>(RegistrationType.Transient);
#if DEBUG
            Catel.Logging.LogManager.RegisterDebugListener();
#endif
            base.OnStartup(e);
        }
    }
}