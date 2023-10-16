namespace Localizer.Core.Model;

using System.Collections;
using System.Collections.ObjectModel;
using Localizer.Core.Helpers;
public record ResxFileSystemLeafNode : ResxFileSystemNodeBase
{
    public ResxNodeEntry ResxEntry { get; init; }

    public string NeutralFileNameWithExtension => $"{NodeName}.resx";

    /// <summary>
    /// Neutral file keys 
    /// </summary>
    public HashSet<string> Keys => ResxEntry.Keys;

    public ResxFileSystemLeafNode(string resXFile) : base(resXFile, resXFile.GetNeutralFileNameWithoutExtension())
    {
        ResxEntry = new(this);
    }

    /// <summary>
    /// Adds a culture file to the current node group of resx files
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="ArgumentException"></exception>
    public void AddFile(string path)
    {
        var cultureName = path.GetCultureName();

        if (ResxEntry.ContainsCulture(cultureName))
        {
            return;
        }

        ResxEntry.Add(cultureName, new ResxKeyValueCollection(path));

    }

    public async Task ReadAllResourceFiles()
    {
        foreach (var culture in ResxEntry.Cultures)
        {
            await ResxEntry.ReadFileOfCulture(culture);
        }
    }

    public record ResxKeyValue(string Key, string? Value)
    {
        public override string ToString() => $"{Key} : {Value}";
    }
}
