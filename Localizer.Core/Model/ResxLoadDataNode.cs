using System.Collections.Concurrent;

namespace Localizer.Core.Model;

public record struct ResxLoadDataNode(string FolderName, string FullPath) : IResxLoadDataNode
{
    /// <summary>
    /// Children of the current node can be either a leaf node or a node where leaf is resx file and node is a folder
    /// </summary>
    public ConcurrentDictionary<string, IResxLoadDataNode> Children { get; init; } = new();

    public void AddChild(IResxLoadDataNode child)
    {
        if (child is ResxLoadDataNode node)
        {
            this.Children.TryAdd(node.FolderName, node);
        }
        else if (child is ResxLoadDataLeafNode leaf)
        {
            this.Children.TryAdd(leaf.FileName, leaf);
        }
        else
            throw new ArgumentException("Invalid child type");
    }

}
