using Sharpnado.Presentation.Forms.RenderedViews;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Sharpnado.Presentation.Forms.iOS.Helpers
{
    public class iOSPlatformHelper : PlatformHelper
    {
        public override int DpToPixels(int dp, Rounding rounding = Rounding.Round) => dp;

        public override int DpToPixels(double dp, Rounding rounding = Rounding.Round) => (int)dp;

        public override double PixelsToDp(double pixels) => pixels;

        public override string DumpNativeViewHierarchy(View formsView, bool verbose)
        {
            var renderer = Platform.GetRenderer(formsView);
            Platform.SetRenderer(formsView, renderer);
            return renderer.NativeView.DumpHierarchy(verbose);
        }

        public override string DumpNativeViewInfo(View formsView)
        {
            var renderer = Platform.GetRenderer(formsView);
            Platform.SetRenderer(formsView, renderer);
            return renderer.NativeView.DumpInfo();
        }
    }
}