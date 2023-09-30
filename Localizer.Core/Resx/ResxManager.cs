using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using Localizer.Core.Model;

namespace Localizer.Core.Resx;

public record ResxManager
{
    public ObservableCollection<ResxEntity> ResxEntities { get; private set; } = new();
    private ConcurrentBag<ResxEntity> COncurrentResxEntities { get; init; } = new();
    public ResxLoadDataTree Tree { get; }

    public string SolutionPath => Tree.SolutionFolder;

    public ResxManager(string solutionPath)
    {
        Tree = new ResxLoadDataTree(solutionPath);
    }
    public async Task BuildCollectionAsync()
    {
        await Tree.BuildTreeAsync();

        COncurrentResxEntities.Clear();

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        var nodes = GetAllFileNodes();
        await Parallel.ForEachAsync(nodes, parallelOptions, async (fileNode,token) =>
        {
            if (fileNode is null)
                return;

            await fileNode.ReadAllResourceFiles();

            //group by key

            var keys = fileNode.Keys;

            foreach (var key in keys)
            {
                var entity = new ResxEntity(key, fileNode);
                COncurrentResxEntities.Add(entity);
            }
        });

        ResxEntities = new ObservableCollection<ResxEntity>(COncurrentResxEntities);
    }


    IEnumerable<ResxFileSystemLeafNode> GetAllFileNodes()
    {
        if (Tree.Root is null)
            throw new InvalidOperationException("Root node is null");

        return Tree.Root.GetAllChildren().OfType<ResxFileSystemLeafNode>();
    }

    internal void DeleteEntries(string fullPath)
    {
        for (int i = ResxEntities.Count - 1; i >= 0; i--)
        {
            var entity = ResxEntities[i];
            if (entity.NeutralFilePath == fullPath)
                ResxEntities.RemoveAt(i);
        }
    }
}