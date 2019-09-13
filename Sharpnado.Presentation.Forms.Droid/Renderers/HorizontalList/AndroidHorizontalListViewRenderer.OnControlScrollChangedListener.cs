using System;
using System.Threading;

using Android.Graphics;
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

            private int _currentOffset = 0;

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

                _currentOffset += dx;

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

                        // System.Diagnostics.Debug.WriteLine("DEBUG_SCROLL: BeginScroll command");
                        _element.ScrollBeganCommand?.Execute(null);
                        break;
                    }

                    case RecyclerView.ScrollStateSettling:
                    {
                        if (!_weakNativeView.TryGetTarget(out AndroidHorizontalListViewRenderer nativeView))
                        {
                            return;
                        }

                        nativeView.IsScrolling = true;
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
                            // System.Diagnostics.Debug.WriteLine("DEBUG_SCROLL: returning !nativeView.IsScrolling");
                            return;
                        }

                        if (nativeView.IsSnapHelperBusy)
                        {
                            // System.Diagnostics.Debug.WriteLine("DEBUG_SCROLL: returning nativeView.IsSnapHelperBusy");
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
                    if (_element.SnapStyle != SnapStyle.None)
                    {
                        newIndex = nativeView.CurrentSnapIndex;
                    }
                    else
                    {
                        newIndex = nativeView.LinearLayoutManager.FindFirstCompletelyVisibleItemPosition();
                        if (newIndex == -1)
                        {
                            newIndex = nativeView.LinearLayoutManager.FindFirstVisibleItemPosition();
                        }
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