using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localizer.ViewModels;

namespace Localizer.Events
{
    internal class DataGridCurrentSelectionChangedevent
    {
        public int Index { get; set; }
        public ResxEntityViewModel? Item { get; set; }
        public DataGridCurrentSelectionChangedevent(int index, ResxEntityViewModel? item) 
        {
            Index = index;
            Item = item;
        }
    }
}
