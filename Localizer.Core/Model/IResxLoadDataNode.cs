using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Localizer.Core.Model;

/// <summary>
/// Represents a node in the ResxLoadDataTree
/// </summary>
public interface IResxLoadDataNode
{

    /// <summary>
    /// Parent node of the current node
    /// </summary>
    public IResxLoadDataNode? Parent { get;init;}

    /// <summary>
    /// Denotes if the current node is a leaf node (resxFile)
    /// </summary>
    /// <returns>true if leaf node</returns>
    public bool IsLeafResXFileNode()
    {
        return this is ResxLoadDataLeafNode;
    }

    /// <summary>
    /// Adds a child node to the current node
    /// </summary>
    /// <param name="child"></param>
    public void AddChild(IResxLoadDataNode child)
    {
        if (child is ResxLoadDataNode node)
        {
            node.AddChild(node);
        }
    }
}