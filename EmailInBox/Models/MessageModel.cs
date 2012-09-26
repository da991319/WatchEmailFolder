using System;
using System.Net.Mail;

namespace EmailInBox.Models
{
    public class MessageModel
    {
        public DateTime DateReceived { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Path { get; set; }
        public bool NewEmail { get; set; }
    }
}
