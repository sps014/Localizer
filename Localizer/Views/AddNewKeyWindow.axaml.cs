using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Localizer.Core.Model;
using Localizer.Events;

namespace Localizer;

public partial class AddNewKeyWindow : Window
{
    public AddNewKeyWindow()
    {
        InitializeComponent();
        MainWindow.SetThemeOfWindow(this);
    }

    public required ResxFileSystemLeafNode Node { get; set; }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (keyBox.Text == null)
            return;

        EventBus.Instance.Publish(new AddNewKeyToResourceEvent(keyBox.Text,Node));
        this.Close();
    }

    private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
}