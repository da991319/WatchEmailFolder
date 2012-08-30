using System.IO;

namespace EmailInBox.Utils
{
    public class WatchingFolder : FileSystemWatcher
    {
        public WatchingFolder(string path, string filter) : base(path, filter)
        {
            NotifyFilter = NotifyFilters.LastAccess
                         | NotifyFilters.LastWrite
                         | NotifyFilters.FileName
                         | NotifyFilters.DirectoryName;
        }
    }
}
