using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Events
{
    internal class ExportToExcelEvent
    {
        public string Path { get; }
        internal ExportToExcelEvent(string path)
        {
            Path = path;
        }
    }
}
