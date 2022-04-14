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
public abstract class PDFSelectionViewBase : ReactiveUserControl<PDFSelectionViewModel> { }
public partial class PDFSelectionView : PDFSelectionViewBase
{
    public PDFSelectionView()
    {
        InitializeComponent();
        var p = new Person();
        this.WhenActivated(d =>
        {
            this.BindCommand(ViewModel, vm => vm.SelectFolder, v => v.Button_SelectFolder).DisposeWith(d);
           // this.BindCommand(ViewModel, vm => vm.ExtractPDFs, v => v.Button_ExtractPDF).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.SelectedFolderPart, v => v.TextBox_FolderPath.Text).DisposeWith(d);

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


            ViewModel.WhenAnyValue(x => x.SelectedPDFJob)
                .WhereNotNull()
                .DistinctUntilChanged()
                .Subscribe(pdfJob =>
                {
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
                })
                .DisposeWith(d);
        });
    }
}
