using Android.Content.Res;
using Sharpnado.Presentation.Forms.RenderedViews;

namespace Sharpnado.Presentation.Forms.Droid.Helpers
{
    public class AndroidPlatformHelper : PlatformHelper
    {
        public override int DpToPixels(int dp)
        {
            return (int)(dp * Resources.System.DisplayMetrics.Density);
        }

        public override int DpToPixels(double dp)
        {
            return (int)(dp * Resources.System.DisplayMetrics.Density);
        }

        public override int PixelsToDp(int pixels)
        {
            return (int)(pixels / Resources.System.DisplayMetrics.Density);
        }
    }
}