using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Localizer.Helper;

public static class IncludeAllResxFiles
{
    public static void Build(string solutionFolder)
    {
        var projd = Directory.GetFiles(solutionFolder, "*.csproj", SearchOption.AllDirectories);

        bool isModified = false;

        foreach (var projPath in projd)
        {
            isModified = false;

            var isSdkStyleProject = Regex.IsMatch(File.ReadAllText(projPath), "^\\s*\\<\\s*Project\\s+Sdk\\s*=\\s*\"");

            if (isSdkStyleProject)
                continue;

            var doc = new XmlDocument();
            doc.Load(projPath);

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003");
            var embeddedResources = doc.SelectNodes("//msbuild:EmbeddedResource", nsmgr)
                .Cast<XmlNode>()
                .Select(x => x.Attributes["Include"].Value)
                .ToHashSet();

            var projDir = Path.GetDirectoryName(projPath);
            var localizedResx = GetLocalizedResX(projDir);

            foreach (var resource in localizedResx)
            {
                var relativePath = resource.Replace(projDir + "\\", string.Empty);
                if (!embeddedResources.Contains(relativePath))
                {
                    //AnsiConsole.MarkupLine($"[green] adding {relativePath} in {projPath} [/]");

                    var itemGroup = doc.CreateElement("ItemGroup", nsmgr.LookupNamespace("msbuild"));
                    var embeddedResource = doc.CreateElement("EmbeddedResource", nsmgr.LookupNamespace("msbuild"));
                    embeddedResource.SetAttribute("Include", relativePath);
                    var dependentUpon = doc.CreateElement("DependentUpon", nsmgr.LookupNamespace("msbuild"));

                    var csFile = GetDependsOnFileName(relativePath,resource); //depends on path

                    dependentUpon.InnerText = csFile;
                    embeddedResource.AppendChild(dependentUpon);
                    itemGroup.AppendChild(embeddedResource);
                    doc.DocumentElement.AppendChild(itemGroup);
                    isModified = true;
                }
                else
                {
                    //AnsiConsole.MarkupLine($"[yellow] already exist resource {relativePath} in {projPath} [/]");
                    var file = doc.SelectSingleNode($"//msbuild:EmbeddedResource[@Include='{relativePath}']", nsmgr);
                    if (file != null)
                    {
                        var dependentUponNode = file.SelectSingleNode("msbuild:DependentUpon", nsmgr);
                       

                        var dependentUpon = doc.CreateElement("DependentUpon", nsmgr.LookupNamespace("msbuild"));
                        var csFile = GetDependsOnFileName(relativePath, resource); //new depends on parg

                        //if dependentUponNode is correct continue do not modify xml
                        if(csFile==dependentUponNode.InnerText)
                            continue;

                        if (dependentUponNode != null)
                            file.RemoveChild(dependentUponNode);
                        
                        dependentUpon.InnerText = csFile;
                        file.AppendChild(dependentUpon);

                        isModified = true;
                    }
                }
            }

            if(isModified)
                doc.Save(projPath);

        }
        //AnsiConsole.MarkupLine("[green] Finished Adding ResX in csproj[/]");
        //AnsiConsole.WriteLine("Done");


    }

    private static string GetDependsOnFileName(string relativeFilePath,string absoluteFilePath)
    {
        var csFile = relativeFilePath.Split('\\').Last();
        csFile = csFile.Replace(".fr", string.Empty)
            .Replace(".de", string.Empty)
            .Replace(".zh-CN", string.Empty)
            .Replace(".resx", string.Empty);

        if (IsWinformDesignerFile(absoluteFilePath))
            csFile += ".cs";
        else
            csFile += ".resx";

        return csFile;
    }
    private static string[] GetLocalizedResX(string projectPath)
    {
        //AnsiConsole.MarkupLine("[yellow] finding all resources in directory [/]");

        var resx = Directory.GetFiles(projectPath, "*.resx", SearchOption.AllDirectories);
        return resx.Where(y =>
        {
            var x = Path.GetFileNameWithoutExtension(y);
            return x.EndsWith("de") || x.EndsWith("fr") || x.EndsWith("zh-CN");
        }).ToArray();
    }

    private static bool IsWinformDesignerFile(string resxFile)
    {
        resxFile = resxFile.Replace(".fr", string.Empty);
        resxFile = resxFile.Replace(".de", string.Empty);
        resxFile = resxFile.Replace(".zh-CN", string.Empty);

        var designerFile = resxFile.Replace(".resx", ".cs");
        return File.Exists(designerFile);
    }

    private static HashSet<string> memorizedCultures = new HashSet<string>();

    private static HashSet<string> GetAllCulturesLanguageCode()
    {
        if(memorizedCultures.Count != 0)
        {
            return memorizedCultures;
        }

        CultureInfo[] cinfo =
            CultureInfo.GetCultures(CultureTypes.AllCultures &
            ~CultureTypes.NeutralCultures);

        foreach (var cultureInfo in cinfo)
        {
            if (!string.IsNullOrWhiteSpace(cultureInfo.Name))
            {
                memorizedCultures.Add(cultureInfo.Name);
                var parts = cultureInfo.Name.Split('-');
                if (parts.Length > 1)
                    memorizedCultures.Add(parts[0]);
            }
        }

        return memorizedCultures;
    }
}


