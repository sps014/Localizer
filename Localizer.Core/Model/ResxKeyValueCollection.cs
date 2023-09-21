using System.Collections;
using Localizer.Core.Helper;

namespace Localizer.Core.Model;

/// <summary>
/// Represents a collection of key value pairs for a culture (usually content of a single resx file)
/// </summary>
public record ResxKeyValueCollection : IEnumerable<KeyValuePair<string, string?>>
{
    private Dictionary<string, string?> KeyValuePairs = new();
    public string CurrentCulture { get; init; }
    public string FileName { get; init; } = string.Empty;

    public ResxKeyValueCollection(string fileName)
    {
        var culture = fileName.GetCultureName();
        FileName = fileName;
        CurrentCulture = culture;
    }

    /// <summary>
    /// Adds a key value pair to the collection
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(string key, string? value)
    {
        if (KeyValuePairs.ContainsKey(key))
            KeyValuePairs[key] = value;
        else
            KeyValuePairs.Add(key, value);
    }

    /// <summary>
    /// Deletes a key value pair from the collection
    /// </summary>
    /// <param name="key"></param>
    public void Delete(string key)
    {
        if (KeyValuePairs.ContainsKey(key))
            KeyValuePairs.Remove(key);
    }


    /// <summary>
    /// Checks if the collection contains a key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(string key)
    {
        return KeyValuePairs.ContainsKey(key);
    }

    /// <summary>
    /// Gets the all the keys in the collection
    /// </summary>
    public IEnumerable<string> Keys => KeyValuePairs.Keys;

    /// <summary>
    /// Gets all the values in the collection
    /// </summary>
    public IEnumerable<string?> Values => KeyValuePairs.Values;

    public IEnumerator<KeyValuePair<string, string?>> GetEnumerator()
    {
        return KeyValuePairs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    internal void Clear()
    {
        KeyValuePairs.Clear();
    }

    public string? this[string key]
    {
        get => KeyValuePairs.GetValueOrDefault(key, null);
        set
        {
            if (KeyValuePairs.ContainsKey(key))
                KeyValuePairs[key] = value;
            else
                KeyValuePairs.Add(key, value);
        }
    }

}