using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Test
{
    internal static class Extensions
    {
        public static string JoinPath(this string path,string path2)
        {
            return Path.Join(path, path2);
        }
    }
}
