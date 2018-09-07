using System;
using CoreGraphics;
using UIKit;

namespace Sharpnado.Presentation.Forms.iOS.Renderers.HorizontalList
{
    public class CenteredCollectionViewFlowLayout : UICollectionViewFlowLayout
    {
        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
        {
            var result = base.LayoutAttributesForElementsInRect(rect);

            int columnCount = ComputeColumnCount(result);
            nfloat centerXDelta = rect.Width / (columnCount + 1);
            nfloat nextCenterX = centerXDelta;
            nfloat previousRight = -1;

            UICollectionViewLayoutAttributes[] clonedResult = new UICollectionViewLayoutAttributes[result.Length];
            for (int index = 0; index < result.Length; index++)
            {
                clonedResult[index] = (UICollectionViewLayoutAttributes)result[index].Copy();
            }

            foreach (var layoutAttribute in clonedResult)
            {
                if (layoutAttribute.Frame.Left < previousRight)
                {
                    // new line
                    nextCenterX = centerXDelta;
                }

                previousRight = layoutAttribute.Frame.Right;

                layoutAttribute.Center = new CGPoint(nextCenterX, layoutAttribute.Center.Y);

                nextCenterX += centerXDelta;
            }

            return clonedResult;
        }

        private int ComputeColumnCount(UICollectionViewLayoutAttributes[] attributes)
        {
            int columnCount = 0;
            nfloat previousRight = -1;
            foreach (var layoutAttribute in attributes)
            {
                if (layoutAttribute.Frame.Left < previousRight)
                {
                    break;
                }

                previousRight = layoutAttribute.Frame.Right;
                columnCount++;
            }

            return columnCount;
        }

    }
}