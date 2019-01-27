using System;

using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;

using Sharpnado.Infrastructure;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public class CenterSnapHelper : LinearSnapHelper
    {
        private readonly WeakReference<AndroidHorizontalListViewRenderer> _weakNativeView;

        public CenterSnapHelper(AndroidHorizontalListViewRenderer nativeView)
        {
            _weakNativeView = new WeakReference<AndroidHorizontalListViewRenderer>(nativeView);
        }

        public CenterSnapHelper(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override int[] CalculateDistanceToFinalSnap(RecyclerView.LayoutManager layoutManager, View targetView)
        {
            var result = base.CalculateDistanceToFinalSnap(layoutManager, targetView);

            InternalLogger.Info($"CalculateDistanceToFinalSnap()");
            return result;
        }

        public override int FindTargetSnapPosition(RecyclerView.LayoutManager layoutManager, int velocityX, int velocityY)
        {
            var targetSnapPosition = base.FindTargetSnapPosition(layoutManager, velocityX, velocityY);
            InternalLogger.Info($"FindTargetSnapPosition() : {targetSnapPosition}");

            if (_weakNativeView.TryGetTarget(out var target))
            {
                target.IsSnapHelperBusy = targetSnapPosition == -1;
            }

            return targetSnapPosition;
        }

        public override View FindSnapView(RecyclerView.LayoutManager layoutManager)
        {
            var view = base.FindSnapView(layoutManager);

            InternalLogger.Info($"FindSnapView()");

            ReleaseIsBusy();

            return view;
        }

        protected void ReleaseIsBusy()
        {
            if (_weakNativeView.TryGetTarget(out var target) && target.IsSnapHelperBusy)
            {
                target.IsSnapHelperBusy = false;
            }
        }
    }
}