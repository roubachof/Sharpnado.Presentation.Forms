using System;

using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;

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
            int right = _space / 2;
            int left = _space / 2;

            bool isViewEdgeLeft = false;
            bool isViewEdgeRight = false;

            int viewPosition = parent.GetChildAdapterPosition(view);
            int viewCount = parent.GetAdapter().ItemCount;

            if (parent.GetLayoutManager() is ResponsiveGridLayoutManager responsiveGridLayout)
            {
                int top = _space / 2;
                int bottom = _space / 2;

                bool isAutoComputedItemWidth = responsiveGridLayout.IsAutoComputedItemWidth();

                int spanCount = responsiveGridLayout.SpanCount;
                isViewEdgeLeft = viewPosition % spanCount == 0;
                bool isViewEdgeTop = viewPosition < spanCount;
                isViewEdgeRight = viewPosition % spanCount == spanCount - 1;
                bool isViewEdgeBottom = viewPosition / spanCount == viewCount - 1 / spanCount;

                if (isViewEdgeTop)
                {
                    top = isAutoComputedItemWidth ? 0 : _topPadding;
                }

                if (isViewEdgeBottom)
                {
                    bottom = isAutoComputedItemWidth ? 0 : _bottomPadding;
                }

                bool isHorizontalEdge = isViewEdgeLeft || isViewEdgeRight;

                if (!responsiveGridLayout.TryGetItemWidth(out int itemWidth))
                {
                    base.GetItemOffsets(outRect, view, parent, state);
                    return;
                }

                int gridLeftPadding = _leftPadding;
                int gridRightPadding = _rightPadding;
                if (spanCount == 1)
                {
                    // If there is only one column we want our items centered
                    gridLeftPadding = 0;
                    gridRightPadding = 0;
                }

                //InternalLogger.Info(
                //    $"interSpacing computation:{Environment.NewLine}    parent.MeasuredWidth: {parent.MeasuredWidth}{Environment.NewLine}    parent.Width: {parent.Width}{Environment.NewLine}    spanCount: {spanCount}{Environment.NewLine}    itemWidth:{itemWidth}{Environment.NewLine}    gridLeftPadding: {gridLeftPadding}{Environment.NewLine}    gridRightPadding: {gridRightPadding}");

                int availableWidthSpace =
                    parent.MeasuredWidth - gridLeftPadding - gridRightPadding - spanCount * itemWidth;

                //InternalLogger.Info($"availableWidthSpace: {availableWidthSpace}");

                if (spanCount == 1)
                {
                    if (isAutoComputedItemWidth)
                    {
                        left = 0;
                        right = 0;
                    }
                    else
                    {
                        left = right = availableWidthSpace / 2;
                    }
                }
                else
                {
                    int interItemSpace = availableWidthSpace / (spanCount - 1);
                    int halfInterItemSpace = interItemSpace / 2;
                    left = interItemSpace / 2;
                    right = interItemSpace / 2;

                    if (isHorizontalEdge)
                    {
                        int remaining = availableWidthSpace - halfInterItemSpace * 2 * (spanCount - 1);

                        // InternalLogger.Info($"halfInterItemSpace: {halfInterItemSpace}, remaining: {remaining}");

                        if (isViewEdgeLeft)
                        {
                            left = _leftPadding + remaining;
                        }

                        if (isViewEdgeRight)
                        {
                            right = _rightPadding;
                        }
                    }
                }

                if (isAutoComputedItemWidth)
                {
                    base.GetItemOffsets(outRect, view, parent, state);
                    outRect.Set(outRect.Left, top, outRect.Right, bottom);

                    // InternalLogger.Info(
                    //    $"view n°{viewPosition + 1} => left: {outRect.Left}, top: {outRect.Top}, right: {outRect.Right}, bottom: {outRect.Bottom}");
                    return;
                }

                outRect.Set(left, top, right, bottom);
                // InternalLogger.Info(
                //    $"view n°{viewPosition + 1} => left: {outRect.Left}, top: {outRect.Top}, right: {outRect.Right}, bottom: {outRect.Bottom}");
                return;
            }

            if (!(parent.GetLayoutManager() is LinearLayoutManager))
            {
                base.GetItemOffsets(outRect, view, parent, state);
                return;
            }

            isViewEdgeLeft = viewPosition == 0;
            isViewEdgeRight = viewPosition == viewCount - 1;

            if (isViewEdgeLeft)
            {
                left = 0;
            }

            if (isViewEdgeRight)
            {
                right = 0;
            }

            outRect.Set(left, 0, right, 0);

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