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
        public static async Task<ResxLoadDataTree> LoadAsync(string path,CancellationTokenSource? cts=default)
        {
            var tree = new ResxLoadDataTree(path);
            await tree.BuildTreeAsync(cts);
            var resxManager = new ResxManager(tree);
            await resxManager.BuildCollectionAsync();
            return tree;
        }
        public static async Task<ResxManager> LoadAllEntities(this ResxLoadDataTree tree,CancellationTokenSource? cts=default)
        {
            var resxManager = new ResxManager(tree);
            await resxManager.BuildCollectionAsync();
            return resxManager;
        }
    }
}
