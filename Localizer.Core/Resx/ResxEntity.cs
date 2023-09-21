using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using Localizer.Core.Model;

namespace Localizer.Core.Resx;

public record ResxEntity
{
    public string Key { get; }

    public string NeutralFilePath => Node.FullPath;

    public ImmutableList<string> Cultures => Node.ResxEntry.Cultures.ToImmutableList();


    public string? GetValue(string? culture = null)
    {
        var value = Node.ResxEntry.GetValue(Key, culture);
        return value;
    }

    public ResxFileSystemLeafNode Node { get; init; }

    public ResxEntity(string key, ResxFileSystemLeafNode node)
    {
        Node = node;
        Key = key;
    }

}