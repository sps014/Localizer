using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Localizer.Core.Helper;

namespace Localizer.Core.Model;

public record ResxLoadDataTree
{
    [JsonPropertyName("root")]
    public IResxLoadDataNode? Root { get; private set; }
    private ImmutableDictionary<string, ProjectInfo> CsProjs = ImmutableDictionary<string, ProjectInfo>.Empty;
    public string? SolutionFolder { get; private set; }

    public void BuildTree(string solutionPath, CancellationTokenSource? cts=default)
    {
        if (!Directory.Exists(solutionPath))
            throw new ArgumentException("Invalid solution path");

        Root = new ResxLoadDataNode(solutionPath.GetDirectoryName()!, solutionPath);

        ScanAndPopulateTree(solutionPath, cts);

    }
    private void ScanAndPopulateTree(string solutionPath, CancellationTokenSource? cts)
    {
        SolutionFolder = solutionPath;

        var resxFiles = Directory.GetFiles(solutionPath, "*.resx", SearchOption.AllDirectories);
        CsProjs = Directory.GetFiles(solutionPath, "*.csproj", SearchOption.AllDirectories)
                        .Select(x => (x.GetParentDirectory()!, new ProjectInfo(x)))
                        .ToImmutableDictionary(x => x.Item1, x => x.Item2);


        foreach (var resxFile in resxFiles)
        {
            AddNewFileToTree(resxFile);

        }

    }

    private void AddNewFileToTree(string resxFile)
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
            if (currentFolder is ResxLoadDataNode node)
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
                    var newNode = new ResxLoadDataNode(part, currentDirPath)
                    {
                        IsCsProjNode = isCsProj,
                        CsProjPath = isCsProj ? CsProjs[currentDirPath].ProjectPath : null
                    };
                    node.AddChild(newNode);
                    currentFolder = newNode;
                }
            }
        }

        if (currentFolder is ResxLoadDataNode cur)
            cur.AddLeafFile(resxFile);
        else
            throw new InvalidOperationException("Invalid , should be a folder node");
    }

    private record struct ProjectInfo(string ProjectPath)
    {
        public string ProjectName => ProjectPath.GetFileName();
        public string ProjectDirectory => ProjectPath.GetDirectoryName()!;
    }
}


