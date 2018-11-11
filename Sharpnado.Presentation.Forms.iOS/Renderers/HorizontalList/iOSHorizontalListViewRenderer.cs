using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
using Foundation;

using Sharpnado.Infrastructure;
using Sharpnado.Presentation.Forms.iOS.Renderers.HorizontalList;
using Sharpnado.Presentation.Forms.RenderedViews;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HorizontalListView), typeof(iOSHorizontalListViewRenderer))]
namespace Sharpnado.Presentation.Forms.iOS.Renderers.HorizontalList
{
    [Preserve]
    public partial class iOSHorizontalListViewRenderer : ViewRenderer<HorizontalListView, UICollectionView>
    {
        private IEnumerable _itemsSource;

        private bool _isScrolling;
        private bool _isCurrentIndexUpdateBackfire;
        private bool _isInternalScroll;
        private bool _isMovedBackfire;

        private int _lastVisibleItemIndex = -1;

        public static void Initialize()
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(HorizontalListView.ItemsSource):
                    UpdateItemsSource();
                    break;
                case nameof(HorizontalListView.IsVisible):
                    CreateView();
                    break;
                case nameof(HorizontalListView.CurrentIndex) when !_isCurrentIndexUpdateBackfire:
                    ScrollToCurrentItem();
                    break;
                case nameof(HorizontalListView.DisableScroll):
                    ProcessDisableScroll();
                    break;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HorizontalListView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                if (Control != null)
                {
                    Control.DecelerationEnded -= OnStopScrolling;
                    Control.ScrollAnimationEnded -= OnStopScrolling;
                    Control.Scrolled -= OnScrolled;

                    Control.DraggingEnded -= OnDraggingEnded;
                    Control.DecelerationEnded -= OnDecelerationEnded;

                    Control.DataSource?.Dispose();
                    Control.CollectionViewLayout?.Dispose();
                    Control.Dispose();
                }

                if (_itemsSource is INotifyCollectionChanged oldNotifyCollection)
                {
                    oldNotifyCollection.CollectionChanged -= OnCollectionChanged;
                }
            }

            if (e.NewElement != null)
            {
                CreateView();
            }
        }

        private void ScrollToCurrentItem()
        {
            if (Element.ListLayout == HorizontalListViewLayout.Grid || Control.NumberOfItemsInSection(0) == 0)
            {
                return;
            }

            InternalLogger.Info($"ScrollToCurrentItem( Element.CurrentIndex = {Element.CurrentIndex} )");
            _isInternalScroll = true;
            Control.ScrollToItem(
                NSIndexPath.FromRowSection(Element.CurrentIndex, 0),
                UICollectionViewScrollPosition.Left,
                false);
        }

        private void ProcessDisableScroll()
        {
            Control.ScrollEnabled = !Element.DisableScroll;
        }

        private void CreateView()
        {
            if (Control == null && !Element.IsVisible)
            {
                return;
            }

            if (Control != null && !Element.IsVisible)
            {
                Control.Hidden = true;
                return;
            }
            Element.OnScrollRequested += Element_OnScrollRequested;
            Control?.DataSource?.Dispose();
            Control?.CollectionViewLayout?.Dispose();
            Control?.Dispose();

            var sectionInset = new UIEdgeInsets(
                (nfloat)Element.CollectionPadding.Top,
                (nfloat)Element.CollectionPadding.Left,
                (nfloat)Element.CollectionPadding.Bottom,
                (nfloat)Element.CollectionPadding.Right);

            var layout = Element.ListLayout == HorizontalListViewLayout.Grid
                ? new UICollectionViewFlowLayout
                {
                    ScrollDirection = UICollectionViewScrollDirection.Vertical,
                    ItemSize = new CGSize(Element.ItemWidth, Element.ItemHeight),
                    MinimumInteritemSpacing = Element.ItemSpacing,
                    MinimumLineSpacing = Element.ItemSpacing,
                    SectionInset = sectionInset,
                }
                : new SnappingCollectionViewLayout(Element.SnapStyle)
                {
                    ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                    ItemSize = new CGSize(Element.ItemWidth, Element.ItemHeight),
                    MinimumInteritemSpacing = Element.ItemSpacing,
                    MinimumLineSpacing = Element.ItemSpacing,
                    SectionInset = sectionInset,
                };

            var rect = new CGRect(0, 0, Element.ItemWidth, Element.ItemHeight);
            var collectionView = new UICollectionView(rect, layout)
            {
                DecelerationRate = UIScrollView.DecelerationRateFast,
                BackgroundColor = Element?.BackgroundColor.ToUIColor(),
                ShowsHorizontalScrollIndicator = false,
                ContentInset = new UIEdgeInsets(0, 0, 0, 0),
            };

            // Otherwise the UICollectionView doesn't seem to take enough space
            Element.HeightRequest = Element.ItemHeight
                + Element.CollectionPadding.VerticalThickness
                + Element.Margin.VerticalThickness;

            SetNativeControl(collectionView);
            UpdateItemsSource();

            if (Element.SnapStyle == SnapStyle.Center)
            {
                Control.DraggingEnded += OnDraggingEnded;
                Control.DecelerationEnded += OnDecelerationEnded;
            }

            Control.Scrolled += OnScrolled;
            Control.ScrollAnimationEnded += OnStopScrolling;
            Control.DecelerationEnded += OnStopScrolling;

            if (Element.EnableDragAndDrop)
            {
                EnableDragAndDrop();
            }

            ScrollToCurrentItem();
            ProcessDisableScroll();
        }

        private void SnapToCenter()
        {
            var collectionRect = new CGRect
            {
                X = Control.ContentOffset.X,
                Y = Control.ContentOffset.Y,
                Size = new CGSize(Control.Frame.Width, Control.Frame.Height),
            };

            var collectionViewCenter = new CGPoint(collectionRect.GetMidX(), collectionRect.GetMidY());

            var indexPath = Control.IndexPathForItemAtPoint(collectionViewCenter);
            if (indexPath == null)
            {
                return;
            }

            Control.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredHorizontally, true);
        }

        private void OnDecelerationEnded(object sender, EventArgs e)
        {
            SnapToCenter();
        }

        void Element_OnScrollRequested(object sender, ScrollRequestedEventArgs e)
        {


            if (Element.ListLayout == HorizontalListViewLayout.Grid || Control.NumberOfItemsInSection(0) == 0 || e.Position >= Control.NumberOfItemsInSection(0))
            {
                return;
            }




            _isInternalScroll = true;

            UICollectionViewScrollPosition type = UICollectionViewScrollPosition.Left;

            switch(e.Type)
            {
                case ScrollType.Start:
                    type = UICollectionViewScrollPosition.Left;
                    break;
                case ScrollType.End:
                    type = UICollectionViewScrollPosition.Right;
                    break;
                case ScrollType.Center:
                    type = UICollectionViewScrollPosition.CenteredHorizontally;
                    break;
            }



            Control.ScrollToItem(
                NSIndexPath.FromRowSection(e.Position, 0),
                type,
                e.Animate);
        }


        private void OnDraggingEnded(object sender, DraggingEventArgs e)
        {
            if (!e.Decelerate)
            {
                SnapToCenter();
            }
        }

        private void OnScrolled(object sender, EventArgs e)
        {
            var infiniteListLoader = Element?.InfiniteListLoader;
            if (infiniteListLoader != null)
            {
                int lastVisibleIndex =
                    Control.IndexPathsForVisibleItems
                        .Select(path => path.Row)
                        .DefaultIfEmpty(-1)
                        .Max();

                if (_lastVisibleItemIndex == lastVisibleIndex)
                {
                    return;
                }

                _lastVisibleItemIndex = lastVisibleIndex;

                InternalLogger.Info($"OnScrolled( lastVisibleItem: {lastVisibleIndex} )");
                infiniteListLoader.OnScroll(lastVisibleIndex);
            }

            if (_isInternalScroll)
            {
                _isInternalScroll = false;
                return;
            }

            if (_isScrolling)
            {
                return;
            }

            _isScrolling = true;
            Element.ScrollBeganCommand?.Execute(null);
        }

        private void OnStopScrolling(object sender, EventArgs e)
        {
            _isScrolling = false;

            _isCurrentIndexUpdateBackfire = true;
            try
            {
                UpdateCurrentIndex();
            }
            finally
            {
                _isCurrentIndexUpdateBackfire = false;
            }
        }

        private void UpdateCurrentIndex()
        {
            var firstCellBounds = new CGRect
            {
                X = Control.ContentOffset.X,
                Y = Control.ContentOffset.Y,
                Size = new CGSize(Element.ItemWidth, Element.ItemHeight),
            };

            var firstCellCenter = new CGPoint(firstCellBounds.GetMidX(), firstCellBounds.GetMidY());

            var indexPath = Control.IndexPathForItemAtPoint(firstCellCenter);
            if (indexPath == null)
            {
                return;
            }

            InternalLogger.Info($"UpdateCurrentIndex => {indexPath.Row}");
            Element.CurrentIndex = indexPath.Row;
        }

        private void UpdateItemsSource()
        {
            InternalLogger.Info("UpdateItemsSource");
            Control.DataSource?.Dispose();
            Control.DataSource = null;

            if (_itemsSource is INotifyCollectionChanged oldNotifyCollection)
            {
                oldNotifyCollection.CollectionChanged -= OnCollectionChanged;
            }

            _itemsSource = Element.ItemsSource;
            if (_itemsSource == null)
            {
                return;
            }

            Control.DataSource = new iOSViewSource(Element);
            Control.RegisterClassForCell(typeof(iOSViewCell), nameof(iOSViewCell));

            if (_itemsSource is INotifyCollectionChanged newNotifyCollection)
            {
                newNotifyCollection.CollectionChanged += OnCollectionChanged;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isMovedBackfire)
            {
                return;
            }

            if (Control == null)
            {
                return;
            }

            if (Control.NumberOfItemsInSection(0) == ((IList)_itemsSource).Count)
            {
                return;
            }

            ((iOSViewSource)Control.DataSource).HandleNotifyCollectionChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var addedIndexPathes = new NSIndexPath[e.NewItems.Count];
                    for (int addedIndex = e.NewStartingIndex, index = 0; index < addedIndexPathes.Length; addedIndex++, index++)
                    {
                        addedIndexPathes[index] = NSIndexPath.FromRowSection(addedIndex, 0);
                    }

                    Control.InsertItems(addedIndexPathes);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var removedIndexPathes = new NSIndexPath[e.OldItems.Count];
                    for (int removedIndex = e.OldStartingIndex, index = 0; index < removedIndexPathes.Length; removedIndex++, index++)
                    {
                        removedIndexPathes[index] = NSIndexPath.FromRowSection(removedIndex, 0);
                    }

                    Control.DeleteItems(removedIndexPathes);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Control.ReloadData();
                    break;
                case NotifyCollectionChangedAction.Move:
                    Control.MoveItem(
                        NSIndexPath.FromRowSection(e.OldStartingIndex, 0),
                        NSIndexPath.FromRowSection(e.NewStartingIndex, 0));
                    break;
            }
        }
    }
}