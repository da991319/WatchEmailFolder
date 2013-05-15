
using EmailInBox.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EmailInBox.Utils
{
    public static class FilesRetrieve
    {
        public static List<MessageModel> RetrieveEmails(string path, int numberOfFile)
        {
            return Directory.Exists(path)
                       ? (from file in Directory.GetFiles(path, "*.eml")
                          where File.Exists(file)
                          let message = new MimeReader().GetEmail(file)
                          select new MessageModel
                                     {
                                         DateReceived = message.DeliveryDate.ToLocalTime(),
                                         From = message.From.Address,
                                         To = message.To.ToString(),
                                         Path = file,
                                         Subject = message.Subject
                                     }).OrderByDescending(f => f.DateReceived).Take(numberOfFile).ToList()
                       : new List<MessageModel>();
        }

        public static MessageModel RetreiveEmail(string path)
        {
            var mail = new MimeReader().GetEmail(path);
            return new MessageModel
                {
                    DateReceived = mail.DeliveryDate,
                    From = mail.From.Address,
                    To = mail.To.ToString(),
                    Path = path,
                    Subject = mail.Subject
                };
        }
    }
}
