using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localizer.ViewModels;
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
            using var excel = new ExcelWriter(path);

            // write header
            excel.Write("Project", 1, 1);
            excel.Write("File", 2, 1);
            excel.Write("Key", 3, 1);
            int colNo = 4;

            foreach (var culture in cultures.OrderBy(x => x))
            {
                excel.Write(culture,colNo++,1);
                var commentStr = culture == string.Empty ? "Comment":$"Comment.{culture}";
                excel.Write(commentStr, colNo++, 1);
            }

            //write data

            int row = 2;
            int col = 1;

            for (int i = 0; i < data.Count; i++)
            {
                col = 1;
                excel.Write(data[i].ResxEntity.Node.GetProjectName(), col++, row);
                excel.Write(data[i].ResxEntity.Node.NodeName, col++, row);
                excel.Write(data[i].Key, col++, row);

                foreach (var key in data[i].CultureValues.Keys.OrderBy(x=>ResxEntityViewModel.GetKeyNameFromVm(x)))
                {
                    excel.Write(data[i].CultureValues[key],col++,row);
                    excel.Write(data[i].CultureComments[key], col++, row);
                }
                row++;
            }

            excel.Save();
        });

        
    }
}
