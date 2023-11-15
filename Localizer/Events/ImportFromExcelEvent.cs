using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localizer.Core.Resx;

namespace Localizer.Events
{
    internal class ImportFromExcelEvent
    {
        public string Path { get; }
        public ResxManager ResxManager { get; }

        internal ImportFromExcelEvent(string path,ResxManager resxManager)
        {
            Path = path;
            ResxManager  = resxManager;
        }
    }
}
