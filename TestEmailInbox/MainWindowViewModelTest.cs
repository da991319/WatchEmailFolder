using System.ComponentModel;
using System.Windows;
using EmailInBox.ViewModels;
using NUnit.Framework;
using TestEmailInbox.Utility;

namespace TestEmailInbox
{
    
    
    /// <summary>
    ///This is a test class for MainWindowViewModelTest and is intended
    ///to contain all MainWindowViewModelTest Unit Tests
    ///</summary>
     
    public class MainWindowViewModelTest
    {
        public class when_creating_the_main_viewmodel : ContextSpec<MainWindowViewModel>
        {
            [Test]
            public void it_should_be_visible()
            {
                sut.Visibility.ShouldBeEqualTo(Visibility.Visible);
            }

            [Test]
            public void it_should_have_home_viewmodel_as_current_viewmodel()
            {
                //sut.CurrentViewModel.ShouldBeInstanceOf(typeof(HomeWindowViewModel));
            }

            protected override void UnderTheseConditions()
            {
                
            }

            protected override void BecauseOf()
            {
                
            }
        }
        
        public class when_left_clicking_command_is_triggered : ContextSpec<MainWindowViewModel>
        {
            [Test]
            public void it_should_have_set_the_visiblity_to_visible()
            {
                sut.Visibility.ShouldBeEqualTo(Visibility.Visible);
            }

            protected override void UnderTheseConditions()
            {
                sut.Visibility = Visibility.Hidden;
            }

            protected override void BecauseOf()
            {
                sut.IconLeftClickCommand.Execute();
            }
        }

        public class when_hidden_app_is_triggered_and_it_is_just_hide_gui : ContextSpec<MainWindowViewModel>
        {
            private CancelEventArgs e;

            [Test]
            public void it_should_have_cancel_the_exit_action()
            {
                e.Cancel.ShouldBeTrue();
            }

            [Test]
            public void it_should_have_set_the_visibility_to_hidden()
            {
                sut.Visibility.ShouldBeEqualTo(Visibility.Hidden);
            }

            protected override void UnderTheseConditions()
            {
                e = new CancelEventArgs();
                sut.trueExit = false;
            }

            protected override void BecauseOf()
            {
                sut.HiddenAppCommand.Execute(e);
            }
        }

        //public class when_quit_menu_item_click_is_triggered : ContextSpec<MainWindowViewModel>
        //{
        //    private Application application;

        //    [Test]
        //    public void it_should_have_set_true_exit_to_true()
        //    {
        //        sut.trueExit.ShouldBeTrue();
        //    }

        //    [Test]
        //    public void it_should_have_close_the_application()
        //    {
        //        application.ShouldHaveBeenToldTo(x => x.MainWindow.Close());
        //    }

        //    protected override void UnderTheseConditions()
        //    {
        //        application = MakeMock<Application>();

        //        application.Setup(x => x.MainWindow, new Window());
        //    }

        //    protected override void BecauseOf()
        //    {
        //        sut.QuitMenuItemClickCommand.Execute();
        //    }
        //}
    }
}
