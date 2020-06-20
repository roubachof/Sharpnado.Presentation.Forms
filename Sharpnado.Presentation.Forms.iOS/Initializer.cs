using Sharpnado.MaterialFrame.iOS;
using Sharpnado.Presentation.Forms.iOS.Helpers;
using Sharpnado.Presentation.Forms.iOS.Renderers.HorizontalList;
using Sharpnado.Presentation.Forms.RenderedViews;
using Sharpnado.Shades.iOS;

namespace Sharpnado.Presentation.Forms.iOS
{
    public static class SharpnadoInitializer
    {
        public static void Initialize(bool enableInternalLogger = false, bool enableInternalDebugLogger = false)
        {
            InternalLogger.EnableLogger(enableInternalLogger, enableInternalDebugLogger);
            PlatformHelper.InitializeSingleton(new iOSPlatformHelper());
            iOSMaterialFrameRenderer.Init();
            iOSHorizontalListViewRenderer.Initialize();
            iOSShadowsRenderer.Initialize();
        }
    }
}