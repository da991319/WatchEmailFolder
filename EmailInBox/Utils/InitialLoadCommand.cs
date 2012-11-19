using EmailInBox.Models;
using System.Collections.ObjectModel;
using EmailInBox.Task.Emails;

namespace EmailInBox.Utils
{
    public interface IInitialLoadCommand
    {
        ObservableCollection<MessageModel> Load();
    }

    public class InitialLoadCommand : IInitialLoadCommand
    {
        public ObservableCollection<MessageModel> Load()
        {
            return EmailListToFileTask.LoadEmailListFromFile();
        }
    }
}
