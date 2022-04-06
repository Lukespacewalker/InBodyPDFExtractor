using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Wpf;
using Ookii.Dialogs.Wpf;
using ReactiveUI.Fody.Helpers;
using System.IO;
using DynamicData;
using InBodyPDFExtractor.Models;
using System.Reactive;
using Splat;
using DynamicData.Binding;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Syncfusion.Pdf;
using Syncfusion.Windows.PdfViewer;
using InBodyPDFExtractor.Utilities;
using System.Windows.Threading;
using InBodyPDFExtractor.Services;

namespace InBodyPDFExtractor.ViewModels;

public class PDFSelectionViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    #region RXUI
    public string? UrlPathSegment => "Main";

    public IScreen HostScreen { get; }

    public ViewModelActivator Activator { get; } = new ViewModelActivator();
    #endregion

    private readonly PdfJobService pdfJobService;
    [Reactive] internal string SelectedFolderPart { get; private set; } = string.Empty;
    [Reactive] internal PdfJob? SelectedPDFJob { get; private set; }
    internal ReactiveCommand<Unit, Unit> SelectFolder { get; private set; }
    internal ReactiveCommand<Unit, Unit> ExtractPDFs { get; private set; }

    internal ObservableCollectionExtended<PdfJob> PdfJobs = new();


    internal PDFSelectionViewModel(IScreen? screen = null, PdfJobService? pdfJobService = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        this.pdfJobService = pdfJobService ?? Locator.Current.GetService<PdfJobService>()!;

        SelectFolder = ReactiveCommand.Create(SelectFolderImpl);
        ExtractPDFs = ReactiveCommand.CreateFromTask(this.pdfJobService.RunAllJobs, outputScheduler: RxApp.MainThreadScheduler);

        this.WhenActivated(d =>
        {
            this.pdfJobService.PdfJobs.Connect()
            .ObserveOnDispatcher()
            .Bind(PdfJobs)
            .Subscribe().DisposeWith(d);

            this.WhenAnyValue(vm => vm.SelectedPDFJob).WhereNotNull().Select(job => this.pdfJobService.ExtractedTextLineCollection[job.])
        });
    }
    private void SelectFolderImpl()
    {
        VistaFolderBrowserDialog dialog = new();
        if (dialog.ShowDialog() ?? false)
        {
            SelectedFolderPart = dialog.SelectedPath;
            EnumerateFolder();
        }
    }
    private void EnumerateFolder()
    {
        pdfJobs.Clear();
        var filesPath = Directory.GetFiles(SelectedFolderPart).Select(file =>
        {
            return new PdfJob
            {
                FileName = Path.GetFileName(file),
                AbsolutePath = file
            };
        });
        pdfJobs.AddRange(filesPath);
    }
}
