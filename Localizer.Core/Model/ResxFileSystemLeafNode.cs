namespace Localizer.Core.Model;
using Localizer.Core.Helper;
public record ResxFileSystemLeafNode : ResxFileSystemNodeBase
{
    public Dictionary<string, string> CultureFileNameMap { get; init; } = new();
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
}
