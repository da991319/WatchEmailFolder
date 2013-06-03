
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EmailInBox.Models;
using EmailInBox.Utils;
using EmailInBox.ViewModels;
using TestEmailInbox.Utility;
using System.Linq;
using NUnit.Framework;

namespace TestEmailInbox
{
    public class HomeWindowViewModelTest
    {
        public class when_marking_an_email_as_read : ContextSpec<HomeWindowViewModel>
        {
            private MessageModel message;
            
            [Test]
            public void it_should_only_have_set_one_email_as_read()
            {
                sut.Messages.Count(x => x.NewEmail).ShouldBeEqualTo(3);
            }

            protected override void UnderTheseConditions()
            {
                message = new MessageModel {NewEmail = true, From = "from", To = "to"};
                sut.Messages = new ObservableCollection<MessageModel>
                                   {
                                       new MessageModel{NewEmail = true},
                                       new MessageModel{NewEmail = true},
                                       new MessageModel{NewEmail = true},
                                       message
                                   };
            }

            protected override void BecauseOf()
            {
                sut.MarkAsReadCommand.Execute(message);
            }
        }

        public class when_double_clicking_on_a_row : ContextSpec<HomeWindowViewModel>
        {
            private MouseButtonEventArgs param;
            private MessageModel message;
            private readonly ITryFindParent tryFindParent;

            [Test]
            public void it_should_have_called_marked_as_read()
            {
                sut.ShouldHaveBeenToldTo(x => x.MarkedAsRead(message));
            }

            protected override void UnderTheseConditions()
            {
                var mouseDevice = Mouse.PrimaryDevice;
                param = new MouseButtonEventArgs(mouseDevice,2, MouseButton.Left ) {Source = new ListView()};
                //param.Setup(x => x.Source, new ListView());

                tryFindParent.Setup(x => x.Search<GridViewColumnHeader>(param.OriginalSource as DependencyObject), null);
            }

            protected override void BecauseOf()
            {
                sut.RowDoubleClick.Execute(param);
            }
        }
    }
}
