namespace Localizer.Core.Model;
using Localizer.Core.Helper;
public class ResxLoadDataLeafNode : IResxLoadDataNode
{
    public Dictionary<string,string> CultureFileNameMap { get; init; } = new();

    private string? neutralFileName = null;
    public string NeutralFileName => neutralFileName!;
    public string NeutralFileNameWithExtension => $"{NeutralFileName}.resx";

    public ResxLoadDataLeafNode(string resXFile)
    {
        AddFile(resXFile);
    }
   
    public void AddFile(string path)
    {
        var neutralName = path.GetNeutralFileName();
        var cultureName = path.GetCultureName();

        if (CultureFileNameMap.ContainsKey(cultureName))
        {
            throw new ArgumentException($"Duplicate culture file : {path} found");
        }

        CultureFileNameMap.Add(cultureName, path);
        neutralFileName ??= neutralName;

    }

}
