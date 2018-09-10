using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
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
                    Control.Scrolled -= OnStartScrolling;

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

            // System.Diagnostics.Debug.WriteLine($"ScrollToCurrentItem( Element.CurrentIndex = {Element.CurrentIndex} )");
            _isInternalScroll = true;
            Control.ScrollToItem(
                NSIndexPath.FromRowSection(Element.CurrentIndex, 0),
                UICollectionViewScrollPosition.Left,
                false);
        }

        private void ProcessDisableScroll()
        {
            if (Element.ListLayout == HorizontalListViewLayout.Grid)
            {
                return;
            }

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

            SetNativeControl(collectionView);
            UpdateItemsSource();

            if (Element.SnapStyle == SnapStyle.Center)
            {
                Control.DraggingEnded += OnDraggingEnded;
                Control.DecelerationEnded += OnDecelerationEnded;
            }

            Control.Scrolled += OnStartScrolling;
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

        private void OnDraggingEnded(object sender, DraggingEventArgs e)
        {
            if (!e.Decelerate)
            {
                SnapToCenter();
            }
        }

        private void OnStartScrolling(object sender, EventArgs e)
        {
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

            // System.Diagnostics.Debug.WriteLine($"UpdateCurrentIndex => {indexPath.Row}");
            Element.CurrentIndex = indexPath.Row;
        }

        private void UpdateItemsSource()
        {
            // System.Diagnostics.Debug.WriteLine("UpdateItemsSource");
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
                    Control.InsertItems(new[] { NSIndexPath.FromRowSection(e.NewStartingIndex, 0) });
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Control.DeleteItems(new[] { NSIndexPath.FromRowSection(e.OldStartingIndex, 0) });
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