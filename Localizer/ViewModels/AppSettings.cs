using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Localizer.ViewModels;

public class AppSettings
{
    internal readonly static string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),nameof(Localizer), $"{nameof(AppSettings)}.json");

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private static AppSettings instance;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static AppSettings Instance
    {
        get
        {
            if (instance == null)
                LoadSettings();

            return instance!;
        }
    }

    public Dictionary<string, DateTime> FolderTimeStampMap { get; set; } = new Dictionary<string, DateTime>();

    private ObservableCollection<ResxFolderPath>? folders=null;


    [JsonIgnore]
    public ObservableCollection<ResxFolderPath> Folders
    {
        get
        {
            if (folders!=null)
            {
                return folders;
            }

            folders = new ObservableCollection<ResxFolderPath>();

            foreach (var item in FolderTimeStampMap.OrderByDescending(x=>x.Value))
            {
                folders.Add(new ResxFolderPath(item.Key, item.Value));
            }
            return folders;
        }
    }

    private static object instanceLock = new object();
    public static void LoadSettings()
    {
        lock (instanceLock)
        {
            try
            {
               var json = File.ReadAllText(FilePath);
               instance = (AppSettings)JsonSerializer.Deserialize(json, typeof(AppSettings), AppSettingsContext.Default)!;
            }
            catch(Exception e)
            {
                instance = new AppSettings();
            }

            if(instance==null)
                instance = new AppSettings();
        }
    }
    public static void SaveSettings()
    {
        lock(instanceLock)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);

                var json = JsonSerializer.Serialize(instance,typeof(AppSettings),AppSettingsContext.Default);

                File.WriteAllText(FilePath, json);
            }
            catch(Exception e) { }
        }
    }

    public void AddFolder(string path)
    {
        if(FolderTimeStampMap.ContainsKey(path))
        {
            FolderTimeStampMap[path] = DateTime.Now;
        }
        else
            FolderTimeStampMap.Add(path, DateTime.Now);

    }
    public void RemoveFolder(ResxFolderPath folder)
    {
        Folders.Remove(folder);
        FolderTimeStampMap.Remove(folder.FolderPath);
    }
}

[JsonSerializable(typeof(AppSettings))]
internal partial class AppSettingsContext:JsonSerializerContext
{

}