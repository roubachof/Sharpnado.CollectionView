using MauiSample.Presentation.Navigables;
using MauiSample.Presentation.Navigables.Impl;
using MauiSample.Presentation.ViewModels;

namespace MauiSample.Presentation.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChoosePage : ContentPage, IBindablePage
    {
        private readonly FormsNavigationService _navigationService;

        public ChoosePage(FormsNavigationService navigationService)
        {
            _navigationService = navigationService;

            InitializeComponent();
        }

        private async void LayoutButton_OnClicked(object sender, EventArgs e)
        {
            await _navigationService.NavigateToAsync<GridPageViewModel>();
        }

        private async void HeaderButton_OnClicked(object sender, EventArgs e)
        {
            await _navigationService.NavigateToAsync<HeaderFooterGroupingPageViewModel>();
        }
    }
}