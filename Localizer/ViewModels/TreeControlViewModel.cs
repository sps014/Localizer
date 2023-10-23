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

    public required TreeView TreeView { get; internal set; }

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

    bool IsSearchedTree(string searchTerm)
    {
        return !string.IsNullOrWhiteSpace(searchTerm);
    }

    public void SearchNodes(string searchTerm)
    {
        Nodes = CreateSearchTree(searchTerm);

        if (IsSearchedTree(searchTerm))
            ExpandAllNodes();
    }

    async void ExpandAllNodes()
    {
        foreach (var item in TreeView.Items)
        {
            TreeViewItem? container = TreeView.TreeContainerFromItem(item!) as TreeViewItem;
            if (container != null)
            {
                container.IsExpanded = true;
                await Task.Delay(TimeSpan.FromMilliseconds(10)); //let parent expand 
                await ExpandChildNodes(container);

            }
        }
    }

    async Task ExpandChildNodes(TreeViewItem parentContainer)
    {
        foreach (var item in parentContainer.Items)
        {
            TreeViewItem? container = parentContainer.ContainerFromItem(item!) as TreeViewItem;
            if (container != null)
            {
                container.IsExpanded = true;
                await Task.Delay(TimeSpan.FromSeconds(10)); //let parent expand 
                await ExpandChildNodes(container);
            }
        }
    }


    ObservableCollection<ResxFileSystemNodeBase> CreateSearchTree(string searchTerm)
    {
        if(!IsSearchedTree(searchTerm))
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

        if (parent.NodeName.Contains(searchTerm,StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        return false;

    }

}
