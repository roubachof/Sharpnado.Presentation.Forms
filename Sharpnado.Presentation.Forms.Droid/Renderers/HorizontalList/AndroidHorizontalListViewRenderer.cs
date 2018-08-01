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
    public partial class AndroidHorizontalListViewRenderer : ViewRenderer<HorizontalListView, RecyclerView>
    {
        private bool _isCurrentIndexUpdateBackfire;
        private bool _isLandscape;

        public AndroidHorizontalListViewRenderer(Context context)
            : base(context)
        {
        }

        public CustomLinearLayoutManager LinearLayoutManager => Control?.GetLayoutManager() as CustomLinearLayoutManager;

        public bool IsScrolling { get; set; }

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
                        : new GridLayoutManager(Context, Element.GridColumnCount));

                if (Element.ItemSpacing > 0)
                {
                    recyclerView.AddItemDecoration(new SpaceItemDecoration(Element.ItemSpacing));
                }
            }
            else
            {
                recyclerView.SetLayoutManager(new CustomLinearLayoutManager(Context, OrientationHelper.Horizontal, false));
            }

            SetNativeControl(recyclerView);

            var snapHelper = new StartSnapHelper();
            snapHelper.AttachToRecyclerView(Control);

            Control.HorizontalScrollBarEnabled = false;

            if (Element.ItemsSource != null)
            {
                UpdateItemsSource();
            }

            if (Element.ViewCacheSize != 10)
            {
                Control.AddOnScrollListener(new OnControlScrollChangedListener(this, horizontalList));
                Control.ViewTreeObserver.PreDraw += OnPreDraw;

                ProcessDisableScroll();
                ScrollToCurrentItem();
            }
        }

        private void OnPreDraw(object sender, ViewTreeObserver.PreDrawEventArgs e)
        {
            if (Control == null)
            {
                return;
            }

            if (Control.Height < Control.Width)
            {
                if (!_isLandscape)
                {
                    _isLandscape = true;

                    // Has just rotated
                    ScrollToCurrentItem();
                }

                _isLandscape = true;
            }
            else
            {
                _isLandscape = false;
            }
        }

        private void ProcessDisableScroll()
        {
            LinearLayoutManager.CanScroll = !Element.DisableScroll;
        }

        private void ScrollToCurrentItem()
        {
            if (!_isLandscape)
            {
                return;
            }

            LinearLayoutManager.ScrollToPositionWithOffset(Element.CurrentIndex, 0);
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