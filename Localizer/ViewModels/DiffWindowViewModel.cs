using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Localizer.Core.Resx;
using Localizer.Events;

namespace Localizer.ViewModels
{
    internal partial class DiffWindowViewModel:ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ResxEntityViewModel>? source;
        public required DataGrid DataGrid { get; init; }

        private ObservableCollection<ResxEntityViewModel>? DataInOriginalGrid
        {
            get
            {
                var referer = new RequestSourceDataEvent();
                EventBus.Instance.Publish(referer);
                return referer.DataInGrid;
            }
        }

        public void Init()
        {
            AddColumns();
            FindChangedResources();
        }

        protected void AddColumns()
        {
            DataGrid!.Columns.Add(DataGridViewModel.CreateColumn(nameof(ResxEntityViewModel.Key)));

            foreach (var culture in MainWindowViewModel.Instance!.ResxManager.Tree.Cultures.OrderBy(x => x))
            {

                //for value col
                DataGrid.Columns.Add(DataGridViewModel.CreateColumn(DataGridViewModel.GetColumnHeaderName(culture), DataGridViewModel.GetKeyPath(culture)));

                //for comment col
                DataGrid.Columns.Add(DataGridViewModel.CreateColumn(DataGridViewModel.GetColumnHeaderName(culture, true), DataGridViewModel.GetKeyPath(culture, true)));

            }

        }

        void FindChangedResources()
        {
            var data = DataInOriginalGrid;
            if (data == null)
                return;

            Source = new ObservableCollection<ResxEntityViewModel>(data.Where(x => x.IsDiffWithSnapshot()));

        }
    }
}
