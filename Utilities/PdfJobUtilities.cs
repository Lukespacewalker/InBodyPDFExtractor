using InBodyPDFExtractor.Models;
using Syncfusion.Pdf;
using Syncfusion.Windows.PdfViewer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InBodyPDFExtractor.Extensions;

namespace InBodyPDFExtractor.Utilities;

internal static class PdfJobUtilities
{
    /// <summary>
    /// *This fuction is required to run on UI Thread*
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    public static List<TextLine> ExtractTextLineFromPDF(PdfJob job)
    {
        PdfDocumentView pdfDocumentView = new PdfDocumentView();
        pdfDocumentView.Load(job.AbsolutePath);
        pdfDocumentView.ExtractText(0, out TextLines textLines);
        return textLines.ToList();
    }

    private static readonly Regex ageRegex = new Regex(@"(?<Age>\d+\.\d+)years", RegexOptions.Compiled);
    private static readonly Regex idRegex = new Regex(@".*\((?<Id>.*)\).*", RegexOptions.Compiled);

    public static List<ExtractionGroup> ExtractDataFromTextLines(IList<TextLine> textLines)
    {
        List<ExtractionGroup> extractionGroups = new();

        int? nameRegionStartIndex = null;
        int? nameRegionEndIndex = null;
        bool gotName = false;
        bool gotAge = false;
        for (var index = 0; index < textLines.Count; index++)
        {
            string currentLine = textLines[index].Text;
            RectangleF currentBound = textLines[index].Bounds;

            if (nameRegionStartIndex is null)
                if (string.Equals(currentLine, "TIME"))
                    nameRegionStartIndex = index + 1;
            if (!gotAge)
            {
                if (ageRegex.IsMatch(currentLine, out Match match))
                {
                    nameRegionEndIndex = index - 1;
                    extractionGroups.Add(new ExtractionGroup<double>(nameof(Person.Age),
                        double.Parse(match.Groups["Age"].Value),
                        new IdBound(index, currentBound)));
                    gotAge = true;
                }
            }
            if (!gotName)
            {
                if (nameRegionStartIndex is not null && nameRegionEndIndex is not null)
                {
                    /**
                    var textLineArray = textLines.Take(new Range(new Index(nameRegionStartIndex.Value), new Index(nameRegionEndIndex.Value + 1)));
                    // Try to find ID from each textLine
                    foreach (var textLine in textLineArray)
                    {
                        if (idRegex.IsMatch(textLine.Text, out Match match))
                        {
                            extractionGroups.Add(new ExtractionGroup<double>(nameof(Age), double.Parse(match.Groups["Id"].Value), new IdBound(index, textLine.Bounds)));
                        }
                    }
                    */
                    //string nameAndId = string.Concat(textLineArray.Select(t => t.Text));
                    //var idBounds = textLineArray.Select((t, i) => new IdBound(nameRegionStartIndex.Value + i, t.Bounds));
                    List<IdBound> idBounds = new List<IdBound>();
                    StringBuilder nameBuilder = new StringBuilder();
                    for (var nameIndex = nameRegionStartIndex.Value; nameIndex <= nameRegionEndIndex; nameIndex++)
                    {
                        var textLine = textLines[nameIndex];
                        if (idRegex.IsMatch(textLine.Text, out Match match))
                        {
                            string id = match.Groups["Id"].Value;
                            extractionGroups.Add(new ExtractionGroup<string>(nameof(Person.Id), id, new IdBound(index, textLine.Bounds)));
                            string newText = textLine.Text.Replace($"({id})", "");

                            if (newText.Length == 0) { continue; }
                            else
                            {
                                nameBuilder.Append(newText);
                                idBounds.Add(new IdBound(nameIndex, textLine.Bounds));
                            }
                        }
                        else
                        {
                            nameBuilder.Append(textLine.Text);
                            idBounds.Add(new IdBound(nameIndex, textLine.Bounds));
                        }
                    }
                    var name = nameBuilder.ToString().Trim();
                    extractionGroups.Add(new ExtractionGroup<string>(nameof(Person.Name), name, idBounds));
                    gotName = true;
                }
            }
        }
        return extractionGroups;
    }
}
