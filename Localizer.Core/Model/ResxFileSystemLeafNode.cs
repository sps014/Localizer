namespace Localizer.Core.Model;

using System.Collections;
using System.Collections.ObjectModel;
using System.Resources.NetStandard;
using Localizer.Core.Helper;
public record ResxFileSystemLeafNode : ResxFileSystemNodeBase
{
    public Dictionary<string, string> CultureFileNameMap { get; init; } = new();

    internal Dictionary<string,Dictionary<string,string?>> CultureKeyValueMap { get; init; } = new();

    public string NeutralFileNameWithExtension => $"{NodeName}.resx";

    public ResxFileSystemLeafNode(string resXFile):base(resXFile, resXFile.GetNeutralFileNameWithoutExtension())
    {
        AddFile(resXFile);
    }

    /// <summary>
    /// Adds a culture file to the current node group of resx files
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="ArgumentException"></exception>
    public void AddFile(string path)
    {
        var cultureName = path.GetCultureName();

        if (CultureFileNameMap.ContainsKey(cultureName))
        {
            throw new ArgumentException($"Duplicate culture file : {path} found");
        }

        CultureFileNameMap.Add(cultureName, path);

    }

    public void ReadFileOfCulture(string? culture=null)
    {
        if(culture is null)
            culture = string.Empty;
        
        CultureFileNameMap.TryGetValue(culture, out var path);

        if (path is null)
            return;

        ResXResourceReader? resxReader=null;
        try
        {
            resxReader = new ResXResourceReader(path);
            foreach(DictionaryEntry entry in resxReader)
            {
                if(entry.Key is null)
                    continue;

                if(!CultureKeyValueMap.ContainsKey(culture))
                    CultureKeyValueMap.Add(culture, new Dictionary<string, string?>());
                
                var key = entry.Key.ToString()!;
                var value = entry.Value is null ? string.Empty : entry.Value.ToString();

                CultureKeyValueMap[culture].TryAdd(key, value);
            }
        }
        catch
        {

        }
        finally
        {
            resxReader?.Close();
        }
        

    }

    public record ResxKeyValue(string Key, string? Value)
    {
        public override string ToString() => $"{Key} : {Value}";
    }
}
