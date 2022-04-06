using InBodyPDFExtractor.Models;
using InBodyPDFExtractor.ViewModels;
using ReactiveUI;
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
            this.BindCommand(ViewModel, vm => vm.ExtractPDFs, v => v.Button_ExtractPDF).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.SelectedFolderPart, v => v.TextBox_FolderPath.Text).DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.PdfJobs, v => v.ListView_FilesList.ItemsSource).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFJob, v => v.ListView_FilesList.SelectedItem).DisposeWith(d);

            this.Bind(ViewModel, vm => vm.SelectedPDFJob.Person.Name, v => v.TextBox_Name.Text,  ).DisposeWith(d);

            ViewModel.WhenAnyValue(x => x.SelectedPDFJob)
                .WhereNotNull()
                .Select(pdfjob => pdfjob.TextLines)
                .BindTo(this, x => x.ListView_DataList.ItemsSource)
                .DisposeWith(d);

            ViewModel.WhenAnyValue(x => x.SelectedPDFJob)
                .WhereNotNull()
                .Subscribe(pdfJob =>
                {
                    var loadedDocument = new PdfLoadedDocument(pdfJob.AbsolutePath);
                    var page = loadedDocument.Pages[0] as PdfLoadedPage;
                    var bursh = new PdfSolidBrush(new PdfColor(0.5f));
                    pdfJob.TextLines
                    page.Graphics.DrawRectangle(,);
                    PdfViewer.Load(loadedDocument);
                    PdfViewer.
                })
                .DisposeWith(d);
        });
    }
}
