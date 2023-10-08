
using Localizer.Core.Model;
using Localizer.Core.Resx;

namespace Localizer.Core.Watcher;

public class ResxFileWatcher
{
    private readonly FileSystemWatcher _watcher;
    private ResxLoadDataTree _tree;
    public ResxManager? ResxManager { get; set; }
    public ResxFileWatcher(ResxLoadDataTree tree)
    {
        _tree = tree;
        _watcher = new FileSystemWatcher(tree.SolutionFolder, "*.resx");
        _watcher.IncludeSubdirectories = true;
        _watcher.EnableRaisingEvents = true;
        _watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

        _watcher.Created += OnCreated;
        _watcher.Changed += OnChanged;
        _watcher.Deleted += OnDeleted;
        _watcher.Renamed += OnRenamed;
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        ResxManager?.DeleteFile(e.OldFullPath);
        if (e.FullPath.EndsWith(".resx"))
            _tree.AddNewFileToTree(e.FullPath);
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        ResxManager?.DeleteFile(e.FullPath);
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        _tree.AddNewFileToTree(e.FullPath);
    }
}