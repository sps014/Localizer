using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Localizer.Core.Helpers;

namespace Localizer.Core.Model;

public record class ResxFileSystemFolderNode : ResxFileSystemNodeBase
{

    public ResxFileSystemFolderNode(string folderName, string fullPath):base(fullPath, folderName)
    {

    }

    /// <summary>
    /// If the current node is a csproj node, this will contain the path to the csproj file
    /// </summary>
    public string? CsProjPath { get; init; } = null;

    /// <summary>
    /// Does current node contains a Csproj
    /// </summary>
    public bool IsCsProjNode { get; init; } = false;

    /// <summary>
    /// Adds a child node to the current node
    /// </summary>
    public new void AddChild(ResxFileSystemNodeBase child)
    {
        if (child is ResxFileSystemFolderNode node)
        {
            Children.Add(node.NodeName, node);
        }
        else
            throw new ArgumentException("Invalid child type");
    }
    public void AddLeafFile(string fileName)
    {
        var neutralName = fileName.GetNeutralFileName();
        if (this.Children.ContainsKey(neutralName))
        {
            var node = this.Children[neutralName];

            if (node is ResxFileSystemLeafNode leafNode)
            {
                leafNode.AddFile(fileName);
            }
            else
            {
                throw new ArgumentException("Invalid child type");
            }
        }
        else
        {
            var leafNode = new ResxFileSystemLeafNode(neutralName)
            {
                Parent = this
            };
            leafNode.AddFile(fileName);
            this.Children.Add(neutralName,leafNode);
        }
    }
    public ObservableCollection<ResxFileSystemNodeBase> SortedChildren()
    {
        var sorted = Children.Values.OrderByDescending(x => x is ResxFileSystemFolderNode);
        return new ObservableCollection<ResxFileSystemNodeBase>(sorted);
    }

    internal void RemoveChild(ResxFileSystemNodeBase node, string fileName)
    {
        if (node is ResxFileSystemFolderNode n)
        {
            Children.Remove(n.NodeName);
        }
        else if (node is ResxFileSystemLeafNode leafNode)
        {
            var culture = fileName.GetCultureName();

            // remove the culture file from the leaf node
            leafNode.ResxEntry.Delete(culture);

            // if there is no culture file, remove the leaf node
            if(!leafNode.ResxEntry.Any())
            {
                Children.Remove(leafNode.NodeName);
            }
        }
        else
        {
            throw new ArgumentException("Invalid child type");
        }
    }
}
