using System;
using System.Drawing;
using System.IO;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace EmailInBox.Utils
{
    public interface INotifyService
    {
        void Notify(string message);
        void ChangeIconSource(string path);
    }

    public class NotifyService : INotifyService
    {
        private TaskbarIcon icon = new TaskbarIcon
            {
                Name = "NotifyIcon",
                Icon =
                    new System.Drawing.Icon(
                        Application.GetResourceStream(Utils.FileUtils.MakeUri("/Icons/email.ico")).Stream),
            };


        public void Notify(string message)
        {
            
            icon.ShowBalloonTip("title", message, BalloonIcon.None);
        }

        public void ChangeIconSource(string path)
        {
            icon.Icon = new System.Drawing.Icon(
                        Application.GetResourceStream(Utils.FileUtils.MakeUri(path)).Stream);
        }
    }
}
