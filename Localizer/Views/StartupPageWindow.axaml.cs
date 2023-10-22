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

        MainWindow.SetThemeOfWindow(this);

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
        var mainWindow = new MainWindow(path);
        mainWindow.Show();
        this.Close();
        settings.AddFolder(path,true);
    }

    async void BrowseNew()
    {
        var folders =await StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions
        {
            AllowMultiple = false,
            Title = "Open Folder with .resx files",
        });


        var folder = folders.FirstOrDefault();

        if(folder != null)
        {
            var path = folder.Path.LocalPath;
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