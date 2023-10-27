using System.Collections;
using System.Collections.Concurrent;
using Localizer.Core.Resx;
namespace Localizer.Core.Model;

public class ResxNodeEntry : IEnumerable<KeyValuePair<string, ResxKeyValueCollection>>
{
    private ConcurrentDictionary<string, ResxKeyValueCollection> culturedKeyValues = new(); //culture -> localized value,comment of culture

    public ResxFileSystemLeafNode LeafNode { get; init; }

    public HashSet<string> Keys
    {
        get
        {
            if (culturedKeyValues.ContainsKey(string.Empty))
            {
                return culturedKeyValues[string.Empty].Keys.ToHashSet();
            }
            else
            {
                return new HashSet<string>();
            }
        }
    }

    public ResxNodeEntry(ResxFileSystemLeafNode leafNode)
    {
        LeafNode = leafNode;
    }


    public void Add(string culture, ResxKeyValueCollection keyValuePairs)
    {
        if (culturedKeyValues.ContainsKey(culture))
            culturedKeyValues[culture] = keyValuePairs;
        else
            culturedKeyValues.TryAdd(culture, keyValuePairs);
    }
    public void Delete(string culture)
    {
        if (culturedKeyValues.ContainsKey(culture))
            culturedKeyValues.TryRemove(culture, out var _);
    }
    public bool ContainsCulture(string culture)
    {
        return culturedKeyValues.ContainsKey(culture);
    }


    IEnumerator IEnumerable.GetEnumerator()
    {
        return culturedKeyValues.GetEnumerator();
    }

    IEnumerator<KeyValuePair<string, ResxKeyValueCollection>> IEnumerable<KeyValuePair<string, ResxKeyValueCollection>>.GetEnumerator()
    {
        return culturedKeyValues.GetEnumerator();
    }

    public void AddKeyValuePair(string culture, string key, string? value, string? comment)
    {
        if (culturedKeyValues.ContainsKey(culture))
        {
            culturedKeyValues[culture].Add(key, value);
            culturedKeyValues[culture].SetComment(key, comment);
        }
    }

    public IEnumerable<string> Cultures => culturedKeyValues.Keys;
    public IEnumerable<ResxKeyValueCollection> Values => culturedKeyValues.Values;

    public bool TryGetFilePath(string culture, out string? path)
    {
        if (culture is null)
            culture = string.Empty;

        if (culturedKeyValues.TryGetValue(culture, value: out var value))
        {
            path = value.FileName;
            return true;
        }
        path = null;
        return false;
    }

    public string? GetValue(string key, string? culture)
    {
        if (culture is null)
            culture = string.Empty;

        if (culturedKeyValues.ContainsKey(culture))
        {
            if (culturedKeyValues[culture].ContainsKey(key))
            {
                return culturedKeyValues[culture][key];
            }
        }
        return null;
    }
    public void SetValue(string key, string? value, string? culture)
    {
        if (culture is null)
            culture = string.Empty;

        var path = LeafNode.GetFilePathOfCulture(culture);
        ResxResourceWriter writer = new ResxResourceWriter(path);


        if (culturedKeyValues.ContainsKey(culture))
        {
            if (culturedKeyValues.ContainsKey(culture))
            {
                culturedKeyValues[culture][key] = value;
                writer.UpdateResource(key, value, culturedKeyValues[culture].GetComment(key));
            }
            else
            {
                culturedKeyValues[culture].Add(key, value);
                writer.AddResource(key, value, culturedKeyValues[culture].GetComment(key));
            }
        }
    }
    public void AddUpdateOrDeleteKey(string key, KeyChangeOperationType type, string? newKey = null)
    {
        foreach (var culture in culturedKeyValues.Keys)
        {
            var path = LeafNode.GetFilePathOfCulture(culture);
            ResxResourceWriter writer = new ResxResourceWriter(path);

            if (type == KeyChangeOperationType.Add)
            {
                culturedKeyValues[culture].AddKey(key);
                writer.UpdateResource(key, culturedKeyValues[culture][key], culturedKeyValues[culture].GetComment(key));
            }
            else if (type == KeyChangeOperationType.Update)
            {
                writer.DeleteResource(key);
                writer.UpdateResource(newKey, culturedKeyValues[culture][key], culturedKeyValues[culture].GetComment(key));
                culturedKeyValues[culture].UpdateKey(key, newKey);
            }
            if (type == KeyChangeOperationType.Delete)
            {
                culturedKeyValues[culture].DeleteKey(key);
                writer.DeleteResource(key);
            }
        }
    }
    public string? GetComment(string key, string? culture)
    {
        if (culture is null)
            culture = string.Empty;

        if (culturedKeyValues.ContainsKey(culture))
        {
            if (culturedKeyValues[culture].ContainsKey(key))
            {
                return culturedKeyValues[culture].GetComment(key);
            }
        }
        return null;

    }
    public void SetComment(string key, string? comment, string? culture = null)
    {
        if (culture is null)
            culture = string.Empty;

        var path = LeafNode.GetFilePathOfCulture(culture);
        ResxResourceWriter writer = new ResxResourceWriter(path);

        if (culturedKeyValues.ContainsKey(culture))
        {
            culturedKeyValues[culture].SetComment(key, comment);
            writer.UpdateResource(key, culturedKeyValues[culture][key],comment);
        }
    }

    public bool TryGetValue(string key, string? culture, out string? value)
    {
        var val = GetValue(key, culture);
        if (val is null)
        {
            value = null;
            return false;
        }
        value = val;
        return true;
    }

    public Task ReadFileOfCulture(string? culture = null)
    {
        return Task.Run(() =>
        {
            if (culture is null)
                culture = string.Empty;

            TryGetFilePath(culture, out var path);

            if (path is null)
                return;

            try
            {
                var resxReader = new ResxResourceReader(path);

                ClearKeysOfCulture(culture);

                foreach (var entry in resxReader)
                {
                    var key = entry.Key.ToString()!;

                    var value = entry.Value;

                    value ??= string.Empty;

                    var comment = entry.Comment;
                    comment ??= string.Empty;

                    AddKeyValuePair(culture, key, value, comment);
                }
            }
            catch
            {
                // ignored
            }
        });
    }

    public void ClearKeysOfCulture(string culture)
    {
        if (culturedKeyValues.ContainsKey(culture))
        {
            culturedKeyValues[culture].Clear();
        }
    }

}

public enum KeyChangeOperationType
{
    Add,
    Update,
    Delete
}