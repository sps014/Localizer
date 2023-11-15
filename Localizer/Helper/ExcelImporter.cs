using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localizer.Core.Resx;
using Localizer.ViewModels;
using ExcelDataReader;
using System.IO;
using System.Data;
using MsBox.Avalonia;
using Avalonia.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Localizer.Core.Helpers;
using Localizer.Events;

namespace Localizer.Helper
{
    internal class ExcelImporter
    {
        internal static async Task ReadFromExcel(string path, ResxManager resxManager)
        {
            await Task.Run(()=>
            {
                try
                {
                    var enc = CodePagesEncodingProvider.Instance.GetEncoding(1252);
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    using var stream = File.OpenRead(path);
                    using var reader = ExcelReaderFactory.CreateReader(stream);
                    var dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = false,
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    var table = dataset.Tables[0];

                    if (table.Columns[0].ColumnName != "Location" || table.Columns[1].ColumnName != "Key")
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            var box = MessageBoxManager.GetMessageBoxStandard("Error importing", "Can't import Excel file as xlsx file is not a valid resx dataset.", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                            box.ShowWindowDialogAsync(WindowHelper.ParentWindow<MainWindow>());
                        });

                        return;
                    }

                    //read rest column names exculing 3
                    var columns = new List<IndexedColumn>();

                    for (int i =2;i<table.Columns.Count;i++)
                    {
                        var name = table.Columns[i].ColumnName;
                        IndexedColumn? column=null;
                        if(!name.StartsWith("Comment"))

                            column =new IndexedColumn(i,
                                name.StartsWith("Column")?string.Empty:name,
                                false);
                        else
                            column = new IndexedColumn(i,
                                name.Contains(".") ? name.Split('.').Last() : string.Empty,
                                true);

                        columns.Add(column);
                    }

                    //already created entities map
                    var createdEntitiesMap = resxManager.ResxEntities.ToDictionary(x => x.Node.NodePathPartsFromParent.Add(x.Key).JoinBy("\\"), y => y);
                    

                    //read all rows now
                    for(int i =0;i<table.Rows.Count;i++)
                    {
                        var items = table.Rows[i].ItemArray;

                        var location = items[0].ToString()!;
                        var key = items[1].ToString()!;

                        var searchKey = location + "\\" + key;

                        //update existing
                        if (createdEntitiesMap.ContainsKey(searchKey))
                        {
                            var entity = createdEntitiesMap[searchKey];

                            foreach(var col in columns)
                            {
                               if(!col.IsComment)
                                {
                                    var alreadyVal = entity.GetValue(col.Culture);
                                    alreadyVal ??= string.Empty;
                                    var newVal = items[col.Index].ToString();

                                    if(!alreadyVal.Equals(newVal))
                                    {
                                        entity.SetValue(newVal, col.Culture);
                                    }
                                }
                               else
                                {
                                    var alreadyVal = entity.GetComment(col.Culture);
                                    alreadyVal ??= string.Empty;
                                    var newVal = items[col.Index].ToString();

                                    if (!alreadyVal.Equals(newVal))
                                    {
                                        entity.SetComment(newVal, col.Culture);
                                    }
                                }
                            }
                        }
                        //create a new file with keys
                        else
                        {
                            foreach(var group in columns.GroupBy(x=>x.Culture))
                            {
                                var culture = group.First().Culture;
                                var indexOfValue = group.First(x => !x.IsComment).Index;
                                var indexOfComment = group.First(x => x.IsComment).Index;

                                var value = items[indexOfValue].ToString();
                                var comment = items[indexOfComment].ToString();

                                var path = resxManager.SolutionPath;

                                var resxPath = Path.Combine(path, location.Replace('\\', Path.PathSeparator));

                                //if not neutral append culture in fileName
                                if (culture != string.Empty)
                                    resxPath += $".{culture}";
                                resxPath += ".resx";

                                ResxResourceWriter writer = new ResxResourceWriter(resxPath);
                                writer.UpdateResource(key,value??string.Empty, comment??string.Empty);
                            }
                        }

                        EventBus.Instance.Publish(new ReloadResourcesEvent());

                    }

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

        internal record IndexedColumn(int Index,string Culture,bool IsComment);
    }
}
