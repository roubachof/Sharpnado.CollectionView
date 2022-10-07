using MauiSample.Domain.Silly;
using MauiSample.Infrastructure;
using MauiSample.Presentation.Navigables;
using MauiSample.Presentation.Navigables.Impl;
using MauiSample.Presentation.ViewModels;
using MauiSample.Presentation.Views;

namespace MauiSample;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        var navigationService = new FormsNavigationService(
            new Lazy<NavigationPage>(() => (NavigationPage)Current.MainPage),
            new ViewLocator());

        DependencyContainer.Instance.Options.EnableAutoVerification = false;

        var errorEmulator = new ErrorEmulator();
        DependencyContainer.Instance.Register<ISillyDudeService>(() => new SillyDudeService(errorEmulator));
        DependencyContainer.Instance.Register<INavigationService>(() => navigationService);

        DependencyContainer.Instance.Register<GridPageViewModel>();
        DependencyContainer.Instance.Register<GridPage>();

        DependencyContainer.Instance.Register<HeaderFooterGroupingPageViewModel>();
        DependencyContainer.Instance.Register<HeaderFooterGroupingPage>();

        MainPage = new NavigationPage(new ChoosePage(navigationService) { BindingContext = new ChoosePageViewModel() });
	}
}
