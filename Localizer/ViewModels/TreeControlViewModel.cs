using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
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

    [ObservableProperty]
    private ObservableCollection<string>? nodeNames = null;

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

        NodeNames = new ObservableCollection<string>(Root.GetAllChildren().Select(x => x.NodeName).ToHashSet());

        CreateSearchTree("Form");
    }


    public void SearchNodes(string searchTerm)
    {
        Nodes = CreateSearchTree(searchTerm);
    }

    
    ObservableCollection<ResxFileSystemNodeBase> CreateSearchTree(string searchTerm)
    {
        if(string.IsNullOrWhiteSpace(searchTerm))
        {
            return new ObservableCollection<ResxFileSystemNodeBase>
            {
                ResxManager.Tree.Root!
            };
        }

        var newRoot = ResxManager.Tree.Root! with { Children = new() };

        DfsQueryBuildTree(searchTerm, ResxManager.Tree.Root!, newRoot);

        return new ObservableCollection<ResxFileSystemNodeBase>
            {
                newRoot
            };

    }

    bool DfsQueryBuildTree(string searchTerm, ResxFileSystemNodeBase parent,ResxFileSystemNodeBase newParent)
    {
        bool anyTrue = false;
        foreach (var node in parent.Children)
        {
            var newNode = node.Value with { Children = new Dictionary<string, ResxFileSystemNodeBase>() };
            if (DfsQueryBuildTree(searchTerm, node.Value,newNode ))
            { 
                anyTrue = true;
                newParent.Children.Add(node.Key,newNode);
            }
        }
        if(anyTrue)
        {
            return anyTrue;
        }

        if (parent.NodeName.Contains(searchTerm))
        {
            return true;
        }

        return false;

    }

}
