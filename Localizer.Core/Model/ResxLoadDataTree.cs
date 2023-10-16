using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Localizer.Core.Helpers;
using Localizer.Core.Watcher;

namespace Localizer.Core.Model;

public record ResxLoadDataTree
{
    [JsonPropertyName("root")]
    public ResxFileSystemNodeBase? Root { get; private set; }
    private ImmutableDictionary<string, ProjectInfo> CsProjs = ImmutableDictionary<string, ProjectInfo>.Empty;
    public string SolutionFolder { get; private set; }
    private ResxFileWatcher resxFileWatcher;

    public HashSet<string> Cultures { get; } = new();

    public ResxLoadDataTree(string solutionPath)
    {
        if (!Directory.Exists(solutionPath))
            throw new ArgumentException("Invalid solution path");

        Root = new ResxFileSystemFolderNode(solutionPath.GetDirectoryName()!, solutionPath);
        SolutionFolder = solutionPath;
        resxFileWatcher = new ResxFileWatcher(this);
    }
    public Task BuildTreeAsync(CancellationTokenSource? cts = default)
    {
        Cultures.Clear();
        return Task.Run(() => ScanAndPopulateTree(cts), cts == null ? CancellationToken.None : cts.Token);
    }
    private void ScanAndPopulateTree(CancellationTokenSource? cts)
    {

        var resxFiles = Directory.GetFiles(SolutionFolder, "*.resx", SearchOption.AllDirectories);
        CsProjs = Directory.GetFiles(SolutionFolder, "*.csproj", SearchOption.AllDirectories)
                        .Select(x => (x.GetParentDirectory()!, new ProjectInfo(x)))
                        .ToImmutableDictionary(x => x.Item1, x => x.Item2);


        foreach (var resxFile in resxFiles)
        {
            AddNewFileToTree(resxFile);

            if (cts != null && cts.IsCancellationRequested)
                break;

        }

    }

    public ResxFileSystemNodeBase? GetNodeFromPath(string fullPath)
    {
        if (fullPath == null)
            throw new ArgumentNullException(nameof(fullPath));

        if (fullPath == SolutionFolder)
            return Root;

        var solutionFolderName = SolutionFolder!.GetDirectoryName();

        bool isFile = Path.HasExtension(fullPath) && fullPath.EndsWith(".resx", StringComparison.OrdinalIgnoreCase);

        if (isFile && !fullPath.EndsWith(".resx"))
            return null;

        var parts = fullPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
                .SkipWhile(x => x != solutionFolderName).Skip(1).ToList();

        var currentFolder = Root!;

        for (int i = 0; i < parts.Count; i++)
        {
            var part = parts[i];

            // if it is file part will be neutral file name
            if (i == parts.Count - 1 && isFile)
            {
                part = fullPath.GetNeutralFileNameWithoutExtension();
            }

            if (currentFolder is ResxFileSystemFolderNode node)
            {
                if (node.Children.TryGetValue(part, out var child))
                {
                    currentFolder = child;
                }
                else
                {
                    return null;
                }
            }
        }

        return currentFolder;
    }

    public void AddNewFileToTree(string resxFile)
    {
        var directoryInfo = new DirectoryInfo(resxFile);

        var solutionFolderName = SolutionFolder!.GetDirectoryName();


        // Get the parts of the path after the solution folder excluding the file name
        var parts = resxFile.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
                .SkipWhile(x => x != solutionFolderName)
                .Skip(1).SkipLast(1);

        var currentFolder = Root!;

        foreach (var part in parts)
        {
            // If the current folder is a directory node, then we can add  more children to it
            if (currentFolder is ResxFileSystemFolderNode node)
            {

                // If the current folder already has a child with the same name, then we can skip adding it
                if (node.Children.TryGetValue(part, out var child))
                {
                    currentFolder = child;
                }
                else
                {
                    var currentDirPath = Path.Combine(node.FullPath, part);
                    var isCsProj = CsProjs.ContainsKey(currentDirPath); // is directory contains a csproj
                    var newNode = new ResxFileSystemFolderNode(part, currentDirPath)
                    {
                        IsCsProjNode = isCsProj,
                        CsProjPath = isCsProj ? CsProjs[currentDirPath].ProjectPath : null,
                        Parent = node
                    };
                    node.AddChild(newNode);
                    currentFolder = newNode;
                }
            }
        }

        if (currentFolder is ResxFileSystemFolderNode cur)
        {
            Cultures.Add(resxFile.GetCultureName());
            cur.AddLeafFile(resxFile);
        }
        else
            throw new InvalidOperationException("Invalid , should be a folder node");
    }

    internal void DeleteFileFromTree(string oldFullPath)
    {
        var node = GetNodeFromPath(oldFullPath);
        if (node is null)
            return;

        var parent = node.Parent;

        if (parent is ResxFileSystemFolderNode parentNode)
        {
            parentNode.RemoveChild(node, oldFullPath);
        }
        else
        {
            throw new InvalidOperationException("Invalid , should be a folder node");
        }

        parent.NotifyNodeChanged();
    }

    private record struct ProjectInfo(string ProjectPath)
    {
        public string ProjectName => ProjectPath.GetFileName();
        public string ProjectDirectory => ProjectPath.GetDirectoryName()!;
    }
}


