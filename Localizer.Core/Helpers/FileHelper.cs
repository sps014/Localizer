namespace Localizer.Core.Helper;

public static class FileHelperExtension
{
    /// <summary>
    /// Returns the file name from the path with extension
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileName(this string path)
    {
        return Path.GetFileName(path);
    }

    /// <summary>
    /// Returns the file name from the path without extension
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileNameWithoutExtension(this string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }

    /// <summary>
    /// Returns the directory name from the path
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string? GetDirectoryName(this string path)
    {
        return Path.GetFileName(path);
    }
}
