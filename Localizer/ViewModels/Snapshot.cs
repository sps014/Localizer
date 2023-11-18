using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Localizer.ViewModels;

public class Snapshot
{
    public List<SnapShotItem> Items { get; set; } = new();
}
public class SnapShotItem
{
    public string Location { get; set; }
    public string Key { get; set; }
    public Dictionary<string, string?> Values { get; set; } = new();
    public Dictionary<string, string?> Comments { get; set; } = new();
}

[JsonSerializable(typeof(Snapshot))]
public partial class SnapShotContext:JsonSerializerContext
{

}