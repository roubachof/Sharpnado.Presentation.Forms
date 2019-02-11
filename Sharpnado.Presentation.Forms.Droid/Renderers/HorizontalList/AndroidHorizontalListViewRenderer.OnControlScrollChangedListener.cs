using System;
using System.Threading;

using Android.Runtime;
using Android.Support.V7.Widget;

using Sharpnado.Infrastructure;
using Sharpnado.Presentation.Forms.RenderedViews;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public partial class AndroidHorizontalListViewRenderer
    {
        private class OnControlScrollChangedListener : RecyclerView.OnScrollListener
        {
            private readonly WeakReference<AndroidHorizontalListViewRenderer> _weakNativeView;
            private readonly HorizontalListView _element;

            private CancellationTokenSource _cts;
            private int _lastVisibleItemIndex = -1;

            public OnControlScrollChangedListener(IntPtr handle, JniHandleOwnership transfer)
                : base(handle, transfer)
            {
            }

            public OnControlScrollChangedListener(
                AndroidHorizontalListViewRenderer nativeView,
                HorizontalListView element)
            {
                _weakNativeView = new WeakReference<AndroidHorizontalListViewRenderer>(nativeView);
                _element = element;
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                base.OnScrolled(recyclerView, dx, dy);

                var infiniteListLoader = _element?.InfiniteListLoader;
                if (infiniteListLoader == null)
                {
                    return;
                }

                var linearLayoutManager = (LinearLayoutManager)recyclerView.GetLayoutManager();
                int lastVisibleItem = linearLayoutManager.FindLastVisibleItemPosition();
                if (_lastVisibleItemIndex == lastVisibleItem)
                {
                    return;
                }

                _lastVisibleItemIndex = lastVisibleItem;
                // InternalLogger.Info($"OnScrolled( lastVisibleItem: {lastVisibleItem} )");
                infiniteListLoader.OnScroll(lastVisibleItem);
            }

            public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
            {
                InternalLogger.Info($"OnScrollStateChanged( newState: {newState} )");
                switch (newState)
                {
                    case RecyclerView.ScrollStateSettling:
                    case RecyclerView.ScrollStateDragging:
                    {
                        if (!_weakNativeView.TryGetTarget(out AndroidHorizontalListViewRenderer nativeView))
                        {
                            return;
                        }

                        if (nativeView.IsScrolling)
                        {
                            return;
                        }

                        if (_cts != null && !_cts.IsCancellationRequested)
                        {
                            // System.Diagnostics.Debug.WriteLine("DEBUG_SCROLL: Cancelling previous update index task");
                            _cts.Cancel();
                        }

                        nativeView.IsScrolling = true;

                        _element.ScrollBeganCommand?.Execute(null);
                        break;
                    }

                    case RecyclerView.ScrollStateIdle:
                    {
                        if (!_weakNativeView.TryGetTarget(out AndroidHorizontalListViewRenderer nativeView))
                        {
                            return;
                        }

                        if (!nativeView.IsScrolling)
                        {
                            return;
                        }

                        if (nativeView.IsSnapHelperBusy)
                        {
                            return;
                         }

                        nativeView.IsScrolling = false;

                        _cts = new CancellationTokenSource();
                        UpdateCurrentIndex(nativeView, _cts.Token);
                        _element.ScrollEndedCommand?.Execute(null);

                        break;
                    }
                }
            }

            private void UpdateCurrentIndex(
                AndroidHorizontalListViewRenderer nativeView,
                CancellationToken token)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (nativeView?.LinearLayoutManager == null || _element == null)
                {
                    return;
                }

                nativeView._isCurrentIndexUpdateBackfire = true;
                try
                {
                    int newIndex = -1;
                    if (_element.SnapStyle == SnapStyle.Center)
                    {
                        int firstIndex = nativeView.LinearLayoutManager.FindFirstCompletelyVisibleItemPosition();
                        if (firstIndex == 0)
                        {
                            // Check if first item is fully visible, if true don't snap.
                            newIndex = 0;
                        }

                        int lastIndex = nativeView.LinearLayoutManager.FindLastCompletelyVisibleItemPosition();
                        if (lastIndex == nativeView.Control.GetAdapter().ItemCount - 1)
                        {
                            // Check if first item is fully visible, if true don't snap.
                            newIndex = lastIndex;
                        }

                        if (newIndex == -1)
                        {
                            int firstItemIndex = nativeView.LinearLayoutManager.FindFirstVisibleItemPosition();
                            int lastItemIndex = nativeView.LinearLayoutManager.FindLastVisibleItemPosition();
                            newIndex = firstItemIndex + (lastItemIndex - firstItemIndex) / 2;
                        }
                    }
                    else
                    {
                        newIndex = nativeView.LinearLayoutManager.FindFirstVisibleItemPosition();
                    }

                    if (newIndex == -1)
                    {
                        InternalLogger.Warn(
                            "Failed to find the current index: UpdateCurrentIndex returns nothing");
                        return;
                    }

                    _element.CurrentIndex = newIndex;
                    InternalLogger.Info($"CurrentIndex: {_element.CurrentIndex}");
                }
                finally
                {
                    nativeView._isCurrentIndexUpdateBackfire = false;
                }
            }
        }
    }
}