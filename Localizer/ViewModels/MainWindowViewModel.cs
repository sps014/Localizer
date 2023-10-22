using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Localizer.Core.Resx;

namespace Localizer.ViewModels
{
    internal partial class MainWindowViewModel: ObservableObject
    {
        public string SolutionFolder { get; }
        public ResxManager ResxManager { get; }


        [NotifyPropertyChangedFor(nameof(ShowProgress))]
        [ObservableProperty]
        private bool isResxContentLoaded;

        public bool ShowProgress => !IsResxContentLoaded;


        [ObservableProperty]
        private int currentLoadedCount;

        [ObservableProperty]
        private int totalCount;

        [ObservableProperty]
        private string currentLoadItemName;

        public MainWindowViewModel(string solutionFolder)
        {
            SolutionFolder = solutionFolder;
            ResxManager = new ResxManager(SolutionFolder);
            ResxManager.OnResxReadProgressChanged += ResxManager_OnResxReadProgressChanged;
            ResxManager.OnResxReadFinished += ResxManager_OnResxReadFinished;
        }

        ~MainWindowViewModel()
        {
            ResxManager.OnResxReadProgressChanged -= ResxManager_OnResxReadProgressChanged;
            ResxManager.OnResxReadFinished -= ResxManager_OnResxReadFinished;
        }

        private void ResxManager_OnResxReadFinished(object sender, Core.Model.ResxReadFinishedEventArgs e)
        {
            IsResxContentLoaded = true;
        }

        private void ResxManager_OnResxReadProgressChanged(object sender, Core.Model.ResxFileReadProgressEventArg e)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                CurrentLoadedCount = e.Progress;
                TotalCount = e.Total;
                CurrentLoadItemName = e.FileName;
            });
        }

        public async void LoadAsync()
        {
            await ResxManager.BuildCollectionAsync();
        }
    }
}
