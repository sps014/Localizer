using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Threading;
using Localizer.Core.Helpers;
using Localizer.Events;
using Localizer.ViewModels;
using MsBox.Avalonia;

namespace Localizer.Helper
{
    internal class SnapshotLoader
    {
        public static Task LoadSnapshot(ObservableCollection<ResxEntityViewModel> entities,string path)
        {
            return Task.Run(() =>
            {
                try
                {
                    var json = File.ReadAllText(path);
                    var snapshot = JsonSerializer.Deserialize(json,typeof(Snapshot),SnapShotContext.Default) as Snapshot;

                    var mapOfEntityFullPath = entities.ToDictionary(x => x.ResxEntity.Node.NodePathPartsFromParent.Add(x.Key).JoinBy("\\"), y => y);

                    foreach(var snapItem in snapshot.Items)
                    {
                        var key = snapItem.Location + "\\" + snapItem.Key;

                        if(!mapOfEntityFullPath.ContainsKey(key))
                        {
                            continue;
                        }

                        var entity = mapOfEntityFullPath[key];

                        entity.SnapshotCultureValues = snapItem.Values;
                        entity.SnapshotCultureComments = snapItem.Comments;
                    }

                }
                catch(Exception ex)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard("Error importing", ex.Message,
                            MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                        box.ShowWindowDialogAsync(WindowHelper.ParentWindow<MainWindow>());
                    });
                }
            });
        }
    }
}
