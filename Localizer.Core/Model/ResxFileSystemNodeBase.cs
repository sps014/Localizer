﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Localizer.Core.Resx;
using Microsoft.VisualBasic;

namespace Localizer.Core.Model;

/// <summary>
/// Represents a node in the ResxLoadDataTree
/// </summary>
public abstract record class ResxFileSystemNodeBase
{

    /// <summary>
    /// Children of the current node can be either a leaf node or a node where leaf is resx file and node is a folder
    /// </summary>
    public Dictionary<string,ResxFileSystemNodeBase> Children { get; init; } = new();

    /// <summary>
    /// Parent node of the current node
    /// </summary>
    public ResxFileSystemNodeBase? Parent { get; init; }

    /// <summary>
    /// Full path of the current node
    /// </summary>
    public string FullPath { get; init; }

    /// <summary>
    /// Name of the current node (file or directory name)
    /// </summary>
    public string NodeName { get; init; }

    public ImmutableArray<string> NodePathPartsFromParent
    {
        get
        {
            if(Parent == null)
            {
                return ImmutableArray<string>.Empty;
            }

            var collection = new List<string>();
            collection.AddRange(Parent.NodePathPartsFromParent);
            collection.Add(NodeName);
            return collection.ToImmutableArray();
        }
    }


    public ResxFileSystemNodeBase(string fullPath, string nodeName)
    {
        FullPath = fullPath;
        NodeName = nodeName;
    }

    /// <summary>
    /// Denotes if the current node is a leaf node (resxFile)
    /// </summary>
    /// <returns>true if leaf node</returns>
    public bool IsLeafResXFileNode=>this is ResxFileSystemLeafNode;
    public bool IsSimpleFolderNodeFileNode => this is ResxFileSystemFolderNode && !((ResxFileSystemFolderNode)this).IsCsProjNode;
    public bool IsCSharpProjectDirectory => this is ResxFileSystemFolderNode && ((ResxFileSystemFolderNode)this).IsCsProjNode;

    /// <summary>
    /// Adds a child node to the current node
    /// </summary>
    /// <param name="child"></param>
    public void AddChild(ResxFileSystemNodeBase child)
    {
        if (child is ResxFileSystemFolderNode node)
        {
            node.AddChild(node);
        }
    }
    public delegate void OnNodeChangedHandler(ResxFileSystemNodeBase node);
    public event OnNodeChangedHandler? OnNodeChanged;

    public void Dispose()
    {
        OnNodeChanged = null;
    }

    internal void NotifyNodeChanged()
    {
        OnNodeChanged?.Invoke(this);
    }

    public IEnumerable<ResxFileSystemNodeBase> GetAllChildren()
    {
        yield return this;

        foreach (var child in Children)
        {
            foreach (var childChild in child.Value.GetAllChildren())
            {
                yield return childChild;
            }
        }
    }
    public ObservableCollection<ResxFileSystemNodeBase> ChildrenCollection
    {
        get
        {
            //folders first
            return new ObservableCollection<ResxFileSystemNodeBase>(Children.Values.OrderByDescending(x=>x is ResxFileSystemFolderNode));
        }
    }
}