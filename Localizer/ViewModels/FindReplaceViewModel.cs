﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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


        [ObservableProperty]
        private bool isReplaceMode = false;

        //SearchCommentsColumn

        private static bool searchCommentsColumn = true;
        public bool SearchCommentsColumn
        {
            get { return searchCommentsColumn; }
            set
            {
                searchCommentsColumn = value;
                ResetIndex();
            }
        }


        private static bool IsRegexMode = false;

        public bool IsRegex
        {
            get
            {
                return IsRegexMode;
            }
            set
            {
                SetProperty(ref IsRegexMode, value);
                ResetIndex();
            }
        }
        private static bool IsIgnoreCase = true;

        public bool IgnoreCase
        {
            get
            {
                return IsIgnoreCase;
            }
            set
            {
                SetProperty(ref IsIgnoreCase, value);
                ResetIndex();
            }
        }

        public int WinHeight => IsReplaceMode ? 150 : 100;

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
                ResetIndex();
            }
        }

        public ObservableCollection<string> ColumnNames
        {
            get
            {
                var col = new ObservableCollection<string>
                {
                    "All Columns",
                    "Key",
                    "Neutral"
                };

                foreach (var culture in ResxManager.Cultures.OrderBy(x=>x))
                {
                    if (culture == string.Empty)
                        continue;

                    col.Add(culture);
                }
                return col;
            }
        }

        private ObservableCollection<ResxEntityViewModel>? DataInGrid
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
            ResxManager = MainWindowViewModel.Instance!.ResxManager;
        }
        private int subIndex =-1 ;

        public void ResetIndex()
        {
            subIndex = -1;
        }

        [RelayCommand]
        private void Find()
        {

            var result = SearchColumn(Selection);

            if (result.item == null)
                return;

            EventBus.Instance.Publish<DataGridSelectionChangeEvent>(new DataGridSelectionChangeEvent(result.index));

            subIndex = result.index;

        }

        (ResxEntityViewModel? item, int index) SearchColumn(int column)
        {
            var data = DataInGrid;

            if (data == null || findStr == null)
                return (null, -1);

            string columnKey = column == 2 ? ResxEntityViewModel.NeutralKeyName : ColumnNames[column];

            var start = subIndex + 1;

            for (int i = start; i < data.Count; i++)
            {
                ResxEntityViewModel? v = data[i];

                //only key
                if (column==1 && CheckKey(v))
                {
                    return (v, i);
                }

                //all option
                if (column == 0 && CheckAllOptions(v))
                {
                    return (v, i);
                }

                //specific column
                if (column > 1 && CheckValuesAndComments(v, columnKey))
                {
                    return (v, i);
                }
            }

            return (null, -1);
        }

        private bool CheckKey(ResxEntityViewModel v)
        {
            return v.Key != null && Comparer(v.Key);
        }

        private bool CheckAllOptions(ResxEntityViewModel v)
        {
            return CheckKey(v) || CheckValues(v) || CheckComments(v);
        }

        private bool CheckValues(ResxEntityViewModel v)
        {
            foreach (var c in v.CultureValues)
            {
                if (c.Value != null && Comparer(c.Value))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckComments(ResxEntityViewModel v)
        {
            if (!SearchCommentsColumn)
                return false;

            foreach (var c in v.CultureComments)
            {
                if (c.Value != null && Comparer(c.Value))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckValuesAndComments(ResxEntityViewModel v,string columnKey)
        {
            return (v.CultureValues[columnKey] != null && Comparer(v.CultureValues[columnKey]!)) ||
                   (SearchCommentsColumn && v.CultureComments.ContainsKey(columnKey) && Comparer(v.CultureComments[columnKey]!));
        }

        private bool Comparer(string a)
        {
            if(a==null)
                 a = string.Empty;

            if(IsRegex)
            {
                if(!IsIgnoreCase)
                    return Regex.IsMatch(a, FindStr!);
                return Regex.IsMatch(a, FindStr!, RegexOptions.IgnoreCase);

            }

            if (IsIgnoreCase)
                return a.Contains(FindStr!,StringComparison.OrdinalIgnoreCase);
            return a.Contains(FindStr!);
        }

    }
}
