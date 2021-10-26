using Sharpnado.CollectionView.Droid.Helpers;
using Sharpnado.CollectionView.Droid.Renderers;
using Sharpnado.CollectionView.RenderedViews;

namespace Sharpnado.CollectionView.Droid
{
    public static class Initializer
    {
        public static void Initialize(bool enableInternalLogger = false, bool enableInternalDebugLogger = false)
        {
            InternalLogger.EnableLogger(enableInternalLogger, enableInternalDebugLogger);
            PlatformHelper.InitializeSingleton(new AndroidPlatformHelper());
            CollectionViewRenderer.Initialize();
        }
    }
}