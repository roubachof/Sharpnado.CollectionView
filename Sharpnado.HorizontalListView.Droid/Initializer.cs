using Sharpnado.HorizontalListView.Droid.Helpers;
using Sharpnado.HorizontalListView.Droid.Renderers.HorizontalList;
using Sharpnado.HorizontalListView.RenderedViews;

namespace Sharpnado.HorizontalListView.Droid
{
    public static class SharpnadoInitializer
    {
        public static void Initialize(bool enableInternalLogger = false, bool enableInternalDebugLogger = false)
        {
            InternalLogger.EnableLogger(enableInternalLogger, enableInternalDebugLogger);
            PlatformHelper.InitializeSingleton(new AndroidPlatformHelper());
            AndroidHorizontalListViewRenderer.Initialize();
        }
    }
}