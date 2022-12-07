using UIKit;

namespace Sharpnado.CollectionView.iOS.Helpers
{
    public class iOSPlatformHelper : PlatformHelper
    {
        public override int DpToPixels(int dp, Rounding rounding = Rounding.Round) => dp;

        public override int DpToPixels(double dp, Rounding rounding = Rounding.Round) => (int)dp;

        public override double PixelsToDp(double pixels) => pixels;

        public override string DumpNativeViewHierarchy(View formsView, bool verbose)
        {
            if (formsView.Handler?.PlatformView is not UIView nativeView)
            {
                return "platform view is null";
            }

            return nativeView.DumpHierarchy(verbose) ?? "null renderer";
        }

        public override string DumpNativeViewInfo(View formsView)
        {
            if (formsView.Handler?.PlatformView is not UIView nativeView)
            {
                return "platform view is null";
            }

            return nativeView.DumpInfo() ?? "null renderer";
        }
    }
}