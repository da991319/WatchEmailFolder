
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
            private readonly IOpenEmailFile openEmailFile;
            private MessageModel message;
            private readonly ITryFindParent tryFindParent;
            private DependencyObject originalSource;
            ListView source;

            [Test]
            public void it_should_have_called_marked_as_read()
            {
                message.NewEmail.ShouldBeFalse();
            }

            [Test]
            public void it_should_have_open_the_email()
            {
                openEmailFile.ShouldHaveBeenToldTo(x => x.Execute(message.Path));
            }

            protected override void UnderTheseConditions()
            {
                originalSource = new DependencyObject();
                message = new MessageModel{NewEmail = true, Path = ""};
                tryFindParent.Setup(x => x.Search<GridViewColumnHeader>(originalSource), null);
                source = new ListView {ItemsSource = new List<MessageModel>{new MessageModel(), message}, SelectedItem = message};
                sut.Messages = new ObservableCollection<MessageModel>{new MessageModel(), message};
            }

            protected override void BecauseOf()
            {
                sut.ReadAndOpenMessage(originalSource, source);
            }
        }
    }
}
