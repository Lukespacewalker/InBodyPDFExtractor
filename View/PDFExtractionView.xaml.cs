using InBodyPDFExtractor.Models;
using InBodyPDFExtractor.Services;
using InBodyPDFExtractor.ViewModels;
using ReactiveUI;
using Splat;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InBodyPDFExtractor.View;

/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
/// 
public abstract class PDFExtractionViewBase : ReactiveUserControl<PDFExtractionViewModel> { }
public partial class PDFExtractionView : PDFExtractionViewBase
{
    public PDFExtractionView()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            this.BindCommand(ViewModel, vm => vm.GoBack, v => v.Button_Back).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.SaveXlsx, v => v.Button_ExportToExcel).DisposeWith(d);

            ViewModel!.Progress!.BindTo(this, v=>v.Progress_PDFExtraction.Value).DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.PdfJobs, v => v.ListView_FilesList.ItemsSource).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFJob, v => v.ListView_FilesList.SelectedItem).DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.SelectedPDFExtractedTextLines, v => v.ListView_DataList.ItemsSource).DisposeWith(d);

            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.Id, v => v.TextBox_Id.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.Name, v => v.TextBox_Name.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.Age, v => v.TextBox_Age.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.Height, v => v.TextBox_Height.Text).DisposeWith(d);

            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.Weight.Value, v => v.TextBox_Weight.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.Weight.Lower, v => v.TextBox_LowerWeight.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.Weight.Upper, v => v.TextBox_UpperWeight.Text).DisposeWith(d);


            // Body Composition binding
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.SkeletalMuscleMass.Value, v => v.TextBox_SkeletalMuscleMass.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.SkeletalMuscleMass.Lower, v => v.TextBox_LowerSkeletalMuscleMass.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.SkeletalMuscleMass.Upper, v => v.TextBox_UpperSkeletalMuscleMass.Text).DisposeWith(d);

            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.BodyFatMass.Value, v => v.TextBox_BodyFatMass.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.BodyFatMass.Lower, v => v.TextBox_LowerBodyFatMass.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.BodyFatMass.Upper, v => v.TextBox_UpperBodyFatMass.Text).DisposeWith(d);

            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.TotalBodyWater.Value, v => v.TextBox_TotalBodyWater.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.TotalBodyWater.Lower, v => v.TextBox_LowerTotalBodyWater.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.TotalBodyWater.Upper, v => v.TextBox_UpperTotalBodyWater.Text).DisposeWith(d);

            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.FatFreeMass.Value, v => v.TextBox_FatFreeMass.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.FatFreeMass.Lower, v => v.TextBox_LowerFatFreeMass.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.FatFreeMass.Upper, v => v.TextBox_UpperFatFreeMass.Text).DisposeWith(d);

            // Obesity Diagnosis binding
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.BodyMassIndex.Value, v => v.TextBox_BodyMassIndex.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.BodyMassIndex.Lower, v => v.TextBox_LowerBodyMassIndex.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.BodyMassIndex.Upper, v => v.TextBox_UpperBodyMassIndex.Text).DisposeWith(d);

            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.PercentBodyFatIndex.Value, v => v.TextBox_PercentBodyFat.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.PercentBodyFatIndex.Lower, v => v.TextBox_LowerPercentBodyFat.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.PercentBodyFatIndex.Upper, v => v.TextBox_UpperPercentBodyFat.Text).DisposeWith(d);

            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.WaistHipRatioIndex.Value, v => v.TextBox_WaistHipRatio.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.WaistHipRatioIndex.Lower, v => v.TextBox_LowerWaistHipRatio.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.WaistHipRatioIndex.Upper, v => v.TextBox_UpperWaistHipRatio.Text).DisposeWith(d);

            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.BasalMetabolicRatioIndex.Value, v => v.TextBox_BasalMetabolicRatio.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.BasalMetabolicRatioIndex.Lower, v => v.TextBox_LowerBasalMetabolicRatio.Text).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFPerson.BasalMetabolicRatioIndex.Upper, v => v.TextBox_UpperBasalMetabolicRatio.Text).DisposeWith(d);


            ViewModel.WhenAnyValue(x => x.SelectedPDFJob)
                .WhereNotNull()
                .DistinctUntilChanged()
                .Subscribe(pdfJob =>
                {
                    ViewModel!.IsPDFViewerLoading = true;
                    var service = Locator.Current.GetService<PdfJobService>();
                    var loadedDocument = new PdfLoadedDocument(pdfJob.AbsolutePath);
                    var page = loadedDocument.Pages[0] as PdfLoadedPage;
                    var bursh = new PdfPen(new PdfColor(System.Drawing.Color.FromArgb(30,30,30,30)));
                    var exGroups = service.ExtractionGroupsCollection[pdfJob.Id];
                    foreach (var exGroup in exGroups)
                    {
                        foreach (var idBound in exGroup.Bounds)
                        {
                            page.Graphics.DrawRectangle(bursh, idBound.Bound);
                        }
                    }
                    PdfViewer.Load(loadedDocument);
                    ViewModel!.IsPDFViewerLoading = false;
                })
                .DisposeWith(d);
        });
    }
}
