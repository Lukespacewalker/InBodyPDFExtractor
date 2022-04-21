using DynamicData;
using DynamicData.Binding;
using InBodyPDFExtractor.Models;
using InBodyPDFExtractor.Services;
using Ookii.Dialogs.Wpf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InBodyPDFExtractor.ViewModels;

public class SelectFolderViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    #region RXUI
    public string? UrlPathSegment => "SelectFolder";

    public IScreen HostScreen { get; }

    public ViewModelActivator Activator { get; } = new ViewModelActivator();
    #endregion

    private readonly PdfJobService pdfJobService;
    private readonly NavigationService navigationService;

    [Reactive] internal bool IsPDFViewerLoading { get; set; }
    [Reactive] internal string SelectedFolderPath { get; private set; } = string.Empty;
    [Reactive] internal PdfJob? SelectedPDFJob { get; private set; }
    internal ObservableCollectionExtended<PdfJob> PdfJobs = new();

    internal ReactiveCommand<Unit, Unit> SelectFolder { get; private set; }
    internal ReactiveCommand<Unit, IRoutableViewModel> NavigateToPDFExtractionPage { get; }

    internal SelectFolderViewModel(IScreen? screen = null, 
        PdfJobService? pdfJobService = null,
        NavigationService? navigationService = null)
    {
        this.pdfJobService = pdfJobService ?? Locator.Current.GetService<PdfJobService>()!;
        this.navigationService = navigationService ?? Locator.Current.GetService<NavigationService>()!;
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;

        SelectFolder = ReactiveCommand.CreateFromTask(SelectFolderImpl);

        NavigateToPDFExtractionPage = ReactiveCommand.CreateFromObservable(() => this.navigationService.Navigate.Execute(new PDFExtractionViewModel()), this.pdfJobService.PdfJobs.Connect().AutoRefresh().QueryWhenChanged(query => {
            return query.Items.Any(pdfJob => pdfJob.ToBeWork == true);
        }));

        this.WhenActivated(d =>
        {
            this.pdfJobService.PdfJobs.Connect()
                .ObserveOnDispatcher()
                .Bind(PdfJobs)
                .Subscribe()
                .DisposeWith(d);
        });

    }

    private async Task SelectFolderImpl()
    {
        VistaFolderBrowserDialog dialog = new();
        if (dialog.ShowDialog() ?? false)
        {
            SelectedFolderPath = dialog.SelectedPath;
            EnumerateFolder();
        }
    }
    private void EnumerateFolder()
    {
        pdfJobService.ClearAllJobs();
        var filesPath = Directory.GetFiles(SelectedFolderPath).Where(filePath => Path.GetExtension(filePath).Equals(".pdf", StringComparison.InvariantCultureIgnoreCase)).Select((filePath, index) =>
        {
            return new PdfJob
            {
                Id = index,
                ToBeWork = true,
                FileName = Path.GetFileName(filePath),
                AbsolutePath = filePath
            };
        });
        pdfJobService.PdfJobs.AddOrUpdate(filesPath);
    }
}
