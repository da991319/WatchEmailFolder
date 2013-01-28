using System;
using System.Net.Mail;
using Catel.Data;

namespace EmailInBox.Models
{
    public class MessageModel : ModelBase
    {
        public DateTime DateReceived { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Path { get; set; }
        //public bool NewEmail { get; set; }
        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public bool NewEmail
        {
            get { return GetValue<bool>(NewMailProperty); }
            set { SetValue(NewMailProperty, value); }
        }

        /// <summary>
        /// Register the NewMail property so it is known in the class.
        /// </summary>
        public static readonly PropertyData NewMailProperty = RegisterProperty("NewMail", typeof(bool), false);
    }
}
