using Sharpnado.Presentation.Forms.RenderedViews;

namespace Sharpnado.Presentation.Forms.iOS.Helpers
{
    public class iOSPlatformHelper : PlatformHelper
    {
        public override int DpToPixels(int dp) => dp;

        public override int DpToPixels(double dp) => (int)dp;

        public override int PixelsToDp(int pixels) => pixels;
    }
}