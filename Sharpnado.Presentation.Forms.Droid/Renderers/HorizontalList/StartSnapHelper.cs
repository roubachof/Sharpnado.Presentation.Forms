using System;

using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;

using Sharpnado.Infrastructure;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public class StartSnapHelper : CenterSnapHelper
    {
        private OrientationHelper _verticalHelper;
        private OrientationHelper _horizontalHelper;

        public StartSnapHelper(AndroidHorizontalListViewRenderer nativeView)
            : base(nativeView)
        {
        }

        public StartSnapHelper(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override int[] CalculateDistanceToFinalSnap(RecyclerView.LayoutManager layoutManager, View targetView)
        {
            int[] result = new int[2];

            result[0] = layoutManager.CanScrollHorizontally()
                ? DistanceToStart(targetView, GetHorizontalHelper(layoutManager))
                : 0;

            result[1] = layoutManager.CanScrollVertically()
                ? DistanceToStart(targetView, GetVerticalHelper(layoutManager))
                : 0;

            InternalLogger.Info($"CalculateDistanceToFinalSnap()");
            return result;
        }

        public override View FindSnapView(RecyclerView.LayoutManager layoutManager)
        {
            if (!(layoutManager is LinearLayoutManager))
            {
                return base.FindSnapView(layoutManager);
            }

            OrientationHelper helper = layoutManager.CanScrollHorizontally()
                ? GetHorizontalHelper(layoutManager)
                : GetVerticalHelper(layoutManager);

            var startView = GetStartView(layoutManager, helper, out int viewPosition);

            InternalLogger.Info($"FindSnapView ( viewPosition: {viewPosition} )");

            ReleaseIsBusy(startView);

            return startView;
        }

        private View GetStartView(
            RecyclerView.LayoutManager layoutManager,
            OrientationHelper helper,
            out int viewPosition)
        {
            viewPosition = -1;
            if (!(layoutManager is LinearLayoutManager manager))
            {
                return base.FindSnapView(layoutManager);
            }

            int firstChild = manager.FindFirstVisibleItemPosition();

            bool isLastItem = manager.FindLastCompletelyVisibleItemPosition() == layoutManager.ItemCount - 1;

            if (isLastItem)
            {
                if (WeakNativeView.TryGetTarget(out var target))
                {
                    target.CurrentSnapIndex = layoutManager.ItemCount - 1;
                    // System.Diagnostics.Debug.WriteLine($">>>>>> CurrentSnapIndex: {target.CurrentSnapIndex}");
                }

                return null;
            }

            if (firstChild == RecyclerView.NoPosition)
            {
                return null;
            }

            View child = layoutManager.FindViewByPosition(firstChild);

            if (helper.GetDecoratedEnd(child) >= helper.GetDecoratedMeasurement(child) / 2
                && helper.GetDecoratedEnd(child) > 0)
            {
                viewPosition = firstChild;
                return child;
            }

            viewPosition = firstChild + 1;
            return layoutManager.FindViewByPosition(firstChild + 1);
        }

        private int DistanceToStart(View targetView, OrientationHelper helper)
        {
            return helper.GetDecoratedStart(targetView) - helper.StartAfterPadding;
        }

        private OrientationHelper GetVerticalHelper(RecyclerView.LayoutManager layoutManager)
        {
            return _verticalHelper ?? (_verticalHelper = OrientationHelper.CreateVerticalHelper(layoutManager));
        }

        private OrientationHelper GetHorizontalHelper(RecyclerView.LayoutManager layoutManager)
        {
            return _horizontalHelper ?? (_horizontalHelper = OrientationHelper.CreateHorizontalHelper(layoutManager));
        }
    }
}