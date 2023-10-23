using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Localizer.Core.Model;
using Localizer.Core.Resx;

namespace Localizer.ViewModels;

internal partial class TreeControlViewModel:ObservableObject
{
    private ResxManager ResxManager;

    [ObservableProperty]
    private ResxFileSystemNodeBase? root;

    [ObservableProperty]
    private ObservableCollection<ResxFileSystemNodeBase> nodes=new ObservableCollection<ResxFileSystemNodeBase>();

    public TreeControlViewModel()
    {
        ResxManager = MainWindowViewModel.Instance!.ResxManager;
        ResxManager.OnResxReadFinished += ResxManager_OnResxReadFinished;
    }
    ~TreeControlViewModel()
    {
        ResxManager.OnResxReadFinished -= ResxManager_OnResxReadFinished;
    }
    private void ResxManager_OnResxReadFinished(object sender, ResxReadFinishedEventArgs e)
    {
        ProcessNodes();
    }

    void ProcessNodes()
    {
        Root = ResxManager.Tree.Root!;

        Nodes = new ObservableCollection<ResxFileSystemNodeBase>
        {
            Root
        };
    }
}
