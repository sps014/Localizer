using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localizer.ViewModels;

namespace Localizer.Events
{
    internal class RequestSourceDataEvent
    {
        public ObservableCollection<ResxEntityViewModel>? DataInGrid { get; set; }


    }
}
