using System.Collections.Immutable;
using Localizer.Core.Helper;

namespace Localizer.Core.Model;

public record ResxLoadDataTree
{
    public IResxLoadDataNode? Root { get; private set; }

    public void BuildTree(string solutionPath)
    {
        if (!Directory.Exists(solutionPath))
            throw new ArgumentException("Invalid solution path");

        Root = new ResxLoadDataNode(solutionPath.GetDirectoryName()!, solutionPath);

        ScanAndPopulateTree(solutionPath, new CancellationTokenSource());

    }
    private void ScanAndPopulateTree(string solutionPath, CancellationTokenSource cts)
    {
        var resxFiles = Directory.GetFiles(solutionPath, "*.resx", SearchOption.AllDirectories);
        var csprojs = Directory.GetFiles(solutionPath, "*.csproj", SearchOption.AllDirectories)
                        .Select(x => (x.GetDirectoryName()!,x)).ToImmutableDictionary(x=>x.Item1,x=>x.Item2);

        var solutionFolderName = solutionPath.GetDirectoryName();

        ParallelOptions options = new()
        {
            CancellationToken = cts.Token,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        Parallel.ForEach(resxFiles, options, (resxFile) =>
        {
            var directoryInfo = new DirectoryInfo(resxFile);

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
                        var isCsProj = csprojs.ContainsKey(directoryInfo.FullName); // is directory contains a csproj
                        var newNode = new ResxLoadDataNode(part, directoryInfo.FullName)
                        {
                            IsCsProjNode = isCsProj,
                            CsProjPath = isCsProj ? csprojs[directoryInfo.FullName] : null
                        };
                        node.AddChild(newNode);
                        currentFolder = newNode;
                    }
                }
            }

            currentFolder.AddChild(new ResxLoadDataLeafNode(resxFile.GetFileName(), resxFile));

        });
    }
}
