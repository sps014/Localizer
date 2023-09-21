using System.Collections;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using Localizer.Core.Model;

namespace Localizer.Core.Resx;

public record ResxManager
{
    public ObservableCollection<ResxEntity> ResxEntities { get; init; } = new();
    public ResxLoadDataTree Tree { get; }

    public ResxManager(ResxLoadDataTree resxLoadDataTree)
    {
        Tree = resxLoadDataTree;
    }
    public async Task BuildCollectionAsync()
    {
        ResxEntities.Clear();
        await Task.Run(() =>
        {
            foreach (var fileNode in GetAllFileNodes())
            {
                if (fileNode is null)
                    continue;

                fileNode.ReadAllResourceFiles();

                //group by key

                var keys = fileNode.Keys;

                foreach (var key in keys)
                {
                    var entity = new ResxEntity(key, fileNode);
                    ResxEntities.Add(entity);
                }
            }
        });
    }

    IEnumerable<ResxFileSystemLeafNode> GetAllFileNodes()
    {
        if (Tree.Root is null)
            throw new InvalidOperationException("Root node is null");

        return Tree.Root.GetAllChildren().OfType<ResxFileSystemLeafNode>();
    }

    internal void DeleteEntries(string fullPath)
    {
        for(int i=ResxEntities.Count-1;i>=0;i--)
        {
            var entity = ResxEntities[i];
            if (entity.NeutralFilePath == fullPath)
                ResxEntities.RemoveAt(i);
        }
    }
}