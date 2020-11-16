using System;

using DragAndDropSample.Navigables.Impl;
using DragAndDropSample.Services;
using DragAndDropSample.ViewModels;
using DragAndDropSample.Views;

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

            Sharpnado.HorizontalListView.Initializer.Initialize(true, true);
            Sharpnado.Tabs.Initializer.Initialize(true, true);
            Sharpnado.Shades.Initializer.Initialize(loggerEnable: true, true);

            var navigationService = new FormsNavigationService(
                new Lazy<NavigationPage>(() => (NavigationPage)Current.MainPage),
                new ViewLocator());

            var sillyDudeService = new SillyDudeService(new ErrorEmulator());

            var viewModel = new GridPageViewModel(navigationService, sillyDudeService);

            MainPage = new NavigationPage(new GridPage { BindingContext = viewModel });
            viewModel.Load(null);
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