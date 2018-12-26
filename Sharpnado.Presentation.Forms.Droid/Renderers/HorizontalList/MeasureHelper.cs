using System;
using Sharpnado.Presentation.Forms.Droid.Helpers;
using Sharpnado.Presentation.Forms.RenderedViews;

public static class MeasureHelper
{
    public static int ComputeSpan(int availableWidth, HorizontalListView element)
    {
        int itemSpace = PlatformHelper.DpToPixels(element.ItemSpacing);

        int leftPadding = element.CollectionPadding.Left > 0
            ? PlatformHelper.DpToPixels(element.CollectionPadding.Left)
            : itemSpace;

        int rightPadding = element.CollectionPadding.Right > 0
            ? PlatformHelper.DpToPixels(element.CollectionPadding.Right)
            : itemSpace;

        int itemWidth = PlatformHelper.DpToPixels(element.ItemWidth);

        int columnCount = 0;
        while (true)
        {
            columnCount++;
            int interPadding = itemSpace * (columnCount - 1);
            int totalWidth = itemWidth * columnCount + interPadding + leftPadding + rightPadding;

            if (totalWidth > availableWidth)
            {
                break;
            }
        }

        if (--columnCount == 0)
        {
            throw new InvalidOperationException(
                "The CollectionPadding, ItemSpacing and ItemWidth specified doesn't allow a single column to be displayed");
        }

        return columnCount;
    }
}