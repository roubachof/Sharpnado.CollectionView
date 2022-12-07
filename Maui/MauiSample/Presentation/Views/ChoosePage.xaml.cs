using System.Collections.ObjectModel;

using MauiSample.Presentation.Navigables;
using MauiSample.Presentation.Navigables.Impl;
using MauiSample.Presentation.ViewModels;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

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
            On<iOS>().SetUseSafeArea(false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Padding = new Thickness(0);
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