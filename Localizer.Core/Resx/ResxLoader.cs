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
        public static async Task<ResxManager> LoadAsync(string path,CancellationTokenSource? cts=default)
        {

            var resxManager = new ResxManager(path);
            await resxManager.BuildCollectionAsync();
            return resxManager;
        }
    }
}
