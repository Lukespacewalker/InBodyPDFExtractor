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

public class PDFExtractionViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    #region RXUI
    public string? UrlPathSegment => "PDFExtraction";

    public IScreen HostScreen { get; }

    public ViewModelActivator Activator { get; } = new ViewModelActivator();
    #endregion

    private readonly PdfJobService pdfJobService;
    private readonly NavigationService navigationService;
    [Reactive] internal bool IsPDFViewerLoading { get; set; }
    [Reactive] internal PdfJob? SelectedPDFJob { get; private set; }

    [ObservableAsProperty]
    internal List<TextLine> SelectedPDFExtractedTextLines { get; }
    [ObservableAsProperty]
    internal List<ExtractionGroup> SelectedPDFExtractionGroups { get; }

    [ObservableAsProperty]
    internal Person SelectedPDFPerson { get; }

    internal ReactiveCommand<Unit, IRoutableViewModel?> GoBack { get; private set; }

    internal ReactiveCommand<Unit, Unit> SaveXlsx { get; private set; }


    internal ObservableCollectionExtended<PdfJob> PdfJobs = new();

    internal IObservable<double>? Progress { get; private set; }


    internal PDFExtractionViewModel(
        IScreen? screen = null,
        PdfJobService? pdfJobService = null,
        NavigationService? navigationService = null)
    {
        this.pdfJobService = pdfJobService ?? Locator.Current.GetService<PdfJobService>()!;
        this.navigationService = navigationService ?? Locator.Current.GetService<NavigationService>()!;
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        var canIssueCommand = this.WhenAnyValue(vm => vm.pdfJobService.IsWorking).Select(v => !v);
        GoBack = ReactiveCommand.CreateFromObservable<Unit, IRoutableViewModel?>(_ => this.navigationService.GoBack.Execute(), canIssueCommand);

        SaveXlsx = ReactiveCommand.CreateFromTask(ExportToExcelImpl, canIssueCommand);

        this.WhenActivated(d =>
        {
            this.pdfJobService.RunAllJobs();
            Progress = this.WhenAnyValue(vm => vm.pdfJobService.Percentage);

            this.pdfJobService.PdfJobs.Connect().AutoRefresh().Filter(pdfJob => pdfJob.ToBeWork == true)
                .ObserveOnDispatcher()
                .Bind(PdfJobs)
                .Subscribe()
                .DisposeWith(d);

            this.WhenAnyValue(vm => vm.SelectedPDFJob)
                .WhereNotNull()
                .Select(job => this.pdfJobService.ExtractedTextLinesCollection[job.Id])
                .ToPropertyEx(this, x => x.SelectedPDFExtractedTextLines)
                .DisposeWith(d);

            this.WhenAnyValue(vm => vm.SelectedPDFJob)
                .WhereNotNull()
                .Select(job => this.pdfJobService.ExtractionGroupsCollection[job.Id])
                .ToPropertyEx(this, x => x.SelectedPDFExtractionGroups)
                .DisposeWith(d);

            this.WhenAnyValue(vm => vm.SelectedPDFJob)
                .WhereNotNull()
                .Select(job => this.pdfJobService.People[job.Id])
                .ToPropertyEx(this, x => x.SelectedPDFPerson)
                .DisposeWith(d);
        });
    }

    private async Task ExportToExcelImpl()
    {
        VistaSaveFileDialog dialog = new()
        {
            Filter = "xlsx|*.xlsx",
            AddExtension = true,
            OverwritePrompt = true,
            DefaultExt = ".xlsx"
        };
        if (dialog.ShowDialog() ?? false)
        {
            var filePath =dialog.FileName;
            await Task.Run(async ()=>
                await ExcelUtilities.WriteExcel(filePath, this.pdfJobService.People.Values.ToList())
            );
        }

    }
}
