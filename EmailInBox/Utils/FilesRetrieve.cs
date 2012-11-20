
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmailInBox.Models;
using EmailInBox.Properties;

namespace EmailInBox.Utils
{
    public static class FilesRetrieve
    {
        public static List<MessageModel> RetrieveEmails(string path, int numberOfFile)
        {
            var files = Directory.GetFiles(path, "*.eml");

            return (from file in files
                                let message = new MimeReader().GetEmail(file)
                                select new MessageModel
                                           {
                                               DateReceived = message.DeliveryDate,
                                               From = message.From.Address,
                                               To = message.To.ToString(),
                                               Path = file,
                                               Subject = message.Subject
                                           }).OrderByDescending(f => f.DateReceived).Take(numberOfFile).ToList();


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
