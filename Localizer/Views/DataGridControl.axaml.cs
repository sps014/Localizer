using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Localizer.ViewModels;

namespace Localizer;

public partial class DataGridControl : UserControl
{
    DataGridViewModel viewModel;
    public DataGridControl()
    {
        InitializeComponent();
        viewModel = new DataGridViewModel()
        {
            DataGrid = dataGrid
        };
        DataContext = viewModel;
    }

}