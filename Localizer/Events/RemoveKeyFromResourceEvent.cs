﻿using Localizer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.Events
{
    internal class RemoveKeyFromResourceEvent
    {
        public bool RemoveSelection { get; }
        public ResxEntityViewModel? Item { get; }
        public RemoveKeyFromResourceEvent(bool removeSelection,ResxEntityViewModel? item=null)
        {
            RemoveSelection = removeSelection;
            Item =item;
        }
    }
}
