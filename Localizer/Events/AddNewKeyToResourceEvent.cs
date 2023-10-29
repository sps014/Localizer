using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localizer.Core.Model;
using Localizer.Core.Resx;

namespace Localizer.Events
{
    internal class AddNewKeyToResourceEvent
    {
        public string Key;
        public ResxFileSystemLeafNode Node;

        public AddNewKeyToResourceEvent(string key, ResxFileSystemLeafNode node)
        {
            Key = key;
            Node = node;
        }
    }
}
