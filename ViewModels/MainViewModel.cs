using InBodyPDFExtractor.Services;
using InBodyPDFExtractor.View;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InBodyPDFExtractor.ViewModels;

public class MainViewModel : ReactiveObject, IScreen
{
    private readonly NavigationService? navigationService;

    public RoutingState Router => navigationService!.Router;

    public ReactiveCommand<Unit, IRoutableViewModel> NavigateToSelectFolderPage { get; }

    public MainViewModel(NavigationService? navigationService = null)
    {
        this.navigationService = navigationService ?? Locator.Current.GetService<NavigationService>();

        NavigateToSelectFolderPage = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute(new SelectFolderViewModel()));
    }
}
