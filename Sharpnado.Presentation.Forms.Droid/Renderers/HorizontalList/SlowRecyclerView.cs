using System;

using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public class SlowRecyclerView : RecyclerView
    {
        public SlowRecyclerView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public SlowRecyclerView(Context context)
            : base(context)
        {
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent);
        }

        public override bool Fling(int velocityX, int velocityY)
        {
            velocityX = (int)(velocityX * 0.5);

            return base.Fling(velocityX, velocityY);
        }
    }
}