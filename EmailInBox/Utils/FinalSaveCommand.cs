
using System.Collections.Generic;
using EmailInBox.Models;
using EmailInBox.Task.Emails;

namespace EmailInBox.Utils
{
    public interface IFinalSaveCommand
    {
        void Save(IEnumerable<MessageModel> messages );
    }

    public class FinalSaveCommand : IFinalSaveCommand
    {
        public void Save(IEnumerable<MessageModel> messages )
        {
            EmailListToFileTask.SaveEmailListToFile(messages);
        }
    }
}
