using System;

using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;

using Sharpnado.Presentation.Forms.Droid.Helpers;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public class SpaceItemDecoration : RecyclerView.ItemDecoration
    {
        private readonly int _spaceDp;

        public SpaceItemDecoration(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public SpaceItemDecoration(int spaceDp)
        {
            _spaceDp = spaceDp;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            int spacePixel = PlatformHelper.DpToPixels(_spaceDp);
            outRect.Left = spacePixel;
            outRect.Top = spacePixel / 2;
            outRect.Right = spacePixel;
            outRect.Bottom = spacePixel / 2;
        }
    }

    public class ResponsiveGridLayoutManager : GridLayoutManager
    {
        private readonly Context _context;
        private readonly int _itemWidthDp;
        private readonly int _marginDp;

        public ResponsiveGridLayoutManager(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public ResponsiveGridLayoutManager(Context context, int itemWidthDp, int marginDp = 0)
            : base(context, 1)
        {
            _context = context;
            _itemWidthDp = itemWidthDp;
            _marginDp = marginDp;
        }

        public override void OnLayoutChildren(RecyclerView.Recycler recycler, RecyclerView.State state)
        {
            ComputeSpanCount(Width);
            base.OnLayoutChildren(recycler, state);
        }

        private void ComputeSpanCount(int recyclerWidth)
        {
            int spanCount = recyclerWidth / PlatformHelper.DpToPixels(_itemWidthDp + 2 * _marginDp);
            SpanCount = spanCount;
        }
    }
}