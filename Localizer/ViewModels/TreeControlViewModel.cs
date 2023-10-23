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

    [ObservableProperty]
    private string? searchedQuery;

    private bool IsInSearchMode => !string.IsNullOrWhiteSpace(SearchedQuery);

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

    public void SearchNodes()
    {
        Nodes = CreateSearchTree(SearchedQuery);

        if (IsInSearchMode)
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
                await Task.Delay(TimeSpan.FromMilliseconds(10)); //let parent expand 
                await ExpandChildNodes(container);
            }
        }
    }


    public void OnTreeNodeSelection(ResxFileSystemNodeBase node)
    {
        if (IsInSearchMode)
        {
            var nodeInOriginalTree = NodeFromNodeKeys(node);

            SearchedQuery = string.Empty;

            if (nodeInOriginalTree == null)
                return;


            ExpandNodeWithPath(nodeInOriginalTree);

            TreeView.SelectedItem = nodeInOriginalTree;
        }
    }

    private async void ExpandNodeWithPath(ResxFileSystemNodeBase nodeInOriginalTree)
    {
        var pathParts = nodeInOriginalTree.NodePathPartsFromParent;
        var container = TreeView.ContainerFromItem(Nodes.First()) as TreeViewItem;
        if(container != null)
        {
            container.IsExpanded = true;

            await Task.Delay(TimeSpan.FromMilliseconds(10));

            foreach(var path in pathParts)
            {
                var items = container.Items;

                var childItem = items.FirstOrDefault(x=>(x as ResxFileSystemNodeBase)!.NodeName == path); 

                if(childItem==null) return;

                container = container.ContainerFromItem(childItem) as TreeViewItem;
                container.IsExpanded = true;
                await Task.Delay(TimeSpan.FromMilliseconds(10));
            }
        }

    }

    public static ResxFileSystemNodeBase? NodeFromNodeKeys(ResxFileSystemNodeBase node)
    {
        var pathparts = node.NodePathPartsFromParent;
        ResxFileSystemNodeBase curNode = MainWindowViewModel.Instance!.ResxManager.Tree.Root!;

        foreach (var path in  pathparts)
        {
            if(curNode.Children.ContainsKey(path))
                curNode = curNode.Children[path];
            else
                return null;
        }

        return curNode;
    }


    ObservableCollection<ResxFileSystemNodeBase> CreateSearchTree(string? searchTerm)
    {
        if(!IsInSearchMode)
        {
            return new ObservableCollection<ResxFileSystemNodeBase>
            {
                ResxManager.Tree.Root!
            };
        }

        var newRoot = ResxManager.Tree.Root! with { Children = new() };

        DfsQueryBuildTree(searchTerm!, ResxManager.Tree.Root!, newRoot);

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
