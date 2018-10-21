using Sharpnado.Infrastructure;
using Sharpnado.Presentation.Forms.iOS.Renderers.HorizontalList;

namespace Sharpnado.Presentation.Forms.iOS
{
    public static class SharpnadoInitializer
    {
        public static void Initialize(bool enableInternalLogger = false)
        {
            InternalLogger.EnableLogging = enableInternalLogger;
            iOSHorizontalListViewRenderer.Initialize();
        }
    }
}