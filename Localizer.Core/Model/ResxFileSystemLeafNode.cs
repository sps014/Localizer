namespace Localizer.Core.Model;

using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using Localizer.Core.Helpers;
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

    public bool TryGetAbsolutePath(string culture, out string? path)
    {
        return ResxEntry.TryGetFilePath(culture, out path);
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
}
