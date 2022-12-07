using MauiSample.Presentation.Navigables;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace MauiSample.Presentation.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeaderFooterGroupingPage : ContentPage, IBindablePage
    {
        public HeaderFooterGroupingPage()
        {
            InitializeComponent();
            On<iOS>().SetUseSafeArea(false);
        }
    }
}