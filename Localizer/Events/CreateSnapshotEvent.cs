using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Events
{
    internal class CreateSnapshotEvent
    {
        public string Path { get; }
        public CreateSnapshotEvent(string path)
        {
            Path = path;
        }
    }
}
