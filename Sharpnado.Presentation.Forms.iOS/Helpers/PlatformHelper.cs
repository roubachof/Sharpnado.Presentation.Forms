using Sharpnado.Presentation.Forms.RenderedViews;

namespace Sharpnado.Presentation.Forms.iOS.Helpers
{
    public class iOSPlatformHelper : PlatformHelper
    {
        public override int DpToPixels(int dp, Rounding rounding = Rounding.Round) => dp;

        public override int DpToPixels(double dp, Rounding rounding = Rounding.Round) => (int)dp;

        public override double PixelsToDp(double pixels) => pixels;
    }
}