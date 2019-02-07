using System;

using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V7.Widget;

using Sharpnado.Infrastructure;
using Sharpnado.Presentation.Forms.Droid.Helpers;
using Sharpnado.Presentation.Forms.RenderedViews;
using Xamarin.Forms;

using View = Android.Views.View;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public class SpaceItemDecoration : RecyclerView.ItemDecoration
    {
        private readonly int _space;
        private readonly int _leftPadding;
        private readonly int _topPadding;
        private readonly int _rightPadding;
        private readonly int _bottomPadding;

        public SpaceItemDecoration(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public SpaceItemDecoration(int spaceDp, Thickness paddingDp)
        {
            _space = PlatformHelper.Instance.DpToPixels(spaceDp);
            _leftPadding = PlatformHelper.Instance.DpToPixels(paddingDp.Left);
            _topPadding = PlatformHelper.Instance.DpToPixels(paddingDp.Top);
            _rightPadding = PlatformHelper.Instance.DpToPixels(paddingDp.Right);
            _bottomPadding = PlatformHelper.Instance.DpToPixels(paddingDp.Bottom);
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            int left, top, right, bottom;
            left = right = top = bottom = _space / 2;

            bool isViewEdgeLeft = false;
            bool isViewEdgeTop = false;
            bool isViewEdgeRight = false;
            bool isViewEdgeBottom = false;
            bool horizontalOffsetAssigned = false;

            int viewPosition = parent.GetChildAdapterPosition(view);
            int viewCount = parent.GetAdapter().ItemCount;

            if (parent.GetLayoutManager() is ResponsiveGridLayoutManager responsiveGridLayout)
            {
                int spanCount = responsiveGridLayout.SpanCount;
                isViewEdgeLeft = viewPosition % spanCount == 0;
                isViewEdgeTop = viewPosition < spanCount;
                isViewEdgeRight = viewPosition % spanCount == spanCount - 1;
                isViewEdgeBottom = viewPosition / spanCount == viewCount - 1 / spanCount;

                if (responsiveGridLayout.TryGetItemWidth(out int itemWidth))
                {
                    int gridLeftPadding = _leftPadding;
                    int gridRightPadding = _rightPadding;
                    if (spanCount == 1)
                    {
                        // If there is only one column we want our items centered
                        gridLeftPadding = 0;
                        gridRightPadding = 0;
                        horizontalOffsetAssigned = true;
                    }

                    int availableWidthSpace = parent.Width - gridLeftPadding - gridRightPadding - spanCount * itemWidth;
                    int interItemSpacing = spanCount > 1 ? availableWidthSpace / (spanCount - 1) : availableWidthSpace;
                    left = right = interItemSpacing / 2;
                }
            }
            else if (parent.GetLayoutManager() is LinearLayoutManager)
            {
                isViewEdgeLeft = viewPosition == 0;
                isViewEdgeTop = isViewEdgeBottom = true;
                isViewEdgeRight = viewPosition == viewCount - 1;
            }

            if (isViewEdgeLeft && !horizontalOffsetAssigned)
            {
                left = _leftPadding;
            }

            if (isViewEdgeRight && !horizontalOffsetAssigned)
            {
                right = _rightPadding;
            }

            if (isViewEdgeTop)
            {
                top = _topPadding;
            }

            if (isViewEdgeBottom)
            {
                bottom = _bottomPadding;
            }

            outRect.Left = left;
            outRect.Top = top;
            outRect.Right = right;
            outRect.Bottom = bottom;

            InternalLogger.Info(
                $"view n°{++viewPosition} => left: {left}, top: {top}, right: {right}, bottom: {bottom}");
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