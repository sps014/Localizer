﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Localizer.ViewModels;

namespace Localizer.Events
{
    internal class DataGridSelectionChangeEvent
    {
        public int Index { get; }
        public int ColumnSelection { get; set; }

        public ResxEntityViewModel? Item { get; set; }

        public DataGridSelectionChangeEvent(int index)
        {
            Index = index;
        }
    }
}
