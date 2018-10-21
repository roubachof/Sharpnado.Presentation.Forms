using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public class CustomGridLayoutManager : GridLayoutManager
    {
        public CustomGridLayoutManager(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public CustomGridLayoutManager(Context context, int spanCount)
            : base(context, spanCount)
        {
        }

        public bool CanScroll { get; set; }

        public override bool CanScrollHorizontally()
        {
            return CanScroll && base.CanScrollHorizontally();
        }

        public override bool CanScrollVertically()
        {
            return CanScroll && base.CanScrollVertically();
        }
    }
}