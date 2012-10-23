using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Catel.MVVM;
using Catel.Messaging;
using EmailInBox.Models;
using EmailInBox.ViewModels;
using Hardcodet.Wpf.TaskbarNotification;

namespace EmailInBox.Views
{
    using Catel.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    
    public partial class MainWindow : DataWindow
    {
        private string lastMessagePath;
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
            : base(DataWindowMode.Custom)
        {
            InitializeComponent();
            MessageMediatorHelper.SubscribeRecipient(this); 

        }

        [MessageRecipient(Tag = "New Message")]
        public void ShowBallon(MessageModel message)
        {
            lastMessagePath = message.Path;

            var sb = new StringBuilder();

            sb.AppendFormat("From: {0}{1}", message.From, Environment.NewLine);
            sb.AppendFormat("To: {0}{1}", message.To, Environment.NewLine);
            sb.AppendFormat("Subject: {0}{1}", message.Subject, Environment.NewLine);
            sb.AppendFormat("Click here to read the email");

            TaskIcon.ShowBalloonTip("Email Received", sb.ToString(), BalloonIcon.Info);
        }

        private void TaskIcon_TrayBalloonTipClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(lastMessagePath);
        }

    }
}
