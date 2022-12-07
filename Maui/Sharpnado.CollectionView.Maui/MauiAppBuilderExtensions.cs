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
            handlers.AddHandler<CollectionView, Sharpnado.CollectionView.Droid.Renderers.CollectionViewRenderer>();
#elif __IOS__
            handlers.AddHandler<CollectionView, Sharpnado.CollectionView.iOS.Renderers.CollectionViewRenderer>();
#endif
        });

#if __ANDROID__
        Sharpnado.CollectionView.Droid.Initializer.Initialize(loggerEnable, debugLogEnable);
#elif __IOS__
        Sharpnado.CollectionView.iOS.Initializer.Initialize(loggerEnable, debugLogEnable);
#endif

        return builder;
    }
}
