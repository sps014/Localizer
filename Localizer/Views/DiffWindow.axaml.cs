using Avalonia.Controls;
using Avalonia.Interactivity;
using Localizer.ViewModels;

namespace Localizer.Views
{
    public partial class DiffWindow : Window
    {
        DiffWindowViewModel viewModel;
        public DiffWindow()
        {
            InitializeComponent();
            MainWindow.SetThemeOfWindow(this);
            viewModel = new DiffWindowViewModel()
            {
                DataGrid = dataGrid
            };
            DataContext = viewModel;
        }
        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            viewModel.Init();
        }
        public void AddRowNumbers(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
