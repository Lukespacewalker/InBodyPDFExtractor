using DynamicData;
using InBodyPDFExtractor.Models;
using InBodyPDFExtractor.Utilities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace InBodyPDFExtractor.Services;

public class NavigationService
{
    public ReactiveCommand<IRoutableViewModel, IRoutableViewModel> Navigate => Router.Navigate;

    // The command that navigates a user back.
    public ReactiveCommand<Unit, IRoutableViewModel?> GoBack { get; }

    public NavigationService()
    {
        Router = new RoutingState();

        var canGoBack = this
            .WhenAnyValue(x => x.Router.NavigationStack.Count)
            .Select(count => count > 0);
        GoBack = ReactiveCommand.CreateFromObservable(
            () => Router.NavigateBack.Execute(Unit.Default),
            canGoBack);
    }

    public RoutingState Router { get; }
}

internal class PdfJobService : ReactiveObject
{
    public SourceCache<PdfJob, int> PdfJobs { get; private set; } = new(job => job.Id);
    public Dictionary<int, Person> People { get; private set; } = new();
    public Dictionary<int, List<ExtractionGroup>> ExtractionGroupsCollection { get; private set; } = new();
    public Dictionary<int, List<TextLine>> ExtractedTextLinesCollection { get; private set; } = new();

    [Reactive] public bool IsWorking { get; private set; } = false;

    [Reactive] public double Percentage { get; private set; } = 0;

    public void ClearAllJobs()
    {
        PdfJobs.Clear();
        ExtractionGroupsCollection.Clear();
        ExtractedTextLinesCollection.Clear();
    }


    public async Task RunAllJobs()
    {
        //await Task.Run(async () =>
        //{
        foreach (var pdfJob in PdfJobs.Items.Where(pdfJobs => pdfJobs.JobStatus == JobStatus.NotStart || pdfJobs.JobStatus == JobStatus.Error))
        {
            pdfJob.JobStatus = JobStatus.Queue;
        }
        IsWorking = true;
        var queues = PdfJobs.Items.Where(pdfJobs => pdfJobs.JobStatus == JobStatus.Queue).ToArray();
        for (var i = 0; i < queues.Length; i++)
        {
            var pdfJob = queues[i];
            Percentage = (i + 1) * 100 / queues.Length;
            await RunJob(pdfJob);
        }
        Percentage = 100;
        IsWorking = false;
        //});
    }

    public async Task RunJob(PdfJob pdfJob)
    {
        var jobId = pdfJob.Id;
        List<TextLine>? textLines = null;
        try
        {
            pdfJob.JobStatus = JobStatus.Running;
            await Task.Yield();
            Dispatcher.CurrentDispatcher.Invoke(() => textLines = PdfJobUtilities.ExtractTextLineFromPDF(pdfJob));
            var extractionGroups = PdfJobUtilities.ExtractDataFromTextLines(textLines!);
            ExtractedTextLinesCollection.Add(jobId, textLines!);
            ExtractionGroupsCollection.Add(jobId, extractionGroups);
            var person = Person.CreateFromExtractionGroup(extractionGroups);
            People.Add(jobId, person);
            pdfJob.JobStatus = JobStatus.Finish;
            await Task.Delay(100);
            await Task.Yield();
        }
        catch (Exception)
        {
            pdfJob.JobStatus = JobStatus.Error;
        }
    }
}
