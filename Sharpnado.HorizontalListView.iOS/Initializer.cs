using Sharpnado.HorizontalListView.iOS.Helpers;
using Sharpnado.HorizontalListView.iOS.Renderers.HorizontalList;
using Sharpnado.HorizontalListView.RenderedViews;

namespace Sharpnado.HorizontalListView.iOS
{
    public static class SharpnadoInitializer
    {
        public static void Initialize(bool enableInternalLogger = false, bool enableInternalDebugLogger = false)
        {
            InternalLogger.EnableLogger(enableInternalLogger, enableInternalDebugLogger);
            PlatformHelper.InitializeSingleton(new iOSPlatformHelper());
            iOSHorizontalListViewRenderer.Initialize();
        }
    }
}