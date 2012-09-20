using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmailInBox.Utils
{
    public static class FileUtils
    {
        public static Uri MakeUri(string path)
        {
            return new Uri(@"pack://application:,,," + path, UriKind.RelativeOrAbsolute);
        }
    }
}
