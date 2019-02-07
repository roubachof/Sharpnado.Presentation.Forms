using System;

namespace Sharpnado.Presentation.Forms.RenderedViews
{
    public abstract class PlatformHelper
    {
        private static PlatformHelper _instance;

        public static PlatformHelper Instance => _instance;

        public static void InitializeSingleton(PlatformHelper instance)
        {
            _instance = instance;
        }

        public abstract int DpToPixels(int dp);

        public abstract int DpToPixels(double dp);

        public abstract int PixelsToDp(int pixels);
    }
}
