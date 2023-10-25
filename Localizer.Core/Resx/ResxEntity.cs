using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using Localizer.Core.Model;

namespace Localizer.Core.Resx;

public record ResxEntity
{
    public string Key { get; private set; }
    public ResxManager Manager { get; private set; }

    public string NeutralFilePath => Node.FullPath;

    public ImmutableList<string> Cultures => Node.ResxEntry.Cultures.ToImmutableList();

    public bool TryGetAbsolutePath(string culture, out string? path)
    {
        return Node.TryGetAbsolutePath(culture, out path);
    }
    public string? GetValue(string? culture = null)
    {
        var value = Node.ResxEntry.GetValue(Key, culture);
        return value;
    }
    public string? GetComment(string? culture = null)
    {
        var value = Node.ResxEntry.GetComment(Key, culture);
        return value;
    }

    public ResxFileSystemLeafNode Node { get; init; }

    public ResxEntity(string key, ResxFileSystemLeafNode node,ResxManager manager)
    {
        Node = node;
        Key = key;
        Manager =manager;
    }

    public void SetValue(string? value,string? culture = null)
    {
        Node.ResxEntry.SetValue(Key, value, culture);
    }

    public void SetComment(string? comment,string? culture)
    {
        Node.ResxEntry.SetComment(Key, comment, culture);
    }

    public void AddUpdateOrDeleteKey(KeyChangeOperationType type, string? newKey=null)
    {
        Node.ResxEntry.AddUpdateOrDeleteKey(Key, type, newKey);

        if(type == KeyChangeOperationType.Update)
        {
            Key = newKey!;
        }
        else if(type == KeyChangeOperationType.Delete)
        {
            Manager.ResxEntities.Remove(this);
        }
    }

}