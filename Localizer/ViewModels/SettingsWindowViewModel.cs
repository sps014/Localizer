using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Localizer.Helper;
using Localizer.Updater;
using Localizer.Views;
using static System.Net.WebRequestMethods;

namespace Localizer.ViewModels;

public partial class SettingsWindowViewModel:ObservableObject
{
    [ObservableProperty]
    private bool useMicaOrAcrylicDesign;
    public string Repo => "https://github.com/sps014/Localizer";

    [ObservableProperty]
    private bool isNewVersionAvailable;

    [ObservableProperty]
    private string? newVersion;

    public string Version
    {
        get
        {
            var assemblyName = Process.GetCurrentProcess().MainModule!.FileName;
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assemblyName);
            return "v" + fvi.FileVersion;
        }
    }

    public SettingsWindowViewModel() 
    {
        UseMicaOrAcrylicDesign = AppSettings.Instance.UseMicaOrAcrylic;
        CheckNewVersionAvailable();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if(e.PropertyName==nameof(UseMicaOrAcrylicDesign))
        {
            AppSettings.Instance.UseMicaOrAcrylic = UseMicaOrAcrylicDesign;
            AppSettings.SaveSettings();
        }
    }

    [RelayCommand]
    public async Task CopySourceLink()
    {
        var data = new DataObject();
        data.Set(DataFormats.Text, Repo);
        await WindowHelper.ParentWindow<SettingsWindow>()!.Clipboard!.SetDataObjectAsync(data);
    }

    async void CheckNewVersionAvailable()
    {
        var result =await  NewVersionUpdateChecker.IsNewVersionAvailable();
        IsNewVersionAvailable = result.IsAvailable;
        if (result.IsAvailable)
            NewVersion = "v "+result.NewVersion!.ToString();
    }

}