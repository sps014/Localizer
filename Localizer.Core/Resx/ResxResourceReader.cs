using System.Collections;
using System.Xml.Linq;

namespace Localizer.Core.Resx;

public class ResxResourceReader:IEnumerable<ResxResourceReader.ResxRecord>
{
    private readonly Dictionary<string, ResxRecord> _records = new();
    
    public ResxResourceReader(string path)
    {
        XDocument doc = XDocument.Load(path);

        foreach (XElement element in doc.Descendants("data"))
        {
            string? key = element.Attribute("name")?.Value;
            string? value = element.Element("value")?.Value;
            string? comment = element.Element("comment")?.Value;

            if(key is null)
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