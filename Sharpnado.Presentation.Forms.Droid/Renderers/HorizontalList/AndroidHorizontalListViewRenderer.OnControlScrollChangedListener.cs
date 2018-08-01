using System;
using System.Threading;
using System.Threading.Tasks;

using Android.Runtime;
using Android.Support.V7.Widget;

using Sharpnado.Infrastructure.Tasks;
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

            public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
            {
                // System.Diagnostics.Debug.WriteLine($"DEBUG_SCROLL: OnScrollStateChanged ( newState: {newState} )");
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

                        nativeView.IsScrolling = false;

                        _cts = new CancellationTokenSource();
                        NotifyTask.Create(UpdateCurrentIndexAsync(nativeView, _cts.Token));

                        break;
                    }
                }
            }

            private async Task UpdateCurrentIndexAsync(
                AndroidHorizontalListViewRenderer nativeView,
                CancellationToken token)
            {
                await Task.Delay(500);

                if (token.IsCancellationRequested)
                {
                    return;
                }

                nativeView._isCurrentIndexUpdateBackfire = true;
                try
                {
                    _element.CurrentIndex = nativeView.LinearLayoutManager.FindFirstVisibleItemPosition();
                    _element.ScrollBeganCommand?.Execute(null);

                    // System.Diagnostics.Debug.WriteLine($"DEBUG_SCROLL: CurrentIndex: {_element.CurrentIndex}");
                }
                finally
                {
                    nativeView._isCurrentIndexUpdateBackfire = false;
                }
            }
        }
    }
}