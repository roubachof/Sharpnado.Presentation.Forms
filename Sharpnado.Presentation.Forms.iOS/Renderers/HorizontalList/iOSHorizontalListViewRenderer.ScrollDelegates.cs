using System;
using System.Linq;
using CoreGraphics;
using Sharpnado.Infrastructure;
using UIKit;

namespace Sharpnado.Presentation.Forms.iOS.Renderers.HorizontalList
{
    public partial class iOSHorizontalListViewRenderer
    {
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