using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using ExCSS;

namespace Localizer.Updater
{
    internal static class NewVersionUpdateChecker
    {
        internal static HttpClient Http = new HttpClient();
        const string owner = "sps014"; 
        const string repo = "Localizer"; 

        const string URL = $"https://api.github.com/repos/{owner}/{repo}/tags";
        internal static string? NewTempFilePath { get; private set; }

        internal static string Extension
        {
            get
            {
                if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return "exe";
                }
                else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return "elf";
                }
                else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return "app";
                }
                throw new NotImplementedException();
            }
        }

        public static string InstallationFolderPath
        {
            get
            {
                var programFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                var pathOfInstallationFolder = Path.Combine(programFilePath, "Localizer", "NewVersion");
                return pathOfInstallationFolder;
            }
        }

        
        private static async Task<VersionResult> IsNewVersionAvailable()
        {
            try
            {
                var appExePath = Process.GetCurrentProcess()!.MainModule!.FileName;
                var currentVersionString =  FileVersionInfo.GetVersionInfo(appExePath).FileVersion;

                if(currentVersionString == null)
                    return new VersionResult(false,null);

                var currentVersion = Version.Parse(currentVersionString);


                Http.DefaultRequestHeaders.Add("User-Agent", "request");
                Http.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                var response = await Http.GetStreamAsync(URL);

                var jsonDocument = await JsonDocument.ParseAsync(response);
                var latestTag = jsonDocument.RootElement[0].GetProperty("name").GetString()!.TrimStart('v');

                var latestTagVersion = Version.Parse(latestTag); 

                var isNewVersion = currentVersion.CompareTo(latestTagVersion)<0;

                return new VersionResult(isNewVersion, latestTagVersion);
            }
            catch
            {
                return new VersionResult(false, null);
            }
        }
        private static async Task DownloadFileToTemp(Version newVersion)
        {
            if(newVersion==null) return;

            try
            {
                string localFilePath = Path.Combine(Path.GetTempPath(),$"Localizer.{newVersion}.{Extension}");

                var url = $"https://github.com/sps014/Localizer/releases/download/v{newVersion}/Localizer.{Extension}";

                using var response = await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                using var streamToReadFrom = await response.Content.ReadAsStreamAsync();

                using var streamToWriteTo = File.OpenWrite(localFilePath);

                await streamToReadFrom.CopyToAsync(streamToWriteTo);

                NewTempFilePath = localFilePath;

            }
            catch
            {
                //ignore
            }
        }

        //Replace current version with the new one on gitlab
        public static void ExecuteUpdateOnClose()
        {
            if (!File.Exists(NewTempFilePath))
                return;

            try
            {   
                var currentAppPath = Process.GetCurrentProcess().MainModule!.FileName;
                ProcessStartInfo? psi = null;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {

                    var command = $"/C timeout 3 >nul & Del  \"{currentAppPath}\" & move /Y {NewTempFilePath} {currentAppPath}";

                    psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = command,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    };

                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    //execute original
                    var command = $"-c sleep 3; chmod --reference={currentAppPath} {NewTempFilePath}; rm -f \"{currentAppPath}\"; mv -f {NewTempFilePath} {currentAppPath}";

                    psi = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = command,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    };
                }
                else
                    throw new NotImplementedException();

                if(psi!=null)
                    Process.Start(psi);

            }
            catch
            {
                //ignore
            }
        }

        public static async void RunUpdater()
        {
            await Task.Run(async() =>
            {
                //Check if new version available 
                var result = await IsNewVersionAvailable();


                if (!result.IsAvailable)
                    return;

                //download file to temp folder 
                await DownloadFileToTemp(result.NewVersion!);

            });

        }

    }
}

    internal record VersionResult(bool IsAvailable,Version? NewVersion);
