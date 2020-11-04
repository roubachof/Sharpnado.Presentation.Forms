using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Sharpnado.Presentation.Forms.Droid;

namespace Sharpnado.DragAndDropCollection.Sample.Android
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
            
            SharpnadoInitializer.Initialize();
            
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            
            
            
            LoadApplication(new App());
        }
    }
}