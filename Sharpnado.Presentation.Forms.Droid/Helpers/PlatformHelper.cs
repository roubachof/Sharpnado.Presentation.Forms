using Android.Content.Res;

namespace Sharpnado.Presentation.Forms.Droid.Helpers
{
    public class PlatformHelper
    {
        public static int DpToPixels(int dp)
        {
            return (int)(dp * Resources.System.DisplayMetrics.Density);
        }
    }
}