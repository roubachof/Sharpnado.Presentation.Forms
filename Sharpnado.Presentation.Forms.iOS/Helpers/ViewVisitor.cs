using System;

using UIKit;

namespace Sharpnado.Presentation.Forms.iOS.Helpers
{
    public static class ViewVisitor
    {
        public static void Visit(UIView view, Action<UIView, int, bool> action, bool isVerbose)
        {
            Visit(view, action, 0, isVerbose);
        }

        private static void Visit(object @object, Action<UIView, int, bool> action, int currentDepth, bool isVerbose)
        {
            if (!(@object is UIView view))
            {
                return;
            }

            action(view, currentDepth, isVerbose);

            for (int i = 0; i < view.Subviews.Length; i++)
            {
                Visit(view.Subviews[i], action, currentDepth + 1, isVerbose);
            }
        }
    }
}