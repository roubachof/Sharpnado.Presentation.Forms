using System;
using System.ComponentModel;

using Android.Content;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using Sharpnado.Presentation.Forms.Droid.Helpers;
using Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList;
using Sharpnado.Presentation.Forms.RenderedViews;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HorizontalListView), typeof(AndroidHorizontalListViewRenderer))]

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    [Xamarin.Forms.Internals.Preserve]
    public partial class AndroidHorizontalListViewRenderer : ViewRenderer<HorizontalListView, RecyclerView>
    {
        private bool _isCurrentIndexUpdateBackfire;

        private bool _isLandscape;

        private ItemTouchHelper _dragHelper;

        public AndroidHorizontalListViewRenderer(Context context)
            : base(context)
        {
        }

        public CustomLinearLayoutManager HorizontalLinearLayoutManager => Control?.GetLayoutManager() as CustomLinearLayoutManager;

        public GridLayoutManager GridLayoutManager => Control?.GetLayoutManager() as GridLayoutManager;

        public LinearLayoutManager LinearLayoutManager => Control?.GetLayoutManager() as LinearLayoutManager;

        public bool IsScrolling { get; set; }

        public bool IsSnapHelperBusy { get; set; }

        public static void Initialize()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HorizontalListView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null && !Control.IsNullOrDisposed())
            {
                Control.ClearOnScrollListeners();
                var treeViewObserver = Control.ViewTreeObserver;
                if (treeViewObserver != null)
                {
                    treeViewObserver.PreDraw -= OnPreDraw;
                }

                _dragHelper.AttachToRecyclerView(null);
                _dragHelper = null;
            }

            if (e.NewElement != null)
            {
                CreateView(e.NewElement);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(HorizontalListView.ItemsSource):
                    UpdateItemsSource();
                    break;
                case nameof(HorizontalListView.CurrentIndex) when !_isCurrentIndexUpdateBackfire:
                    ScrollToCurrentItem();
                    break;
                case nameof(HorizontalListView.DisableScroll):
                    ProcessDisableScroll();
                    break;
            }
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            if (!changed
                || Control == null
                || Element == null
                || (Element.ColumnCount == 0
                    && (Element.IsLayoutLinear && Element.ItemHeight > 0)))
            {
                base.OnLayout(changed, left, top, right, bottom);
                return;
            }

            int width = right - left;
            int height = bottom - top;

            if (ComputeItemSize(width, height))
            {
                UpdateItemsSource();
            }

            base.OnLayout(changed, left, top, right, bottom);
        }

        private bool ComputeItemSize(int width, int height)
        {
            if (Control == null
                || Element == null
                || (Element.ColumnCount == 0
                    && (Element.IsLayoutLinear && Element.ItemHeight > 0)))
            {
                return false;
            }

            bool widthChanged = false;
            bool heightChanged = false;
            if (Element.ColumnCount > 0)
            {
                int newItemWidth = Element.ComputeItemWidth(width);
                if (Element.ItemWidth != newItemWidth)
                {
                    Element.ItemWidth = newItemWidth;
                    widthChanged = true;
                }

                if (Element.ListLayout == HorizontalListViewLayout.Grid)
                {
                    if (Control.GetLayoutManager() is ResponsiveGridLayoutManager layoutManager)
                    {
                        layoutManager.ResetSpan();
                        Control.InvalidateItemDecorations();
                    }
                }
            }

            if (Element.IsLayoutLinear && Element.ItemHeight == 0)
            {
                int newItemHeight = Element.ComputeItemHeight(height);
                if (Element.ItemHeight != newItemHeight)
                {
                    Element.ItemHeight = newItemHeight;
                    heightChanged = true;
                }
            }

            return widthChanged || heightChanged;
        }

        private void CreateView(HorizontalListView horizontalList)
        {
            Element.CheckConsistency();

            var recyclerView = new SlowRecyclerView(Context, Element.ScrollSpeed);

            if (Element.ListLayout == HorizontalListViewLayout.Grid)
            {
                recyclerView.SetLayoutManager(new ResponsiveGridLayoutManager(Context, Element));
            }
            else
            {
                recyclerView.SetLayoutManager(new CustomLinearLayoutManager(Context, OrientationHelper.Horizontal, false));
            }

            if (Element.ItemSpacing > 0 || Element.CollectionPadding != new Thickness(0))
            {
                recyclerView.AddItemDecoration(new SpaceItemDecoration(Element.ItemSpacing, Element.CollectionPadding));
            }

            SetNativeControl(recyclerView);

            if (Element.SnapStyle != SnapStyle.None)
            {
                LinearSnapHelper snapHelper = Element.SnapStyle == SnapStyle.Start
                    ? new StartSnapHelper(this)
                    : new CenterSnapHelper(this);
                snapHelper.AttachToRecyclerView(Control);
            }

            Control.HorizontalScrollBarEnabled = false;

            if (Element.ItemsSource != null)
            {
                UpdateItemsSource();
            }

            if (LinearLayoutManager != null)
            {
                Control.AddOnScrollListener(new OnControlScrollChangedListener(this, horizontalList));

                ProcessDisableScroll();

                if (HorizontalLinearLayoutManager != null)
                {
                    ScrollToCurrentItem();
                }
            }

            Control.ViewTreeObserver.PreDraw += OnPreDraw;
        }

        private void OnPreDraw(object sender, ViewTreeObserver.PreDrawEventArgs e)
        {
            if (Control.IsNullOrDisposed())
            {
                return;
            }

            bool orientationChanged = false;
            if (Control.Height < Control.Width)
            {
                if (!_isLandscape)
                {
                    orientationChanged = true;
                    _isLandscape = true;

                    // Has just rotated
                    if (HorizontalLinearLayoutManager != null)
                    {
                        ScrollToCurrentItem();
                    }
                }
            }
            else
            {
                orientationChanged = _isLandscape;
                _isLandscape = false;
            }

            if (orientationChanged)
            {
                if (Control.GetLayoutManager() is ResponsiveGridLayoutManager layoutManager)
                {
                    layoutManager.ResetSpan();
                }

                Control.InvalidateItemDecorations();
            }
        }

        private void ProcessDisableScroll()
        {
            if (LinearLayoutManager == null)
            {
                return;
            }

            if (HorizontalLinearLayoutManager != null)
            {
                HorizontalLinearLayoutManager.CanScroll = !Element.DisableScroll;
            }
            else if (GridLayoutManager != null
                && GridLayoutManager is ResponsiveGridLayoutManager responsiveGridLayoutManager)
            {
                responsiveGridLayoutManager.CanScroll = !Element.DisableScroll;
            }
        }

        private void ScrollToCurrentItem()
        {
            if (Element.CurrentIndex == -1 || Control.GetAdapter() == null || Element.CurrentIndex >= Control.GetAdapter().ItemCount)
            {
                return;
            }

            int offset = 0;
            if (HorizontalLinearLayoutManager != null)
            {
                int itemWidth = PlatformHelper.Instance.DpToPixels(Element.ItemWidth + Element.ItemSpacing);
                int width = Control.MeasuredWidth;

                switch (Element.SnapStyle)
                {
                    case SnapStyle.Center:
                        offset = (width / 2) - (itemWidth / 2);
                        break;
                }
            }

            LinearLayoutManager?.ScrollToPositionWithOffset(Element.CurrentIndex, offset);
        }

        private void UpdateItemsSource()
        {
            Control.GetAdapter()?.Dispose();

            var adapter = new RecycleViewAdapter(Element, Context);
            Control.SetAdapter(adapter);

            if (Element.EnableDragAndDrop)
            {
                _dragHelper?.AttachToRecyclerView(null);

                _dragHelper = new ItemTouchHelper(
                    new DragAnDropItemTouchHelperCallback(Element, adapter, Element.DragAndDropEndedCommand));
                _dragHelper.AttachToRecyclerView(Control);
            }
        }
    }
}