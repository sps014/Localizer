using System.Text.RegularExpressions;
using Localizer.Core.Helpers;

namespace Localizer.Core.Helpers;

public static class FileHelperExtension
{
    /// <summary>
    /// Returns the file name from the full path with extension
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
    /// Returns the directory name from the full path
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string? GetDirectoryName(this string path)
    {
        return Path.GetFileName(path);
    }
    /// <summary>
    /// Returns the directory full path from the path
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string? GetParentDirectory(this string path)
    {
        return Path.GetDirectoryName(path);
    }

    public static string Join(this IEnumerable<string> strings, string separator)
    {
        return string.Join(separator, strings);
    }

    /// <summary>
    /// Returns the neutral file name from the path (relative or full path)
    /// </summary>
    /// <param name="fullpath"></param>
    /// <returns></returns>
    public static string GetNeutralFileName(this string fullpath)
    {
        var parts = fullpath.GetFileNameWithoutExtension().Split('.');

        //if no culture, add to empty string
        if (parts.Length == 1)
        {
            return fullpath.GetFileNameWithoutExtension();
        }
        else
        {
            return parts.SkipLast(1).Join(".");
        }
    }

    public static string GetNeutralFileNameWithoutExtension(this string fullpath)
    {
        return Path.GetFileNameWithoutExtension(fullpath.GetNeutralFileName());
    }
    /// <summary>
    /// Returns the culture name from the path (relative or full path)
    /// </summary>
    /// <param name="fullpath"></param>
    /// <returns></returns>
    public static string GetCultureName(this string fullpath)
    {
        var parts = fullpath.GetFileNameWithoutExtension().Split('.');

        //if no culture, add to empty string
        if (parts.Length == 1)
        {
            return string.Empty;
        }
        else
        {
            var culture = parts.Last();

            //Handle file name with multiple dots 
            if (Culture.AllCultures.Contains(culture))
            {
                return culture;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
