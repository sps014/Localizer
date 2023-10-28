using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Localizer.Core.Resx;
using Localizer.Events;

namespace Localizer.ViewModels
{
    internal partial class FindReplaceViewModel:ObservableObject
    {
        private static string? findStr;
        public string? FindStr
        {
            get
            {
                return findStr;
            }
            set
            {
                findStr = value;
                SetProperty(ref findStr, value);
            }
        }

        [ObservableProperty]
        private string? replaceStr;

        private ResxManager ResxManager;

        public static int SelectionIndex = 0;

        public int Selection
        {
            get
            {
                return SelectionIndex;
            }
            set
            {
                SelectionIndex = value;
                SetProperty(ref SelectionIndex, value);
            }
        }

        public ObservableCollection<string> ColumnNames
        {
            get
            {
                var col = new ObservableCollection<string>();
                col.Add("All Columns");
                col.Add("Key");
                col.Add("Neutral");

                foreach(var culture in ResxManager.Cultures)
                {
                    if (culture == string.Empty)
                        continue;

                    col.Add(culture);
                }
                return col;
            }
        }

        private ObservableCollection<ResxEntityViewModel> DataInGrid
        {
            get
            {
                var referer = new RequestSourceDataEvent();
                EventBus.Instance.Publish(referer);
                return referer.DataInGrid;
            }
        }

        internal FindReplaceViewModel()
        {
            ResxManager = MainWindowViewModel.Instance.ResxManager;
        }
        private int subIndex =-1 ;

        [RelayCommand]
        private void Find()
        {
            bool searchAll = Selection == 0;

        }
    }
}
