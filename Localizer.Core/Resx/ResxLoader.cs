using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localizer.Core.Model;

namespace Localizer.Core.Resx
{
    public static class ResxLoader
    {
        public static ResxLoadDataTree Load(string path)
        {
            var tree = new ResxLoadDataTree();
            tree.BuildTree(path);
            return tree;
        }
    }
}
