using System;
using Sharpnado.Presentation.Forms.RenderedViews;

public static class MeasureHelper
{
    public static int ComputeSpan(int availableWidth, HorizontalListView element)
    {
        int itemSpace = PlatformHelper.Instance.DpToPixels(element.ItemSpacing);
        int leftPadding = PlatformHelper.Instance.DpToPixels(element.CollectionPadding.Left);
        int rightPadding = PlatformHelper.Instance.DpToPixels(element.CollectionPadding.Right);

        int itemWidth = PlatformHelper.Instance.DpToPixels(element.ItemWidth);

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