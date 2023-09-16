using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Localizer.Core.Model;

public record struct ResxLoadDataNode(string FolderName, string FullPath) : IResxLoadDataNode
{
    [JsonPropertyName("children")]
    /// <summary>
    /// Children of the current node can be either a leaf node or a node where leaf is resx file and node is a folder
    /// </summary>
    public Dictionary<string, IResxLoadDataNode> Children { get; init; } = new();

    [JsonPropertyName("csprojPath")]
    public string? CsProjPath { get; init; } = null;

    /// <summary>
    /// Does current node contains a Csproj
    /// </summary>
    public bool IsCsProjNode { get; init; } = false;

    public void AddChild(IResxLoadDataNode child)
    {
        if (child is ResxLoadDataNode node)
        {
            Children.Add(node.FolderName, node);
        }
        else if (child is ResxLoadDataLeafNode leaf)
        {
            Children.Add(leaf.FileName, leaf);
        }
        else
            throw new ArgumentException("Invalid child type");
    }
    public IEnumerable<IResxLoadDataNode> SortedChildren()
    {
        return Children.Values.OrderByDescending(x => x is ResxLoadDataNode);
    }

}
