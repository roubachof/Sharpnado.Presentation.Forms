using Sharpnado.Infrastructure;
using Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList;

namespace Sharpnado.Presentation.Forms.Droid
{
    public static class SharpnadoInitializer
    {
        public static void Initialize(bool enableInternalLogger = false)
        {
            InternalLogger.EnableLogging = enableInternalLogger;
            AndroidHorizontalListViewRenderer.Initialize();
        }
    }
}