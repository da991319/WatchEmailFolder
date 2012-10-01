using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EmailInBox.Models;
using Hardcodet.Wpf.TaskbarNotification;

namespace EmailInBox.Utils
{
    public interface INotifyService
    {
        void NotifyNewMessage(MessageModel message);
        void ChangeIconSource(string path);
    }

    public class NotifyService : INotifyService
    {
        public NotifyService(RoutedEventHandler clickBalloonEvent, ICommand leftClickCommand, ContextMenu menu)
        {
            icon.TrayBalloonTipClicked += clickBalloonEvent;
            icon.LeftClickCommand = leftClickCommand;
            icon.ContextMenu = menu;
        }

        private TaskbarIcon icon = new TaskbarIcon
            {
                Name = "NotifyIcon",
                Icon =
                    new System.Drawing.Icon(
                        Application.GetResourceStream(Utils.FileUtils.MakeUri("/Icons/email.ico")).Stream),
                
            };

        public void NotifyNewMessage(MessageModel message)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("From: {0}{1}", message.From, Environment.NewLine);
            sb.AppendFormat("To: {0}{1}", message.To, Environment.NewLine);
            sb.AppendFormat("Subject: {0}{1}", message.Subject, Environment.NewLine);
            sb.AppendFormat("Click here to read the email");

            icon.ShowBalloonTip("Email Received", sb.ToString(), BalloonIcon.Info);
        }

        

        public void ChangeIconSource(string path)
        {
            icon.Icon = new System.Drawing.Icon(
                        Application.GetResourceStream(Utils.FileUtils.MakeUri(path)).Stream);
        }
    }
}
