
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using EmailInBox.Models;
using System.Linq;

namespace EmailInBox.Task.Emails
{
    public static class EmailListToFileTask
    {
        public static void SaveEmailListToFile(IEnumerable<MessageModel> messages )
        {
            var xml = new XElement("Messages",
                                   messages.Select(
                                       x =>
                                       new XElement("message", new XElement("daterecieved", x.DateReceived.ToString()),
                                                    new XElement("from", x.From), new XElement("to", x.To),
                                                    new XElement("subject", x.Subject),
                                                    new XElement("path", x.Path),
                                                    new XElement("newemail", x.NewEmail.ToString()))));

            xml.Save("messages.xml",SaveOptions.None);
        }

        public static ObservableCollection<MessageModel> LoadEmailListFromFile()
        {
            try
            {

           
            DateTime timeTemp; 
            var xml = XElement.Load("messages.xml", LoadOptions.None);
            return new ObservableCollection<MessageModel>(xml.Descendants("message")
                                                             .Select(m => new MessageModel
                                                                              {
                                                                                  From =
                                                                                      m.Element(
                                                                                          "from").Value,
                                                                                  To =
                                                                                      m.Element(
                                                                                          "to").Value,
                                                                                  Path =
                                                                                      m.Element(
                                                                                          "path").Value,
                                                                                  DateReceived =
                                                                                      DateTime.
                                                                                          TryParse
                                                                                          (m.
                                                                                               Element
                                                                                               ("daterecieved").Value
                                                                                            ,
                                                                                           out
                                                                                               timeTemp)
                                                                                          ? timeTemp
                                                                                          : DateTime
                                                                                                .
                                                                                                MinValue,
                                                                                  NewEmail =
                                                                                      Convert.ToBoolean(m.Element("newemail").Value),
                                                                                  Subject =
                                                                                      m.Element(
                                                                                          "subject").Value
                                                                              }));
            }
            catch (Exception exception)
            {

                return new ObservableCollection<MessageModel>();
            }
        }
    }
}
