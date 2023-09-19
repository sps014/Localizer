using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using Localizer.Core.Model;

namespace Localizer.Core.Resx;

public record ResxEntity
{
    public string Key { get; }

    public string NeutralDirectoryPath => Node.FullPath;

    public ImmutableList<string> Cultures => Node.CultureFileNameMap.Keys.ToImmutableList();


    public string? GetValue(string? culture = null)
    {
        if (culture is null)
            culture = string.Empty;

        var isCulturePresent = Node.CultureKeyValueMap.TryGetValue(culture, out var collection);

        if (!isCulturePresent)
            return null;

        if (collection is null)
            return null;

        if (collection.ContainsKey(Key))
            return collection[Key];

        return null;

    }

    public ResxFileSystemLeafNode Node { get; init; }

    public ResxEntity(string key, ResxFileSystemLeafNode node)
    {
        Node = node;
        Key = key;
    }

}