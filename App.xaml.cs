using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace InBodyPDFExtractor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjA1ODE4QDMyMzAyZTMxMmUzMFBYS29kVUwyTUo1Q2JBSHR0RVBjODg5czlrQUs2ZkZKcXYzYWRZZUtlNUE9");
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
            Locator.CurrentMutable.RegisterConstant(new Services.PdfJobService());
            Locator.CurrentMutable.RegisterConstant(new Services.NavigationService());
        }
    }
}
