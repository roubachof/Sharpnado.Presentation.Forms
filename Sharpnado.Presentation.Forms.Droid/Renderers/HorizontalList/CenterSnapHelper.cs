using System;

using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;

using Sharpnado.Infrastructure;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public class CenterSnapHelper : LinearSnapHelper
    {
        public CenterSnapHelper(AndroidHorizontalListViewRenderer nativeView)
        {
            WeakNativeView = new WeakReference<AndroidHorizontalListViewRenderer>(nativeView);
        }

        public CenterSnapHelper(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected WeakReference<AndroidHorizontalListViewRenderer> WeakNativeView { get; }

        public override int FindTargetSnapPosition(RecyclerView.LayoutManager layoutManager, int velocityX, int velocityY)
        {
            var targetSnapPosition = base.FindTargetSnapPosition(layoutManager, velocityX, velocityY);
            InternalLogger.Info($"FindTargetSnapPosition() : {targetSnapPosition}");

            if (WeakNativeView.TryGetTarget(out var target))
            {
                target.IsSnapHelperBusy = targetSnapPosition == -1;
                target.CurrentSnapIndex = targetSnapPosition > -1 ? targetSnapPosition : target.CurrentSnapIndex;
                System.Diagnostics.Debug.WriteLine($">>>>>> CurrentSnapIndex: {target.CurrentSnapIndex}");
            }

            return targetSnapPosition;
        }

        public override View FindSnapView(RecyclerView.LayoutManager layoutManager)
        {
            InternalLogger.Info($"FindSnapView()");

            View snapView = null;
            if (WeakNativeView.TryGetTarget(out var target))
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

            ReleaseIsBusy(snapView);

            return snapView;
        }

        protected void ReleaseIsBusy(View snapView)
        {
            if (WeakNativeView.TryGetTarget(out var target))
            {
                target.IsSnapHelperBusy = false;

                if (snapView != null)
                {
                    var viewHolder = target.Control.FindContainingViewHolder(snapView);
                    target.CurrentSnapIndex = viewHolder.AdapterPosition;
                    // System.Diagnostics.Debug.WriteLine($">>>>>> CurrentSnapIndex: {target.CurrentSnapIndex}");
                }
            }
        }
    }
}