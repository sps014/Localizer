using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private string? currentLoadItemName;

        private CancellationTokenSource cancellationToken=  new CancellationTokenSource();

        public MainWindowViewModel(string solutionFolder)
        {
            SolutionFolder = solutionFolder;
            ResxManager = new ResxManager(SolutionFolder);
            ResxManager.OnResxReadProgressChanged += ResxManager_OnResxReadProgressChanged;
            ResxManager.OnResxReadStarted += ResxManager_OnResxReadStartedChanged;
            ResxManager.OnResxReadFinished += ResxManager_OnResxReadFinished;
        }

        ~MainWindowViewModel()
        {
            ResxManager.OnResxReadProgressChanged -= ResxManager_OnResxReadProgressChanged;
            ResxManager.OnResxReadFinished -= ResxManager_OnResxReadFinished;
            ResxManager.OnResxReadStarted -= ResxManager_OnResxReadStartedChanged;

        }

        private async void ResxManager_OnResxReadFinished(object sender, Core.Model.ResxReadFinishedEventArgs e)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                CurrentLoadedCount = e.Total;
                IsResxContentLoaded = true;

            });
        }
        private void ResxManager_OnResxReadStartedChanged(object sender, Core.Model.ResxReadStartedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                TotalCount = e.Total;
            });

        }

        private void ResxManager_OnResxReadProgressChanged(object sender, Core.Model.ResxFileReadProgressEventArg e)
        {
            try
            {
                Dispatcher.UIThread.Invoke(() =>
               {

                   CurrentLoadedCount = e.Progress;
                   CurrentLoadItemName = e.FileName;
               }, DispatcherPriority.Render, cancellationToken.Token);
            }
            catch (Exception)
            {
                //
            }
        }

        public async void LoadAsync()
        {
            await ResxManager.BuildCollectionAsync(cancellationToken);
        }

        public void RequestCacellationOfLoadingResx()
        {
            cancellationToken.Cancel();
        }
    }
}
