
using System.Collections.Generic;
using System.Linq;

using EmailInBox.Models;

namespace EmailInBox.Utils
{
    public interface IUpdateMessagesListTask
    {
        List<MessageModel> UpdateMessageList(List<MessageModel> initialList, string folderPath, int numberOfFile);
    }

    public class UpdateMessagesListTask : IUpdateMessagesListTask
    {
        public List<MessageModel> UpdateMessageList(List<MessageModel> initialList, string folderPath, int numberOfFile)
        {
            var referenceDate = initialList.Max(m => m.DateReceived);

            var tempList = FilesRetrieve.RetrieveEmails(folderPath, numberOfFile);

            var finalList = initialList.Where(m => tempList.Select(x => x.Path).Contains(m.Path)).ToList();

            finalList.AddRange(tempList.Where(m => !initialList.Select(x => x.Path).Contains(m.Path))
                .Select(x => new MessageModel
                                 {
                                     DateReceived = x.DateReceived,
                                     From = x.From,
                                     NewEmail = x.DateReceived > referenceDate,
                                     Path = x.Path,
                                     Subject = x.Subject,
                                     To = x.To
                                 }));
            
            return finalList.OrderByDescending(m => m.DateReceived).ToList();
        }
    }
}
