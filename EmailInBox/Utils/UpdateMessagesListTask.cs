
using System;
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
            DateTime referenceDate = DateTime.Now;

            if (initialList.Count > 0)
                referenceDate = initialList.Max(m => m.DateReceived);

            var tempList = FilesRetrieve.RetrieveEmails(folderPath, numberOfFile);

            List<MessageModel> finalList;
            if (initialList.Count > 0)
            {
                finalList = initialList.Where(m => tempList.Select(x => x.Path).Contains(m.Path)).ToList();

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
            }
            else
            {
                finalList = tempList;
            }
            return finalList.OrderByDescending(m => m.DateReceived).ToList();
        }
    }
}
