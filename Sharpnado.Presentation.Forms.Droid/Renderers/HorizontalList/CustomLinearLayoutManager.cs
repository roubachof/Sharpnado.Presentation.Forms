using System;

using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public class CustomLinearLayoutManager : LinearLayoutManager
    {
        public CustomLinearLayoutManager(IntPtr javaReference, JniHandleOwnership transfer)
            : base(
            javaReference,
            transfer)
        {
        }

        public CustomLinearLayoutManager(Context context, int orientation, bool reverseLayout)
            : base(
            context,
            orientation,
            reverseLayout)
        {
        }

        public bool CanScroll { get; set; }

        public override bool CanScrollHorizontally()
        {
            return CanScroll && base.CanScrollHorizontally();
        }
    }
}