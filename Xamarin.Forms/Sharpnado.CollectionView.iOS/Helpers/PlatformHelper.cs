#if NET6_0_OR_GREATER
using Microsoft.Maui.Controls.PlatformConfiguration;
#endif

using Sharpnado.CollectionView;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Sharpnado.CollectionView.iOS.Helpers
{
    public class iOSPlatformHelper : PlatformHelper
    {
        public override int DpToPixels(int dp, Rounding rounding = Rounding.Round) => dp;

        public override int DpToPixels(double dp, Rounding rounding = Rounding.Round) => (int)dp;

        public override double PixelsToDp(double pixels) => pixels;

        public override string DumpNativeViewHierarchy(View formsView, bool verbose)
        {
#if NET6_0_OR_GREATER
            if (formsView.Handler?.PlatformView is not UIView nativeView)
            {
                return "platform view is null";
            }

            return nativeView.DumpHierarchy(verbose) ?? "null renderer";
#else
            var renderer = Platform.GetRenderer(formsView);
            Platform.SetRenderer(formsView, renderer);
            return renderer.NativeView.DumpHierarchy(verbose);
#endif
        }

        public override string DumpNativeViewInfo(View formsView)
        {
#if NET6_0_OR_GREATER
            if (formsView.Handler?.PlatformView is not UIView nativeView)
            {
                return "platform view is null";
            }

            return nativeView.DumpInfo() ?? "null renderer";
#else
            var renderer = Platform.GetRenderer(formsView);
            Platform.SetRenderer(formsView, renderer);
            return renderer.NativeView.DumpInfo();
#endif
        }
    }
}