using System.Collections;
using System.Resources.NetStandard;

namespace Localizer.Core.Model;

public class ResxNodeEntry : IEnumerable<KeyValuePair<string, ResxKeyValueCollection>>
{
    private Dictionary<string, ResxKeyValueCollection> culturedKeyValues = new();

    public ResxFileSystemLeafNode LeafNode { get; init; }

    public HashSet<string> Keys => culturedKeyValues[string.Empty].Keys.ToHashSet();

    public ResxNodeEntry(ResxFileSystemLeafNode leafNode)
    {
        LeafNode = leafNode;
    }

    public void Add(string culture, ResxKeyValueCollection keyValuePairs)
    {
        if (culturedKeyValues.ContainsKey(culture))
            culturedKeyValues[culture] = keyValuePairs;
        else
            culturedKeyValues.Add(culture, keyValuePairs);
    }
    public void Delete(string culture)
    {
        if (culturedKeyValues.ContainsKey(culture))
            culturedKeyValues.Remove(culture);
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

    public void AddKeyValuePair(string culture, string key, string? value)
    {
        if (culturedKeyValues.ContainsKey(culture))
        {
            culturedKeyValues[culture].Add(key, value);
        }
    }

    public IEnumerable<string> Cultures => culturedKeyValues.Keys;
    public IEnumerable<ResxKeyValueCollection> Values => culturedKeyValues.Values;

    public bool TryGetFilePath(string culture, out string? path)
    {
        if (culture is null)
            culture = string.Empty;

        if (culturedKeyValues.ContainsKey(culture))
        {
            path = culturedKeyValues[culture].FileName;
            return true;
        }
        path = null;
        return false;
    }

    public string? GetValue(string key,string? culture)
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

    public bool TryGetValue(string key,string? culture,out string? value)
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

    public void ReadFileOfCulture(string? culture = null)
    {
        if (culture is null)
            culture = string.Empty;

        TryGetFilePath(culture, out var path);

        if (path is null)
            return;

        ResXResourceReader? resxReader = null;

        try
        {
            resxReader = new ResXResourceReader(path);

            ClearKeysOfCulture(culture);

            foreach (DictionaryEntry entry in resxReader)
            {
                if (entry.Key is null)
                    continue;

                var key = entry.Key.ToString()!;

                var value = entry.Value is null ? string.Empty : entry.Value.ToString();

                AddKeyValuePair(culture, key, value);
            }
        }
        catch
        {
            // ignored
        }
        finally
        {
            resxReader?.Close();
        }


    }

    public void ClearKeysOfCulture(string culture)
    {
        if (culturedKeyValues.ContainsKey(culture))
        {
            culturedKeyValues[culture].Clear();
        }
    }

}