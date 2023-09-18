using Localizer.Core.Model;

namespace Localizer.Core.Resx;

public record struct ResxEntity
{
    public string Key { get; init; }
    public ResxFileSystemLeafNode Node { get; init; }

}