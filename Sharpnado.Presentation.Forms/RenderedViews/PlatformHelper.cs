using System;

using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.RenderedViews
{
    public abstract class PlatformHelper
    {
        private static PlatformHelper _instance;

        public enum Rounding
        {
            Round = 0,
            Floor = 1,
            Ceil = 2,
        }

        public static PlatformHelper Instance => _instance;

        public static void InitializeSingleton(PlatformHelper instance)
        {
            _instance = instance;
        }

        public abstract int DpToPixels(int dp, Rounding rounding = Rounding.Round);

        public abstract int DpToPixels(double dp, Rounding rounding = Rounding.Round);

        public abstract double PixelsToDp(double pixels);

        public abstract string DumpNativeViewHierarchy(View formsView, bool verbose);

        public abstract string DumpNativeViewInfo(View formsView);
    }
}
