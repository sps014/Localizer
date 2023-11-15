using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using Localizer.Core.Helpers;
using Localizer.ViewModels;
using MsBox.Avalonia;
using SwiftExcel;

namespace Localizer.Helper;

internal class ExcelExporter
{
   internal static async Task WriteToExcelAsync(ObservableCollection<ResxEntityViewModel> data,string path,HashSet<string> cultures)
    {
        if (data == null)
        {
            return;
        }
        await Task.Run(() =>
        {
            try
            {
                using var excel = new ExcelWriter(path);

                // write header
                excel.Write("Location", 1, 1);
                excel.Write("Key", 2, 1);
                int colNo = 3;

                foreach (var culture in cultures.OrderBy(x => x))
                {
                    excel.Write(culture, colNo++, 1);
                    var commentStr = culture == string.Empty ? "Comment" : $"Comment.{culture}";
                    excel.Write(commentStr, colNo++, 1);
                }

                //write data

                int row = 2;
                int col = 1;

                for (int i = 0; i < data.Count; i++)
                {
                    col = 1;
                    excel.Write(data[i].ResxEntity.Node.NodePathPartsFromParent.JoinBy("\\"), col++, row);
                    excel.Write(data[i].Key, col++, row);

                    foreach (var key in data[i].CultureValues.Keys.OrderBy(x => ResxEntityViewModel.GetKeyNameFromVm(x)))
                    {
                        excel.Write(data[i].CultureValues[key], col++, row);
                        excel.Write(data[i].CultureComments[key], col++, row);
                    }
                    row++;
                }

                excel.Save();
            }
            catch(Exception ex)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    var box = MessageBoxManager.GetMessageBoxStandard("Error importing", ex.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                    box.ShowWindowDialogAsync(WindowHelper.ParentWindow<MainWindow>());
                });
            }
        });

        
    }
}
