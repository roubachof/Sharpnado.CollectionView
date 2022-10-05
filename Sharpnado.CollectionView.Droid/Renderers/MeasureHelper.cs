using Sharpnado.CollectionView.RenderedViews;

namespace Sharpnado.CollectionView.Droid.Renderers
{
    public static class MeasureHelper
    {
        public const int RecyclerViewItemVerticalMarginDp = 0;

        public static int ComputeSpan(int availableWidth, CollectionView.RenderedViews.CollectionView element)
        {
            int itemSpace = PlatformHelper.Instance.DpToPixels(element.ItemSpacing);
            int leftPadding = PlatformHelper.Instance.DpToPixels(element.CollectionPadding.Left);
            int rightPadding = PlatformHelper.Instance.DpToPixels(element.CollectionPadding.Right);

            if (element.ItemWidth == 0)
            {
                element.ItemWidth = element.ComputeItemWidth(availableWidth);
            }

            int itemWidth = PlatformHelper.Instance.DpToPixels(element.ItemWidth, PlatformHelper.Rounding.Floor);

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
                InternalLogger.Error(
                    "The CollectionPadding, ItemSpacing and ItemWidth specified doesn't allow a single column to be displayed");
                return 1;
            }

            return columnCount;
        }
    }
}