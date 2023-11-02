namespace Localizer.Core.Model;

using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using Localizer.Core.Helpers;
using Localizer.Core.Resx;

public record ResxFileSystemLeafNode : ResxFileSystemNodeBase
{
    public ResxNodeEntry ResxEntry { get; init; }

    public string NeutralFileNameWithExtension => $"{NodeName}.resx";

    /// <summary>
    /// Neutral file keys 
    /// </summary>
    public HashSet<string> Keys => ResxEntry.Keys;

    public ResxFileSystemLeafNode(string resXFile) : base(resXFile, resXFile.GetNeutralFileName())
    {
        ResxEntry = new(this);
    }

    public string? GetProjectName()
    {
        var parent = Parent;

        while(parent!=null)
        {
            if(parent is ResxFileSystemFolderNode folder)
            {
                if (folder.IsCsProjNode)
                    return folder.NodeName;
            }
            parent = parent.Parent;
        }

        return null;
    }

    public bool TryGetAbsolutePath(string culture, out string? path)
    {
        return ResxEntry.TryGetFilePath(culture, out path);
    }

    public string GetFilePathOfCulture(string culture)
    {
        var dir = Path.GetDirectoryName(ResxEntry.LeafNode.FullPath)!;
        var cultureText = (culture==string.Empty?"":"."+culture) + ".resx";
        dir = Path.Combine(dir, ResxEntry.LeafNode.NodeName+cultureText);
        return dir;
    }

    /// <summary>
    /// Adds a culture file to the current node group of resx files
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="ArgumentException"></exception>
    public void AddFile(string path)
    {
        var cultureName = path.GetCultureName();
        
        ResxEntry.Add(cultureName, new ResxKeyValueCollection(path));

    }

    public async Task ReadAllResourceFiles(CancellationToken token)
    {
        foreach (var culture in ResxEntry.Cultures)
        {
            if(token.IsCancellationRequested) return;
            await ResxEntry.ReadFileOfCulture(culture);
        }
    }

    public record ResxKeyValue(string Key, string? Value)
    {
        public override string ToString() => $"{Key} : {Value}";
    }

    public async Task UpdateFile(string fullPath)
    {
        await ResxEntry.ReadFileOfCulture(fullPath.GetCultureName());
    }

    public ResxEntity AddNewKey(string key,ResxManager manager)
    {
        ResxEntry.AddUpdateOrDeleteKey(key, KeyChangeOperationType.Add);
        var entity = new ResxEntity(key, this, manager);
        manager.ResxEntities.Add(entity);
        return entity;
    }
}
