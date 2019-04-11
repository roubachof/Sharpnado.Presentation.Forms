using System;

using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using Sharpnado.Presentation.Forms.RenderedViews;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public class SlowRecyclerView : RecyclerView
    {
        private readonly ScrollSpeed _scrollSpeed;

        private readonly double _percent = 1;

        public SlowRecyclerView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public SlowRecyclerView(Context context, ScrollSpeed scrollSpeed)
            : base(context)
        {
            _scrollSpeed = scrollSpeed;
            switch (_scrollSpeed)
            {
                case ScrollSpeed.Normal:
                    _percent = 1;
                    break;
                case ScrollSpeed.Slower:
                    _percent = 0.5;
                    break;
                case ScrollSpeed.Slowest:
                    _percent = 0.2;
                    break;
            }

            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent);
        }

        public override bool Fling(int velocityX, int velocityY)
        {
            velocityX = (int)(velocityX * _percent);
            velocityY = (int)(velocityY * _percent);

            return base.Fling(velocityX, velocityY);
        }
    }
}