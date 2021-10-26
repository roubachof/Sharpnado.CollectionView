using Android.App;
using Android.Content.PM;
using Android.Glide;
using Android.Graphics;
using Android.OS;

using Sharpnado.CollectionView.Droid;

namespace DragAndDropSample.Droid
{
    [Activity(Label = "Sharpnado.DragAndDropCollection.Sample", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            
            Initializer.Initialize();

            XamEffects.Droid.Effects.Init();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                var darkSurface = Color.ParseColor("#383838");
                Window.SetStatusBarColor(darkSurface);
            }

            Xamarin.Forms.Forms.SetFlags("Brush_Experimental");

            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Forms.Init(this);

            LoadApplication(new App());
        }
    }
}