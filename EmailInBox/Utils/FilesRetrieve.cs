
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmailInBox.Models;
using EmailInBox.Properties;

namespace EmailInBox.Utils
{
    public static class FilesRetrieve
    {
        public static List<MessageModel> RetrieveEmail(string path, int numberOfFile)
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
    }
}
