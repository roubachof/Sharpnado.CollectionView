using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DragAndDropSample.Navigables.Impl;
using DragAndDropSample.Services;
using DragAndDropSample.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DragAndDropSample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChoosePage : ContentPage
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