using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Reactive.Disposables;
using System.Reactive.Linq;
using InBodyPDFExtractor.ViewModels;
using Splat;
using Syncfusion.Pdf.Parsing;
using InBodyPDFExtractor.Services;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

namespace InBodyPDFExtractor.View;

public abstract class SelectFolderViewBase : ReactiveUserControl<SelectFolderViewModel> { }
/// <summary>
/// Interaction logic for SelectFolderView.xaml
/// </summary>
public partial class SelectFolderView : SelectFolderViewBase
{
    public SelectFolderView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            this.BindCommand(ViewModel, vm => vm.SelectFolder, v => v.Button_SelectFolder).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.SelectedFolderPath, v => v.TextBox_FolderPath.Text).DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.NavigateToPDFExtractionPage, v => v.Button_ProcessPDF).DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.PdfJobs, v => v.ListView_FilesList.ItemsSource).DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.PdfJobs.Count, v => v.TextBlock_FileCounts.Text, v=>$"{v} ไฟล์").DisposeWith(d);
            
            this.OneWayBind(ViewModel, vm => vm.IsPDFViewerLoading, v => v.ListView_FilesList.IsEnabled, v => !v).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.SelectedPDFJob, v => v.ListView_FilesList.SelectedItem).DisposeWith(d);

            ViewModel.WhenAnyValue(x => x.SelectedPDFJob)
                .WhereNotNull()
                .DistinctUntilChanged()
                .Subscribe(async pdfJob =>
                {
                    ViewModel!.IsPDFViewerLoading = true;
                    await PdfViewer.LoadAsync(pdfJob.AbsolutePath);
                    ViewModel!.IsPDFViewerLoading = false;
                })
                .DisposeWith(d);
        });
    }
}
