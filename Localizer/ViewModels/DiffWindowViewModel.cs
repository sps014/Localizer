using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Localizer.Core.Resx;
using Localizer.Events;
using Localizer.Helper;
using Localizer.Views;
using MsBox.Avalonia;

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

        [RelayCommand]
        public async void ExportChanges()
        {
            if(Source ==null || Source.Count<=0)
            {
                var mb = MessageBoxManager.GetMessageBoxStandard("No Changes to export", "No changes were found , nothing to export.", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Info);

                await mb.ShowWindowDialogAsync(WindowHelper.ParentWindow<DiffWindow>());
                return;
            }

            var save = await TopLevel.GetTopLevel(DataGrid)!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export to Excel",
                DefaultExtension = "xlsx",
                ShowOverwritePrompt = true,
                FileTypeChoices = new List<FilePickerFileType>
            {
                new("Excel File")
                {
                    Patterns = new[]{"*.xlsx"},
                    MimeTypes = new[]{"xlsx/*"},
                },
            }

            });
            if (save == null || save.Path == null) return;

            var fileName = save.Path.LocalPath;
            await ExcelExporter.WriteToExcelAsync(Source, fileName,MainWindowViewModel.Instance!.ResxManager.Cultures);
        }
    }
}
