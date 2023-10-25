using System.Collections;
using System.Text.RegularExpressions;
using Localizer.Core.Helpers;
using Localizer.Core.Resx;

namespace Localizer.Core.Model;

/// <summary>
/// Represents a collection of key value pairs for a culture (usually content of a single resx file)
/// </summary>
public record ResxKeyValueCollection : IEnumerable<KeyValuePair<string, string?>>
{
    private Dictionary<string, string?> KeyValuePairs = new();
    private Dictionary<string, string?> KeyCommentPairs = new();

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

    public void SetComment(string key,string? comment)
    {
        if(KeyCommentPairs.ContainsKey(key))
            KeyCommentPairs[key] = comment;
        else
            KeyCommentPairs.Add(key, comment);
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
    public void DeleteComment(string key)
    {
        if (KeyCommentPairs.ContainsKey(key))
            KeyCommentPairs[key] = null;
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
    internal void ClearComment()
    {
        KeyCommentPairs.Clear();
    }

    public string? GetComment(string key)
    {
        return KeyCommentPairs.GetValueOrDefault(key);
    }

    internal void AddUpdateOrDeleteKey(string key, KeyChangeOperationType type,string? newKey)
    {
        if (type == KeyChangeOperationType.Delete)
        {
            KeyValuePairs.Remove(key);
        }
        else if (type == KeyChangeOperationType.Update)
        {
            var previousValue = KeyValuePairs[key];
            KeyValuePairs.Remove(key);
            newKey ??= string.Empty;
            KeyValuePairs.Add(newKey, previousValue);
        }
        else
        {
            KeyValuePairs.Add(key,string.Empty);
        }
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