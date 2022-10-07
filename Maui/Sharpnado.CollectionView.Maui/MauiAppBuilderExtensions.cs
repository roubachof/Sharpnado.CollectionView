using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.Hosting;

namespace Sharpnado.CollectionView;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseSharpnadoCollectionView(
        this MauiAppBuilder builder,
        bool loggerEnable,
        bool debugLogEnable = false)
    {
        Initializer.Initialize(loggerEnable, debugLogEnable);

        builder.UseMauiCompatibility();
        builder.ConfigureMauiHandlers(handlers =>
        {
#if __ANDROID__
            handlers.AddCompatibilityRenderer<CollectionView, Sharpnado.CollectionView.Droid.Renderers.CollectionViewRenderer>();
#elif __IOS__
            // handlers.AddCompatibilityRenderer<CollectionView, Sharpnado.CollectionView.Droid.Renderers.CollectionViewRenderer>();
#endif
        });

#if __ANDROID__
        Sharpnado.CollectionView.Droid.Initializer.Initialize(loggerEnable, debugLogEnable);
#elif __IOS__
        // Sharpnado.Tabs.Effects.iOS.CommandsPlatform.Init();
        // Sharpnado.Tabs.Effects.iOS.TouchEffectPlatform.Init();
        // Sharpnado.Tabs.iOS.iOSTintableImageEffect.Init();
#endif

        return builder;
    }
}
