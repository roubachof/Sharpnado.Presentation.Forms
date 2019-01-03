using System;

namespace Sharpnado.Presentation.Forms.RenderedViews
{
    public abstract class PlatformHelper
    {
        private static PlatformHelper _instance;

        public static PlatformHelper Instance => _instance;

        public static void InitializeSingleton(PlatformHelper instance)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("Singleton already set");
            }

            _instance = instance;
        }

        public abstract int DpToPixels(int dp);

        public abstract int DpToPixels(double dp);

        public abstract int PixelsToDp(int pixels);
    }
}
