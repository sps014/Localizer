using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Events
{
    internal class LoadSnapshotEvent
    {
        public string Path { get; set; }
        public LoadSnapshotEvent(string path)
        {
            Path = path;
        }
    }
}
