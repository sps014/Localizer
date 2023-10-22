using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Localizer.ViewModels;

namespace Localizer;

public partial class StartupPageWindow : Window
{
    AppSettings settings;
    public StartupPageWindow()
    {
        InitializeComponent();
        TransparencyLevelHint = new[] {WindowTransparencyLevel.Mica};
        settings = AppSettings.Instance;
        //settings.AddFolder(@"C:\DoIt\App.Test");
        //settings.AddFolder(@"C:\SFSF\Bwopler.Test");
        searchBox.ItemsSource = settings.Folders.Select(x=>x.FolderPath).ToList();
        DataContext = settings;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        AppSettings.SaveSettings();
    }

    void LoadFolder(string path)
    {
        this.Close();
    }
    async void BrowseNew()
    {
        var folderDialog = new OpenFolderDialog()
        {
            Title = "Open Folder",
        };

        var path = await folderDialog.ShowAsync(this);
        if (path != null)
        {
            settings.AddFolder(path);
            LoadFolder(path);
        }
    }
    

    private void AutoCompleteBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        LoadFolder((searchBox.SelectedItem as  string)!);
    }
    private void ListBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        LoadFolder(((ResxFolderPath)listView.SelectedItem!).FolderPath);
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var dt = (sender as Button)!.DataContext as ResxFolderPath;
        if (dt == null)
            return;

        settings.RemoveFolder(dt);
    }

    private void BrowseNewClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        BrowseNew();
    }
}