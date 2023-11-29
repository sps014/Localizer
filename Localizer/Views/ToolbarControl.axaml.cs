using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Localizer.Events;
using Localizer.Helper;
using Localizer.ViewModels;
using Localizer.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Localizer;

public partial class ToolbarControl : UserControl
{
    ToolbarControlViewModel viewModel;
    public ToolbarControl()
    {
        InitializeComponent();
        viewModel = new ToolbarControlViewModel();
        DataContext = viewModel;
    }

    private void langColmnCheckBox_Tapped(object sender,TappedEventArgs e)
    {
        viewModel.PublishLanguageChanged(sender);
    }

    private void CheckBox_DoubleTapped(object sender, TappedEventArgs e)
    {
        viewModel.PublishLanguageChanged(sender);
    }
}
