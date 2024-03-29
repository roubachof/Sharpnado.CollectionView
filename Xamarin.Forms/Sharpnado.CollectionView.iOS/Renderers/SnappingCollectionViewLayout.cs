﻿using System;
using System.Runtime.InteropServices;

using CoreGraphics;

using Sharpnado.CollectionView;

using UIKit;

namespace Sharpnado.CollectionView.iOS.Renderers
{
    public class SnappingCollectionViewLayout : UICollectionViewFlowLayout
    {
        private readonly SnapStyle _snapStyle;

        public SnappingCollectionViewLayout(SnapStyle snapStyle)
        {
            _snapStyle = snapStyle;
        }

        public override CGPoint TargetContentOffset(CGPoint proposedContentOffset, CGPoint scrollingVelocity)
        {
            if (_snapStyle != SnapStyle.Start)
            {
                return base.TargetContentOffset(proposedContentOffset, scrollingVelocity);
            }

            NFloat offsetAdjustment = NFloat.MaxValue;
            NFloat horizontalOffset = proposedContentOffset.X + CollectionView.ContentInset.Left;

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