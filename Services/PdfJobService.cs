using DynamicData;
using InBodyPDFExtractor.Models;
using InBodyPDFExtractor.Utilities;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace InBodyPDFExtractor.Services;

internal class PdfJobService
{
    public SourceCache<PdfJob, int> PdfJobs { get; private set; } = new(job => job.Id);
    public Dictionary<int, Person> People { get; private set; } = new();
    public Dictionary<int, List<ExtractionGroup>> ExtractionGroupsCollection { get; private set; } = new();
    public Dictionary<int, List<TextLine>> ExtractedTextLinesCollection { get; private set; } = new();


    public async Task RunAllJobs()
    {
        //return Task.Run(async () =>
        //{
        foreach (var pdfJob in PdfJobs.Items)
        {
            await RunJob(pdfJob);
        }
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
            await Task.Yield();
        }
        catch (Exception)
        {
            pdfJob.JobStatus = JobStatus.Error;
        }
    }
}
