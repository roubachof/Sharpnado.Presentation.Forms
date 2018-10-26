using System.ComponentModel;

using Android.Content;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
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

        public AndroidHorizontalListViewRenderer(Context context)
            : base(context)
        {
        }

        public CustomLinearLayoutManager HorizontalLinearLayoutManager => Control?.GetLayoutManager() as CustomLinearLayoutManager;

        public GridLayoutManager GridLayoutManager => Control?.GetLayoutManager() as GridLayoutManager;

        public LinearLayoutManager LinearLayoutManager => Control?.GetLayoutManager() as LinearLayoutManager;

        public bool IsScrolling { get; set; }

        public static void Initialize()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HorizontalListView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Control?.ClearOnScrollListeners();
                var treeViewObserver = Control?.ViewTreeObserver;
                if (treeViewObserver != null)
                {
                    treeViewObserver.PreDraw -= OnPreDraw;
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
            }
        }

        private void CreateView(HorizontalListView horizontalList)
        {
            var recyclerView = new SlowRecyclerView(Context);

            if (Element.ListLayout == HorizontalListViewLayout.Grid)
            {
                recyclerView.SetLayoutManager(
                    Element.GridColumnCount == 0
                        ? new ResponsiveGridLayoutManager(Context, Element.ItemWidth, Element.ItemSpacing)
                        : (GridLayoutManager)new CustomGridLayoutManager(Context, Element.GridColumnCount));
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
                var snapHelper = Element.SnapStyle == SnapStyle.Start ? new StartSnapHelper() : new LinearSnapHelper();
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
            if (Control == null)
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
            else if (GridLayoutManager != null && GridLayoutManager is CustomGridLayoutManager customGridLayoutManager)
            {
                customGridLayoutManager.CanScroll = !Element.DisableScroll;
            }
            else if (GridLayoutManager != null
                && GridLayoutManager is ResponsiveGridLayoutManager responsiveGridLayoutManager)
            {
                responsiveGridLayoutManager.CanScroll = !Element.DisableScroll;
            }
        }

        private void ScrollToCurrentItem()
        {
            HorizontalLinearLayoutManager?.ScrollToPositionWithOffset(Element.CurrentIndex, 0);
        }

        private void UpdateItemsSource()
        {
            Control.GetAdapter()?.Dispose();

            var adapter = new RecycleViewAdapter(Element, Context);
            Control.SetAdapter(adapter);

            if (Element.EnableDragAndDrop)
            {
                var dragHelper = new ItemTouchHelper(new DragAnDropItemTouchHelperCallback(Element, adapter, Element.DragAndDropEndedCommand));
                dragHelper.AttachToRecyclerView(Control);
            }
        }
    }
}