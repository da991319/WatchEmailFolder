
using System.Diagnostics;

namespace EmailInBox.Utils
{
    public interface IOpenEmailFile
    {
        void Execute(string filePath);
    }

    public class OpenEmailFile : IOpenEmailFile
    {
        public void Execute(string filePath)
        {
            Process.Start(filePath);
        }
    }
}
