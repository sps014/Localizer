using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using Localizer.Core.Helpers;
using Localizer.Core.Model;

namespace Localizer.Core.Resx;

public record ResxManager
{
    public ObservableCollection<ResxEntity> ResxEntities { get; private set; } = new();
    private ConcurrentBag<ResxEntity> ConcurrentResxEntities { get; init; } = new();
    public ResxLoadDataTree Tree { get; }

    public string SolutionPath => Tree.SolutionFolder;

    private int _filesRead = 0;

    public ResxManager(string solutionPath)
    {
        Tree = new ResxLoadDataTree(solutionPath,this);
    }

    public async Task BuildCollectionAsync()
    {
        _filesRead = 0;

        await Tree.BuildTreeAsync();

        ConcurrentResxEntities.Clear();

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        var nodes = GetAllFileNodes().ToImmutableArray();
        int _total = nodes.Length;

        await Parallel.ForEachAsync(nodes, parallelOptions, async (fileNode, token) =>
        {
            Interlocked.Increment(ref _filesRead);

            OnResxReadProgressChanged?.Invoke(this, new ResxFileReadProgressEventArg(fileNode.FullPath, _filesRead, _total));
            
            if (fileNode is null)
                return;

            await fileNode.ReadAllResourceFiles();

            //group by key

            var keys = fileNode.Keys;

            foreach (var key in keys)
            {
                var entity = new ResxEntity(key, fileNode);
                ConcurrentResxEntities.Add(entity);
            }
        });

        ResxEntities = new ObservableCollection<ResxEntity>(ConcurrentResxEntities);

        OnResxReadFinished?.Invoke(this, new ResxReadFinishedEventArgs("All files", _total));
    }


    IEnumerable<ResxFileSystemLeafNode> GetAllFileNodes()
    {
        if (Tree.Root is null)
            throw new InvalidOperationException("Root node is null");

        return Tree.Root.GetAllChildren().OfType<ResxFileSystemLeafNode>();
    }

    private void DeleteEntries(string fullPath)
    {
        for (int i = ResxEntities.Count - 1; i >= 0; i--)
        {
            var entity = ResxEntities[i];
            if (entity.NeutralFilePath == fullPath)
                ResxEntities.RemoveAt(i);
        }
    }
    public void DeleteFile(string fullPath)
    {
        Tree.DeleteFileFromTree(fullPath);
        DeleteEntries(fullPath);
    }

    public delegate void OnResxReadProgressChangedEventHandler(object sender, ResxFileReadProgressEventArg e);
    public delegate void OnResxReadFinishedEventHandler(object sender, ResxReadFinishedEventArgs e);

    public event OnResxReadProgressChangedEventHandler? OnResxReadProgressChanged;
    public event OnResxReadFinishedEventHandler? OnResxReadFinished;

    private SemaphoreSlim semaphore = new SemaphoreSlim(1,1);

    public async void UpdateFileNode(string fullPath)
    {
        await semaphore.WaitAsync();
        if(!fullPath.EndsWith(".resx",StringComparison.OrdinalIgnoreCase))
            return;

        var node = Tree.GetNodeFromPath(fullPath) as ResxFileSystemLeafNode;
        
        if(node is null)
            return;
        
        await node.UpdateFile(fullPath);
        
        // remove resource entries for that neutral item
        var exactNeutralPath = fullPath.GetNeutralFileFullPath();

        for (int i = ResxEntities.Count-1; i >=0; i--)
        {
            var item = ResxEntities[i];
            if (item.TryGetAbsolutePath(string.Empty, out string? curPath))
            {
                if (exactNeutralPath == curPath)
                {
                    ResxEntities.RemoveAt(i);
                }
            }
        }
        
        //add new entries
        var keys = node.Keys;
        foreach (var key in keys)
        {
            var entity = new ResxEntity(key, node);
            ResxEntities.Add(entity);
        }

        var values = ResxEntities.Select(x => x.GetValue(string.Empty)).ToList();
        OnResxRefresh?.Invoke(fullPath);
        semaphore.Release();
    }

    public delegate void ResxFIleRefreshHandler(string name);

    public event ResxFIleRefreshHandler? OnResxRefresh;
}