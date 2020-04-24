﻿using Sharpnado.Presentation.Forms.Droid.Helpers;
using Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList;
using Sharpnado.Presentation.Forms.RenderedViews;

namespace Sharpnado.Presentation.Forms.Droid
{
    public static class SharpnadoInitializer
    {
        public static void Initialize(bool enableInternalLogger = false, bool enableInternalDebugLogger = false)
        {
            InternalLogger.EnableLogger(enableInternalLogger, enableInternalDebugLogger);
            PlatformHelper.InitializeSingleton(new AndroidPlatformHelper());
            AndroidHorizontalListViewRenderer.Initialize();
        }
    }
}