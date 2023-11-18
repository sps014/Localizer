using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Localizer.Core.Helpers;
using Localizer.ViewModels;
using System.IO;
using Avalonia.Threading;
using MsBox.Avalonia;

namespace Localizer.Helper
{
    internal static class SnapshotCreator
    {
        public static Task CreateSnapshot(IEnumerable<ResxEntityViewModel> entities, string path)
        {
            return Task.Run(() =>
            {
                try
                {
                    var snap = new Snapshot();

                    foreach (var entry in entities)
                    {
                        var item = new SnapShotItem();
                        item.Location = entry.ResxEntity.Node.NodePathPartsFromParent.JoinBy("\\");
                        item.Key = entry.Key;
                        item.Comments = entry.CultureComments;
                        item.Values = entry.CultureValues;
                        snap.Items.Add(item);
                    }

                    var json = JsonSerializer.Serialize(snap,typeof(Snapshot),SnapShotContext.Default);

                    File.WriteAllText(path, json);
                }
                catch (Exception ex)
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