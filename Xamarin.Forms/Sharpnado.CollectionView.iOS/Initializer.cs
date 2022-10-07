using Sharpnado.CollectionView.iOS.Helpers;
using Sharpnado.CollectionView.iOS.Renderers;
using Sharpnado.CollectionView.RenderedViews;

namespace Sharpnado.CollectionView.iOS
{
    public static class Initializer
    {
        public static void Initialize(bool enableInternalLogger = false, bool enableInternalDebugLogger = false)
        {
            InternalLogger.EnableLogger(enableInternalLogger, enableInternalDebugLogger);
            PlatformHelper.InitializeSingleton(new iOSPlatformHelper());
            CollectionViewRenderer.Initialize();
        }
    }
}