using System;
using System.Linq;
using CoreGraphics;

using Foundation;

using Sharpnado.Infrastructure;
using UIKit;

namespace Sharpnado.Presentation.Forms.iOS.Renderers.HorizontalList
{
    public partial class iOSHorizontalListViewRenderer
    {
        private bool IsCellFullyVisible(NSIndexPath path)
        {
            var cell = Control.CellForItem(path);

            if (cell != null)
            {
                var firstCellFrame = Control.ConvertRectToView(cell.Frame, Control.Superview);
                if (Control.Frame.Contains(firstCellFrame))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsFirstCellFullyVisible()
        {
            return IsCellFullyVisible(NSIndexPath.FromItemSection(0, 0));
        }

        private bool IsLastCellFullyVisible()
        {
            var lastIndex = NSIndexPath.FromItemSection(Control.NumberOfItemsInSection(0) - 1, 0);
            return IsCellFullyVisible(lastIndex);
        }

        private void SnapToCenter()
        {
            if (Control == null)
            {
                return;
            }

            var firstIndex = NSIndexPath.FromItemSection(0, 0);
            if (IsCellFullyVisible(firstIndex))
            {
                // Check if first item is fully visible, if true don't snap.
                Control.ScrollToItem(firstIndex, UICollectionViewScrollPosition.CenteredHorizontally, true);
                return;
            }

            var lastIndex = NSIndexPath.FromItemSection(Control.NumberOfItemsInSection(0) - 1, 0);
            if (IsCellFullyVisible(lastIndex))
            {
                // Check if last item is fully visible, if true don't snap.
                Control.ScrollToItem(lastIndex, UICollectionViewScrollPosition.CenteredHorizontally, true);
                return;
            }

            var collectionViewCenter = Control.Center;
            var contentOffset = Control.ContentOffset;
            var center = new CGPoint(
                collectionViewCenter.X
                + contentOffset.X
                + (nfloat)Element.CollectionPadding.Left
                - (nfloat)Element.CollectionPadding.Right,
                collectionViewCenter.Y
                + contentOffset.Y
                + (nfloat)Element.CollectionPadding.Top
                - (nfloat)Element.CollectionPadding.Bottom);

            var indexPath = Control.IndexPathForItemAtPoint(center);
            if (indexPath == null)
            {
                // Point is right between two cells: picking one
                var indexes = Control.IndexPathsForVisibleItems.OrderBy(i => i.Item).ToArray();
                if (indexes.Length > 0)
                {
                    int middleIndex = (indexes.Count() - 1) / 2;
                    var candidateIndexPath = indexes[middleIndex];
                    if (candidateIndexPath.Row < Element.CurrentIndex)
                    {
                        indexPath = candidateIndexPath;
                    }
                    else
                    {
                        indexPath = indexes[middleIndex + 1 > indexes.Length ? middleIndex : middleIndex + 1];
                    }
                }
            }

            Control.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredHorizontally, true);
        }

        private void UpdateCurrentIndex()
        {
            nint newIndex;
            if (Element.SnapStyle == RenderedViews.SnapStyle.Center)
            {
                var lastIndex = NSIndexPath.FromItemSection(Control.NumberOfItemsInSection(0) - 1, 0);
                if (IsCellFullyVisible(lastIndex))
                {
                    newIndex = lastIndex.Item;
                }
                else if (IsFirstCellFullyVisible())
                {
                    newIndex = 0;
                }
                else
                {
                    var collectionViewCenter = Control.Center;
                    var contentOffset = Control.ContentOffset;
                    var center = new CGPoint(
                        collectionViewCenter.X
                        + contentOffset.X
                        + (nfloat)Element.CollectionPadding.Left
                        - (nfloat)Element.CollectionPadding.Right,
                        collectionViewCenter.Y
                        + contentOffset.Y
                        + (nfloat)Element.CollectionPadding.Top
                        - (nfloat)Element.CollectionPadding.Bottom);

                    var centerPath = Control.IndexPathForItemAtPoint(center);
                    if (centerPath == null)
                    {
                        InternalLogger.Warn(
                            "Failed to find a NSIndexPath in SnapStyle center context: UpdateCurrentIndex returns nothing");
                        return;
                    }

                    newIndex = centerPath.Item;
                }
            }
            else
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

                newIndex = indexPath.Row;
            }

            InternalLogger.Info($"UpdateCurrentIndex => {newIndex}");

            Element.CurrentIndex = (int)newIndex;
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

        private void OnScrolled(object sender, EventArgs e)
        {
            if (Control == null)
            {
                return;
            }

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
            if (Control == null)
            {
                return;
            }

            if (!_isScrolling)
            {
                return;
            }

            _isScrolling = false;

            _isCurrentIndexUpdateBackfire = true;
            try
            {
                UpdateCurrentIndex();
                Element.ScrollEndedCommand?.Execute(null);
            }
            finally
            {
                _isCurrentIndexUpdateBackfire = false;
            }
        }
    }
}