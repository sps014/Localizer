using System.Collections;
using System.Xml.Linq;

namespace Localizer.Core.Resx;

public class ResxResourceReader:IEnumerable<ResxResourceReader.ResxRecord>
{
    private readonly Dictionary<string, ResxRecord> _records = new();
    private static readonly XName SpaceAttributeName = XNamespace.Xml + "space";
    private const string Preserve = "preserve";

    private bool IsStringType(XElement element)
    {
        // Check if the "xml:space" attribute exists and its value is "preserve"
        var spaceAttribute = element.Attribute(SpaceAttributeName);
        return spaceAttribute != null && spaceAttribute.Value== Preserve;
    }

    public ResxResourceReader(string path)
    { 
        string str = File.ReadAllText(path);

        XDocument doc = XDocument.Parse(str,LoadOptions.PreserveWhitespace);

        foreach (XElement element in doc.Descendants("data"))
        {
            string? key = element.Attribute("name")?.Value;
            string? value = element.Element("value")?.Value;
            string? comment = element.Element("comment")?.Value;

            if(key is null ||!IsStringType(element) || key.StartsWith(">>"))
                continue;
            
            var rec = new ResxRecord(key, value, comment);
            
            _records.Add(key,rec);
        }
    }

    public record ResxRecord(string Key, string? Value, string? Comment);

    public IEnumerator<ResxRecord> GetEnumerator()
    {
        return _records.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _records.Values.GetEnumerator();
    }
}