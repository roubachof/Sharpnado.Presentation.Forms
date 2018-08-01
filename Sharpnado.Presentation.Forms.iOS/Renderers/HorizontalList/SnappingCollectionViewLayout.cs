using System;

using CoreGraphics;

using UIKit;

namespace ClassLibrary1.Renderers.HorizontalList
{
    public class SnappingCollectionViewLayout : UICollectionViewFlowLayout
    {
        public override CGPoint TargetContentOffset(CGPoint proposedContentOffset, CGPoint scrollingVelocity)
        {
            nfloat offsetAdjustment = nfloat.MaxValue;
            nfloat horizontalOffset = proposedContentOffset.X + CollectionView.ContentInset.Left;

            var targetRect = new CGRect(
                proposedContentOffset.X,
                0,
                CollectionView.Bounds.Size.Width,
                CollectionView.Bounds.Size.Height);

            var layoutAttributesArray = LayoutAttributesForElementsInRect(targetRect);

            foreach (var layoutAttributes in layoutAttributesArray)
            {
                var itemOffset = layoutAttributes.Frame.X;
                if (Math.Abs(itemOffset - horizontalOffset) < Math.Abs(offsetAdjustment))
                {
                    offsetAdjustment = itemOffset - horizontalOffset;
                }
            }

            return new CGPoint(proposedContentOffset.X + offsetAdjustment, proposedContentOffset.Y);
        }
    }
}