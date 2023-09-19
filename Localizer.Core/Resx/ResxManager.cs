using System.Collections;
using System.Collections.ObjectModel;
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

                //iterate all cultures
                foreach(var culture in fileNode.CultureFileNameMap.Keys)
                {
                    fileNode.ReadFileOfCulture(culture);
                }

                //group by key
                var groupedByKeys = fileNode.CultureKeyValueMap
                    .SelectMany(x => x.Value)
                    .GroupBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.ToDictionary(y => y.Key, y => y.Value));
            }
        });
    }

    IEnumerable<ResxFileSystemLeafNode> GetAllFileNodes()
    {
        if (Tree.Root is null)
            throw new InvalidOperationException("Root node is null");

        return Tree.Root.GetAllChildren().OfType<ResxFileSystemLeafNode>();
    }
}