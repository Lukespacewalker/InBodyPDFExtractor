using Syncfusion.Pdf;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Reflection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace InBodyPDFExtractor.Models;

internal class PdfJob : ReactiveObject
{
    public int Id { get; set; }
    [Reactive] public JobStatus JobStatus { get; set; } = JobStatus.NotStart;
    public string FileName { get; set; } = string.Empty;
    public string AbsolutePath { get; set; } = string.Empty;
    //public Person? Person { get; set; }
    //public List<TextLine> TextLines { get; set; } = new List<TextLine>();
}

internal enum JobStatus
{
    NotStart, Running, Finish, Error
}