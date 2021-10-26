using System;

using DragAndDropSample.Navigables;
using DragAndDropSample.Navigables.Impl;
using DragAndDropSample.Services;
using DragAndDropSample.ViewModels;
using DragAndDropSample.Views;

using Sharpnado.CollectionView;

using SimpleInjector;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

[assembly: ExportFont("ka1.ttf", Alias = "KarmaticFont")]

namespace DragAndDropSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Initializer.Initialize(true, true);
            Sharpnado.Tabs.Initializer.Initialize(true, true);
            Sharpnado.Shades.Initializer.Initialize(loggerEnable: false);

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

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}