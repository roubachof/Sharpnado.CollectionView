using System;
using System.Linq;

using CoreGraphics;

using Foundation;

using Sharpnado.CollectionView.RenderedViews;

using UIKit;

namespace Sharpnado.CollectionView.iOS.Renderers
{
    public class SizedFlowLayout : UICollectionViewDelegateFlowLayout
    {
        private readonly WeakReference<RenderedViews.CollectionView> _weakElement;

        private int _lastVisibleItemIndex = -1;
        private bool _isScrolling;

        public SizedFlowLayout(RenderedViews.CollectionView collectionView)
        {
            _weakElement = new WeakReference<RenderedViews.CollectionView>(collectionView);
        }

        public bool IsCurrentIndexUpdateBackfire { get; private set; }

        public bool IsInternalScroll { get; set; }

        public override CGSize GetSizeForItem(
            UICollectionView collectionView,
            UICollectionViewLayout layout,
            NSIndexPath indexPath)
        {
            if (collectionView.DataSource is iOSViewSource dataSource)
            {
                (double itemWidth, double itemHeight) = dataSource.GetSize(indexPath.Row);

                return new CGSize(itemWidth, itemHeight);
            }

            return new CGSize(0, 0);
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            if (scrollView is not UICollectionView collectionView || !_weakElement.TryGetTarget(out var element))
            {
                return;
            }

            var infiniteListLoader = element.InfiniteListLoader;
            if (infiniteListLoader != null)
            {
                int lastVisibleIndex =
                    collectionView.IndexPathsForVisibleItems
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

            if (IsInternalScroll)
            {
                IsInternalScroll = false;
                return;
            }

            if (_isScrolling)
            {
                return;
            }

            _isScrolling = true;
            element.ScrollBeganCommand?.Execute(null);
        }

        public override void ScrollAnimationEnded(UIScrollView scrollView)
        {
            OnStopScrolling((UICollectionView)scrollView);
        }

        public override void DecelerationEnded(UIScrollView scrollView)
        {
            if (scrollView is not UICollectionView collectionView || !_weakElement.TryGetTarget(out var element))
            {
                return;
            }

            if (element.SnapStyle == SnapStyle.Center)
            {
                SnapToCenter(collectionView);
            }

            OnStopScrolling(collectionView);
        }

        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            if (scrollView is not UICollectionView collectionView || !_weakElement.TryGetTarget(out var element))
            {
                return;
            }

            if (!willDecelerate)
            {
                if (element.SnapStyle == SnapStyle.Center)
                {
                    SnapToCenter(collectionView);
                }

                OnStopScrolling(collectionView);
            }
        }

        private void OnStopScrolling(UICollectionView collectionView)
        {
            if (collectionView == null || !_weakElement.TryGetTarget(out var element))
            {
                return;
            }

            if (!_isScrolling)
            {
                return;
            }

            _isScrolling = false;

            IsCurrentIndexUpdateBackfire = true;
            try
            {
                UpdateCurrentIndex(collectionView);
                element.ScrollEndedCommand?.Execute(null);
            }
            finally
            {
                IsCurrentIndexUpdateBackfire = false;
            }
        }

        private bool IsCellFullyVisible(UICollectionView collectionView, NSIndexPath path)
        {
            var cell = collectionView.CellForItem(path);

            if (cell != null)
            {
                var firstCellFrame = collectionView.ConvertRectToView(cell.Frame, collectionView.Superview);
                if (collectionView.Frame.Contains(firstCellFrame))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsFirstCellFullyVisible(UICollectionView collectionView)
        {
            return IsCellFullyVisible(collectionView, NSIndexPath.FromItemSection(0, 0));
        }

        private bool IsLastCellFullyVisible(UICollectionView collectionView)
        {
            var lastIndex = NSIndexPath.FromItemSection(collectionView.NumberOfItemsInSection(0) - 1, 0);
            return IsCellFullyVisible(collectionView, lastIndex);
        }

        private void SnapToCenter(UICollectionView collectionView)
        {
            if (collectionView == null || !_weakElement.TryGetTarget(out var element))
            {
                return;
            }

            var firstIndex = NSIndexPath.FromItemSection(0, 0);
            if (IsCellFullyVisible(collectionView, firstIndex))
            {
                // Check if first item is fully visible, if true don't snap.
                collectionView.ScrollToItem(firstIndex, UICollectionViewScrollPosition.CenteredHorizontally, true);
                return;
            }

            var lastIndex = NSIndexPath.FromItemSection(collectionView.NumberOfItemsInSection(0) - 1, 0);
            if (IsCellFullyVisible(collectionView, lastIndex))
            {
                // Check if last item is fully visible, if true don't snap.
                collectionView.ScrollToItem(lastIndex, UICollectionViewScrollPosition.CenteredHorizontally, true);
                return;
            }

            var collectionViewCenter = collectionView.Center;
            var contentOffset = collectionView.ContentOffset;
            var center = new CGPoint(
                collectionViewCenter.X
                + contentOffset.X
                + (nfloat)element.CollectionPadding.Left
                - (nfloat)element.CollectionPadding.Right,
                collectionViewCenter.Y
                + contentOffset.Y
                + (nfloat)element.CollectionPadding.Top
                - (nfloat)element.CollectionPadding.Bottom);

            var indexPath = collectionView.IndexPathForItemAtPoint(center);
            if (indexPath == null)
            {
                // Point is right between two cells: picking one
                var indexes = collectionView.IndexPathsForVisibleItems.OrderBy(i => i.Item).ToArray();
                if (indexes.Length > 0)
                {
                    int middleIndex = (indexes.Count() - 1) / 2;
                    var candidateIndexPath = indexes[middleIndex];
                    if (candidateIndexPath.Row < element.CurrentIndex)
                    {
                        indexPath = candidateIndexPath;
                    }
                    else
                    {
                        indexPath = indexes[middleIndex + 1 > indexes.Length ? middleIndex : middleIndex + 1];
                    }
                }
            }

            collectionView.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredHorizontally, true);
        }

        private void UpdateCurrentIndex(UICollectionView collectionView)
        {
            if (collectionView == null || !_weakElement.TryGetTarget(out var element))
            {
                return;
            }

            nint newIndex;
            if (element.SnapStyle == SnapStyle.Center)
            {
                var lastIndex = NSIndexPath.FromItemSection(collectionView.NumberOfItemsInSection(0) - 1, 0);
                if (IsCellFullyVisible(collectionView, lastIndex))
                {
                    newIndex = lastIndex.Item;
                }
                else if (IsFirstCellFullyVisible(collectionView))
                {
                    newIndex = 0;
                }
                else
                {
                    var collectionViewCenter = collectionView.Center;
                    var contentOffset = collectionView.ContentOffset;
                    var center = new CGPoint(
                        collectionViewCenter.X
                        + contentOffset.X
                        + (nfloat)element.CollectionPadding.Left
                        - (nfloat)element.CollectionPadding.Right,
                        collectionViewCenter.Y 
                        + contentOffset.Y 
                        + (nfloat)element.CollectionPadding.Top 
                        - (nfloat)element.CollectionPadding.Bottom);

                    var centerPath = collectionView.IndexPathForItemAtPoint(center);
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
                    X = collectionView.ContentOffset.X,
                    Y = collectionView.ContentOffset.Y,
                    Size = new CGSize(element.ItemWidth, element.ItemHeight),
                };

                var firstCellCenter = new CGPoint(firstCellBounds.GetMidX(), firstCellBounds.GetMidY());

                var indexPath = collectionView.IndexPathForItemAtPoint(firstCellCenter);
                if (indexPath == null)
                {
                    return;
                }

                newIndex = indexPath.Row;
            }

            InternalLogger.Info($"UpdateCurrentIndex => {newIndex}");

            element.CurrentIndex = (int)newIndex;
        }
    }
}