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
            InternalLogger.Info($"FindSnapView()");

            View snapView = null;
            if (_weakNativeView.TryGetTarget(out var target))
            {
                int firstIndex = target.LinearLayoutManager.FindFirstCompletelyVisibleItemPosition();
                if (firstIndex == 0)
                {
                    // Check if first item is fully visible, if true don't snap.
                    snapView = target.LinearLayoutManager.FindViewByPosition(firstIndex);
                    InternalLogger.Info("Getting first view for snap: overriding center snap behavior");
                }

                int lastIndex = target.LinearLayoutManager.FindLastCompletelyVisibleItemPosition();
                if (lastIndex == target.Control.GetAdapter()?.ItemCount - 1)
                {
                    // Check if last item is fully visible, if true don't snap.
                    snapView = target.LinearLayoutManager.FindViewByPosition(lastIndex);
                    InternalLogger.Info("Getting last view for snap: overriding center snap behavior");
                }
            }

            if (snapView == null)
            {
                snapView = base.FindSnapView(layoutManager);
            }

            ReleaseIsBusy();

            return snapView;
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