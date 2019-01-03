using Sharpnado.Infrastructure;
using Sharpnado.Presentation.Forms.Droid.Helpers;
using Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList;
using Sharpnado.Presentation.Forms.RenderedViews;

namespace Sharpnado.Presentation.Forms.Droid
{
    public static class SharpnadoInitializer
    {
        public static void Initialize(bool enableInternalLogger = false)
        {
            InternalLogger.EnableLogging = enableInternalLogger;
            PlatformHelper.InitializeSingleton(new AndroidPlatformHelper());
            AndroidHorizontalListViewRenderer.Initialize();
        }
    }
}