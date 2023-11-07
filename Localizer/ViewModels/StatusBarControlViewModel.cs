﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Localizer.Core.Helpers;
using Localizer.Events;

namespace Localizer.ViewModels
{
    internal partial class StatusBarControlViewModel:ObservableObject
    {
        [ObservableProperty]
        public string? keyName;

        [ObservableProperty]
        public string? titleResourceName;

        [ObservableProperty]
        public string? resourceDir;

        [ObservableProperty]
        public bool isDataGridItemSelected = false;

        public StatusBarControlViewModel()
        {
            EventBus.Instance.Subscribe<DataGridCurrentSelectionChangedevent>(OnSelectionInDataGridChanged);
        }

        private void OnSelectionInDataGridChanged(DataGridCurrentSelectionChangedevent e)
        {
            //todo: think what info to show
            return;

            if (e.Item==null)
            {
                IsDataGridItemSelected = false;
                return;
            }

            IsDataGridItemSelected = true;

            if (e.Item.ResxEntity.TryGetAbsolutePath(string.Empty,out var path))
            {
                var name = path.GetFileNameWithoutExtension();
                ResourceDir = path.GetParentDirectory();
                TitleResourceName = name;
                KeyName = e.Item.Key;
            }

        }
    }
}
