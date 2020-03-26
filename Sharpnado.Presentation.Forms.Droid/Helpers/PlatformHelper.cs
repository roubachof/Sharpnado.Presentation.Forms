using System;

using Android.Content.Res;
using Sharpnado.Presentation.Forms.RenderedViews;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Sharpnado.Presentation.Forms.Droid.Helpers
{
    public class AndroidPlatformHelper : PlatformHelper
    {
        public override int DpToPixels(int dp, Rounding rounding = Rounding.Round) => DpToPixels((double)dp, rounding);

        public override int DpToPixels(double dp, Rounding rounding = Rounding.Round)
        {
            switch (rounding)
            {
                case Rounding.Round:
                    return (int)Math.Round(dp * Resources.System.DisplayMetrics.Density);
                case Rounding.Floor:
                    return (int)Math.Floor(dp * Resources.System.DisplayMetrics.Density);
                default:
                    return (int)Math.Ceiling(dp * Resources.System.DisplayMetrics.Density);
            }
        }

        public override double PixelsToDp(double pixels)
        {
            return Math.Floor((pixels / Resources.System.DisplayMetrics.Density) * 100) / 100;
        }

        public override string DumpNativeViewHierarchy(View formsView, bool verbose)
        {
            var renderer = Platform.GetRenderer(formsView);
            Platform.SetRenderer(formsView, renderer);
            return renderer.View.DumpHierarchy(verbose);
        }

        public override string DumpNativeViewInfo(View formsView)
        {
            var renderer = Platform.GetRenderer(formsView);
            Platform.SetRenderer(formsView, renderer);
            return renderer.View.DumpInfo();
        }
    }
}