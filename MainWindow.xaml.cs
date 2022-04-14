using InBodyPDFExtractor.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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

namespace InBodyPDFExtractor;

public class MainWindowBase : ReactiveWindow<MainViewModel> { }

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
/// 
public partial class MainWindow : MainWindowBase
{
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainViewModel();

        this.WhenActivated(d =>
        {
            this.OneWayBind(ViewModel, x => x.Router, x => x.RoutedViewHost.Router).DisposeWith(d);
            ViewModel.GoNext.Execute().Subscribe().DisposeWith(d);
        });
        
        Loaded += (sender, args) =>
        {
            WPFUI.Appearance.Watcher.Watch(
              this,                           // Window class
              WPFUI.Appearance.BackgroundType.Mica, // Background type
              true                            // Whether to change accents automatically
            );
        };
        
    }
}
