using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Localizer.ViewModels;

public class ResxFolderPath
{
    public string FolderPath { get; set; }
    public DateTime TimeStamp { get; set; }
    public string FolderName => new DirectoryInfo(FolderPath).Name;
    public string DateTimeFormat => DateTime.Now.ToString("f");

    public ResxFolderPath(string folderPath) 
    {
        FolderPath = folderPath;
        TimeStamp = DateTime.Now;
    }
    public ResxFolderPath(string folderPath,DateTime dtime)
    {
        FolderPath = folderPath;
        TimeStamp = dtime;
    }
}

