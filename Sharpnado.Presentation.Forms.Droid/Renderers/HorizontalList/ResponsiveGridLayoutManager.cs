using System;

using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V7.Widget;

using Sharpnado.Presentation.Forms.RenderedViews;

using View = Android.Views.View;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public class SpaceItemDecoration : RecyclerView.ItemDecoration
    {
        private readonly int _space;

        private readonly int _verticalMargin;

        public SpaceItemDecoration(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public SpaceItemDecoration(int spaceDp)
        {
            _space = PlatformHelper.Instance.DpToPixels(spaceDp);
            _verticalMargin = PlatformHelper.Instance.DpToPixels(MeasureHelper.RecyclerViewItemVerticalMarginDp);
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            int viewPosition = parent.GetChildAdapterPosition(view);
            int viewCount = parent.GetAdapter().ItemCount;

            switch (parent.GetLayoutManager())
            {
                case ResponsiveGridLayoutManager responsiveGridLayout:
                    int adaptedSpace = (int)((_space - _verticalMargin * 2) * 0.66);
                    int top = adaptedSpace / 2;
                    int bottom = adaptedSpace / 2;
                    int spanCount = responsiveGridLayout.SpanCount;

                    bool isViewEdgeTop = viewPosition < spanCount;
                    if (isViewEdgeTop)
                    {
                        top = 0;
                    }

                    bool isViewEdgeBottom = viewPosition / spanCount == viewCount - 1 / spanCount;
                    if (isViewEdgeBottom)
                    {
                        bottom = 0;
                    }

                    outRect.Set(0, top, 0, bottom);
                    return;

                case LinearLayoutManager linearLayout:
                    int right = _space / 2;
                    int left = _space / 2;
                    bool isViewEdgeLeft = viewPosition == 0;
                    bool isViewEdgeRight = viewPosition == viewCount - 1;

                    if (isViewEdgeLeft)
                    {
                        left = 0;
                    }

                    if (isViewEdgeRight)
                    {
                        right = 0;
                    }

                    outRect.Set(left, 0, right, 0);
                    return;
            }

            base.GetItemOffsets(outRect, view, parent, state);

            //InternalLogger.Info(
            //    $"view n°{viewPosition + 1} => left: {left}, right: {right}");
        }
    }

    public class ResponsiveGridLayoutManager : GridLayoutManager
    {
        private readonly Context _context;
        private readonly WeakReference<HorizontalListView> _weakElement;

        private bool _spanNeedsCompute;

        public ResponsiveGridLayoutManager(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public ResponsiveGridLayoutManager(Context context, HorizontalListView element)
            : base(context, 1)
        {
            _context = context;
            _weakElement = new WeakReference<HorizontalListView>(element);
            _spanNeedsCompute = true;
        }

        public bool CanScroll { get; set; }

        public bool TryGetItemWidth(out int itemWidth)
        {
            itemWidth = 0;
            if (_weakElement.TryGetTarget(out var element))
            {
                itemWidth = PlatformHelper.Instance.DpToPixels(element.ItemWidth);
            }

            return itemWidth > 0;
        }

        public bool IsAutoComputedItemWidth()
        {
            if (_weakElement.TryGetTarget(out var element))
            {
                return element.ColumnCount > 0;
            }

            return false;
        }

        public void ResetSpan()
        {
            _spanNeedsCompute = true;
        }

        public override void OnLayoutChildren(RecyclerView.Recycler recycler, RecyclerView.State state)
        {
            if (Width > 0 && _spanNeedsCompute)
            {
                ComputeSpanCount(Width);
            }

            base.OnLayoutChildren(recycler, state);
        }

        public override bool CanScrollHorizontally()
        {
            return CanScroll && base.CanScrollHorizontally();
        }

        public override bool CanScrollVertically()
        {
            return CanScroll && base.CanScrollVertically();
        }

        private void ComputeSpanCount(int recyclerWidth)
        {
            if (_weakElement.TryGetTarget(out var element))
            {
                SpanCount = MeasureHelper.ComputeSpan(recyclerWidth, element);
            }

            _spanNeedsCompute = false;
        }
    }
}