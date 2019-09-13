using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

using Android.Content;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;

using Sharpnado.Infrastructure;
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
        private bool _isFirstInitialization = true;
        private IEnumerable _itemsSource;
        private ItemTouchHelper _dragHelper;
        private SpaceItemDecoration _itemDecoration;

        public AndroidHorizontalListViewRenderer(Context context)
            : base(context)
        {
        }

        public CustomLinearLayoutManager HorizontalLinearLayoutManager => Control?.GetLayoutManager() as CustomLinearLayoutManager;

        public GridLayoutManager GridLayoutManager => Control?.GetLayoutManager() as GridLayoutManager;

        public LinearLayoutManager LinearLayoutManager => Control?.GetLayoutManager() as LinearLayoutManager;

        public bool IsScrolling { get; set; }

        public bool IsSnapHelperBusy { get; set; }

        public int CurrentSnapIndex { get; set; }

        public static void Initialize()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HorizontalListView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                if (_dragHelper != null)
                {
                    _dragHelper.AttachToRecyclerView(null);
                    _dragHelper = null;
                }

                if (!Control.IsNullOrDisposed())
                {
                    Control.ClearOnScrollListeners();
                    var treeViewObserver = Control.ViewTreeObserver;
                    if (treeViewObserver != null)
                    {
                        treeViewObserver.PreDraw -= OnPreDraw;
                    }

                    Control.GetAdapter()?.Dispose();
                    Control.GetLayoutManager()?.Dispose();
                }

                if (_itemsSource is INotifyCollectionChanged oldNotifyCollection)
                {
                    oldNotifyCollection.CollectionChanged -= OnCollectionChanged;
                }
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
                case nameof(HorizontalListView.ListLayout):
                    UpdateListLayout();
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
                double newItemWidth = Element.ComputeItemWidth(width);
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
                double newItemHeight = Element.ComputeItemHeight(height);
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
            if (_isFirstInitialization)
            {
                Element.CheckConsistency();
                _isFirstInitialization = false;
            }

            var recyclerView = new SlowRecyclerView(Context, Element.ScrollSpeed) { HasFixedSize = false };

            SetListLayout(recyclerView);

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

        private void SetListLayout(RecyclerView recyclerView)
        {
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
                if (!_itemDecoration.IsNullOrDisposed())
                {
                    recyclerView.RemoveItemDecoration(_itemDecoration);
                    _itemDecoration = null;
                }

                _itemDecoration = new SpaceItemDecoration(Element.ItemSpacing);
                recyclerView.AddItemDecoration(_itemDecoration);
                recyclerView.SetPadding(
                    PlatformHelper.Instance.DpToPixels(Element.CollectionPadding.Left),
                    PlatformHelper.Instance.DpToPixels(Element.CollectionPadding.Top),
                    PlatformHelper.Instance.DpToPixels(Element.CollectionPadding.Right),
                    PlatformHelper.Instance.DpToPixels(Element.CollectionPadding.Bottom));

                recyclerView.SetClipToPadding(false);
            }
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

            InternalLogger.Info($"ScrollToCurrentItem() => CurrentItem: {Element.CurrentIndex}");

            int offset = 0;
            if (HorizontalLinearLayoutManager != null)
            {
                int itemWidth = PlatformHelper.Instance.DpToPixels(
                    Element.ItemWidth
                    + Element.ItemSpacing
                    + Element.CollectionPadding.HorizontalThickness);

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
            InternalLogger.Info($"UpdateItemsSource()");

            var oldAdapter = Control.GetAdapter();

            if (_itemsSource is INotifyCollectionChanged oldNotifyCollection)
            {
                oldNotifyCollection.CollectionChanged -= OnCollectionChanged;
            }

            _itemsSource = Element.ItemsSource;

            Control.GetRecycledViewPool().Clear();
            var adapter = new RecycleViewAdapter(Element, Control, Context);
            Control.SetAdapter(adapter);

            oldAdapter?.Dispose();

            if (_itemsSource is INotifyCollectionChanged newNotifyCollection)
            {
                newNotifyCollection.CollectionChanged += OnCollectionChanged;
            }

            if (Element.EnableDragAndDrop)
            {
                _dragHelper?.AttachToRecyclerView(null);

                _dragHelper = new ItemTouchHelper(
                    new DragAnDropItemTouchHelperCallback(Element, adapter, Element.DragAndDropEndedCommand));
                _dragHelper.AttachToRecyclerView(Control);
            }

            ScrollToCurrentItem();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UpdateItemsSource();
            }
        }

        private void UpdateListLayout()
        {
            if (Control.IsNullOrDisposed())
            {
                return;
            }

            SetListLayout(Control);
            UpdateItemsSource();
            ProcessDisableScroll();
            ScrollToCurrentItem();
        }
    }
}
