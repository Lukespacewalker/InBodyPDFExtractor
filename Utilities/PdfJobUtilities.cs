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

    private static readonly Regex ageRegex = new Regex(@"(?<Age>[\d.]+)years", RegexOptions.Compiled);
    private static readonly Regex idRegex = new Regex(@".*\((?<Id>.*)\).*", RegexOptions.Compiled);
    private static readonly Regex cmRegex = new Regex(@"(?<Number>[\d.]+).*cm", RegexOptions.Compiled);
    private static readonly Regex kgRegex = new Regex(@"(?<Number>[\d.]+).*kg", RegexOptions.Compiled);
    private static readonly Regex numberRegex = new Regex(@"(?<Number>[\d.]+)", RegexOptions.Compiled);
    private static readonly Regex upperRangeRegex = new Regex(@"~\s*(?<Number>[\d.]+)", RegexOptions.Compiled);
    private static readonly Regex dateRegex = new Regex(@"(?<Year>\d+)\/(?<Month>\d+)\/(?<Day>\d+)", RegexOptions.Compiled);
    private static readonly Regex timeRegex = new Regex(@"(?<Hour>[\d]):(?<Minute>[\d]):(?<Second>[\d])", RegexOptions.Compiled);

    private static readonly Regex obesityDiagnosisLowerRangeRegex = new Regex(@"(?<Number>[\d.]+)$", RegexOptions.Compiled);

    public static List<ExtractionGroup> ExtractDataFromTextLines(IList<TextLine> textLines)
    {
        List<ExtractionGroup> extractionGroups = new();

        var tryAddNumData = (int index, string propertyName, Regex regex) =>
        {
            var text = textLines[index].Text.Trim();
            if (regex.IsMatch(text, out Match match))
            {
                extractionGroups.Add(new ExtractionGroup<double>(propertyName,
                double.Parse(match.Groups["Number"].Value),
                new IdBound(index, textLines[index].Bounds)));
            }
        };

        var tryAddNumDataWithNormalRangeBase = (int index, int lowerIndex, int upperIndex, string propertyName, Regex normalRegex, Regex lowerRegex) =>
        {
            var valText = textLines[index].Text.Trim();
            var lowerText = textLines[lowerIndex].Text.Trim();
            var upperText = textLines[upperIndex].Text.Trim();
            if (normalRegex.IsMatch(valText, out Match match))
            {
                List<IdBound> idBoundList = new List<IdBound>();
                idBoundList.Add(new IdBound(index, textLines[index].Bounds));
                var val = double.Parse(match.Groups["Number"].Value);
                double? lowerVal = null, upperVal = null;
                if (lowerRegex.IsMatch(lowerText, out Match lowerMatch))
                {
                    lowerVal = double.Parse(lowerMatch.Groups["Number"].Value);
                    idBoundList.Add(new IdBound(lowerIndex, textLines[lowerIndex].Bounds));
                }
                if (upperRangeRegex.IsMatch(upperText, out Match upperMatch))
                {
                    upperVal = double.Parse(upperMatch.Groups["Number"].Value);
                    idBoundList.Add(new IdBound(upperIndex, textLines[upperIndex].Bounds));
                }
                extractionGroups.Add(new ExtractionGroup<DataWithNormalRange>(propertyName,
                new DataWithNormalRange(val, lowerVal, upperVal), idBoundList));
            }
        };

        var tryAddNumDataWithNormalRange = (int index, int lowerIndex, int upperIndex, string propertyName, Regex normalRegex) =>
        {
            tryAddNumDataWithNormalRangeBase(index, lowerIndex, upperIndex, propertyName, normalRegex, numberRegex);
        };



        var tryAddGender = (int index) =>
        {
            var text = textLines[index].Text.Trim();
            try
            {
                Gender gender = Enum.Parse<Gender>(text);
                extractionGroups.Add(new ExtractionGroup<Gender>(nameof(Person.Gender), gender, new IdBound(index, textLines[index].Bounds)));
            }
            catch (Exception) { }
        };

        int? nameRegionStartIndex = null;
        int? nameRegionEndIndex = null;
        int? ageIndex = null;
        int? bodyCompositionIndex = null;
        int? obesityDiagnosisIndex = null;

        bool gotName = false;
        bool gotAge = false;
        for (var index = 0; index < textLines.Count; index++)
        {
            string currentLine = textLines[index].Text.Trim();
            RectangleF currentBound = textLines[index].Bounds;

            if (nameRegionStartIndex is null)
                if (string.Equals(currentLine, "TIME"))
                    nameRegionStartIndex = index + 1;
            if (bodyCompositionIndex is null)
            {
                if (string.Equals(currentLine, "Body Composition"))
                    bodyCompositionIndex = index;
            }
            if (obesityDiagnosisIndex is null)
            {
                if (string.Equals(currentLine, "Obesity Diagnosis"))
                    obesityDiagnosisIndex = index;
            }
            if (!gotAge)
            {
                if (ageRegex.IsMatch(currentLine, out Match match))
                {
                    nameRegionEndIndex = index - 1;
                    extractionGroups.Add(new ExtractionGroup<double>(nameof(Person.Age),
                        double.Parse(match.Groups["Age"].Value),
                        new IdBound(index, currentBound)));
                    ageIndex = index;
                    gotAge = true;
                }
            }
            if (!gotName)
            {
                if (nameRegionStartIndex is not null && nameRegionEndIndex is not null)
                {
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
        if (ageIndex is not null)
        {
            int heightIndex = ageIndex.Value + 1;
            tryAddNumData(heightIndex, nameof(Person.Height), cmRegex);
            int genderIndex = ageIndex.Value + 2;
            tryAddGender(genderIndex);
            int dateOnlyIndex = ageIndex.Value + 3;
            int timeOnlyIndex = ageIndex.Value + 4;
            var dateText = textLines[dateOnlyIndex].Text.Trim();
            var timeText = textLines[timeOnlyIndex].Text.Trim();
            if (dateRegex.IsMatch(dateText, out Match match))
            {
                var year = int.Parse(match.Groups["Year"].Value);
                var month = int.Parse(match.Groups["Month"].Value);
                var day = int.Parse(match.Groups["Day"].Value);
                if (timeRegex.IsMatch(timeText, out Match match2))
                {
                    var h = int.Parse(match2.Groups["Hour"].Value);
                    var m = int.Parse(match2.Groups["Minute"].Value);
                    var s = int.Parse(match2.Groups["Second"].Value);
                    var dateTime = new DateTime(year, month, day, h, m, s);
                    extractionGroups.Add(new ExtractionGroup<DateTime>(nameof(Person.DateTime), dateTime, new[] { new IdBound(dateOnlyIndex, textLines[dateOnlyIndex].Bounds), new IdBound(timeOnlyIndex, textLines[timeOnlyIndex].Bounds) }));
                }
                else
                {
                    var dateOnly = new DateTime(year, month, day);
                    extractionGroups.Add(new ExtractionGroup<DateTime>(nameof(Person.DateTime), dateOnly, new IdBound(dateOnlyIndex, textLines[dateOnlyIndex].Bounds)));
                }
            }
        }
        if (bodyCompositionIndex is not null)
        {
            int weightIndex = bodyCompositionIndex.Value + 8;
            int normalWeightLowerRangeIndex = bodyCompositionIndex.Value + 21;
            int normalWeightUpperRangeIndex = bodyCompositionIndex.Value + 22;
            tryAddNumDataWithNormalRange(weightIndex, normalWeightLowerRangeIndex, normalWeightUpperRangeIndex,
                nameof(Person.Weight), kgRegex);

            int skeletionMuscleMassIndex = bodyCompositionIndex.Value + 10;
            int normalSkeletionMuscleMassLowerRangeIndex = bodyCompositionIndex.Value + 23;
            int normalSkeletionMuscleMassUpperRangeIndex = bodyCompositionIndex.Value + 24;
            tryAddNumDataWithNormalRange(skeletionMuscleMassIndex, normalSkeletionMuscleMassLowerRangeIndex, normalSkeletionMuscleMassUpperRangeIndex,
                nameof(Person.SkeletalMuscleMass), kgRegex);

            int bodyFatMassIndex = bodyCompositionIndex.Value + 12;
            int normalBodyFatMassLowerRangeIndex = bodyCompositionIndex.Value + 25;
            int normalBodyFatMassUpperRangeIndex = bodyCompositionIndex.Value + 26;
            tryAddNumDataWithNormalRange(bodyFatMassIndex, normalBodyFatMassLowerRangeIndex, normalBodyFatMassUpperRangeIndex,
    nameof(Person.BodyFatMass), kgRegex);

            int totalBodyWaterIndex = bodyCompositionIndex.Value + 29;
            int normalTotalBodyWaterLowerRangeIndex = bodyCompositionIndex.Value + 30;
            int normalTotalBodyWaterUpperRangeIndex = bodyCompositionIndex.Value + 31;
            tryAddNumDataWithNormalRange(totalBodyWaterIndex, normalTotalBodyWaterLowerRangeIndex, normalTotalBodyWaterUpperRangeIndex,
    nameof(Person.TotalBodyWater), kgRegex);

            int fatFreeMassIndex = bodyCompositionIndex.Value + 34;
            int normalFatFreeMassLowerRangeIndex = bodyCompositionIndex.Value + 35;
            int normalFatFreeMassUpperRangeIndex = bodyCompositionIndex.Value + 36;
            tryAddNumDataWithNormalRange(fatFreeMassIndex, normalFatFreeMassLowerRangeIndex, normalFatFreeMassUpperRangeIndex,
    nameof(Person.FatFreeMass), kgRegex);
        }
        if(obesityDiagnosisIndex is not null)
        {
            int bmiIndex = obesityDiagnosisIndex.Value + 4;
            int normalBmiLowerRangeIndex = obesityDiagnosisIndex.Value + 4;
            int normalBmiUpperRangeIndex = obesityDiagnosisIndex.Value + 5;
            tryAddNumDataWithNormalRangeBase(bmiIndex, normalBmiLowerRangeIndex, normalBmiUpperRangeIndex,
nameof(Person.BodyMassIndex), numberRegex, obesityDiagnosisLowerRangeRegex);

            int percentBodyFatIndex = obesityDiagnosisIndex.Value + 8;
            int normalpercentBodyFatLowerRangeIndex = obesityDiagnosisIndex.Value + 8;
            int normalpercentBodyFatUpperRangeIndex = obesityDiagnosisIndex.Value + 9;
            tryAddNumDataWithNormalRangeBase(percentBodyFatIndex, normalpercentBodyFatLowerRangeIndex, normalpercentBodyFatUpperRangeIndex,
nameof(Person.PercentBodyFatIndex), numberRegex, obesityDiagnosisLowerRangeRegex);

            int waistHipRatioIndex = obesityDiagnosisIndex.Value + 12;
            int normalWaistHipRatioLowerRangeIndex = obesityDiagnosisIndex.Value + 12;
            int normalWaistHipRatioUpperRangeIndex = obesityDiagnosisIndex.Value + 13;
            tryAddNumDataWithNormalRangeBase(waistHipRatioIndex, normalWaistHipRatioLowerRangeIndex, normalWaistHipRatioUpperRangeIndex,
nameof(Person.WaistHipRatioIndex), numberRegex, obesityDiagnosisLowerRangeRegex);

            int basalMetabolicRatioIndex = obesityDiagnosisIndex.Value + 16;
            int normalBasalMetabolicRatioLowerRangeIndex = obesityDiagnosisIndex.Value + 16;
            int normalBasalMetabolicRatiooUpperRangeIndex = obesityDiagnosisIndex.Value + 17;
            tryAddNumDataWithNormalRangeBase(basalMetabolicRatioIndex, normalBasalMetabolicRatioLowerRangeIndex, normalBasalMetabolicRatiooUpperRangeIndex,
nameof(Person.BasalMetabolicRatioIndex), numberRegex, obesityDiagnosisLowerRangeRegex);
        }

        return extractionGroups;
    }
}
