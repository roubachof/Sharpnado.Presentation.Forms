using System;

using Android.Views;

namespace Sharpnado.Presentation.Forms.Droid.Helpers
{
    public static class ViewVisitor
    {
        public static void Visit(View view, Action<View, int> action)
        {
            Visit(view, action, 0);
        }

        private static void Visit(object @object, Action<View, int> action, int currentDepth)
        {
            if (!(@object is View view))
            {
                return;
            }

            action(view, currentDepth);

            if (view is ViewGroup viewGroup)
            {
                for (int i = 0; i < viewGroup.ChildCount; i++)
                {
                    Visit(viewGroup.GetChildAt(i), action, currentDepth + 1);
                }
            }
        }
    }
}
