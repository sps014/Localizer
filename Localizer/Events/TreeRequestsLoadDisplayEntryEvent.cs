using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localizer.Core.Model;

namespace Localizer.Events
{
    internal class TreeRequestsLoadDisplayEntryEvent
    {
        public ResxFileSystemNodeBase Node { get; }

        public TreeRequestsLoadDisplayEntryEvent(ResxFileSystemNodeBase node)
        {
            Node = node;
        }
    }
}
