using System;
using System.Threading;

using Android.Runtime;

using AndroidX.RecyclerView.Widget;

using Sharpnado.CollectionView.RenderedViews;

namespace Sharpnado.CollectionView.Droid.Renderers
{
    public partial class CollectionViewRenderer
    {
        private class OnControlScrollChangedListener : RecyclerView.OnScrollListener
        {
            private readonly WeakReference<CollectionViewRenderer> _weakNativeView;
            private readonly CollectionView.RenderedViews.CollectionView _element;

            private CancellationTokenSource _cts;
            private int _lastVisibleItemIndex = -1;

            public OnControlScrollChangedListener(IntPtr handle, JniHandleOwnership transfer)
                : base(handle, transfer)
            {
            }

            public OnControlScrollChangedListener(
                CollectionViewRenderer nativeView,
                CollectionView.RenderedViews.CollectionView element)
            {
                _weakNativeView = new WeakReference<CollectionViewRenderer>(nativeView);
                _element = element;
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                base.OnScrolled(recyclerView, dx, dy);

                if (dx > 0)
                {
                    _element.ScrollingRightCommand?.Execute(null);
                }
                else if (dx < 0)
                {
                    _element.ScrollingLeftCommand?.Execute(null);
                }

                if (dy > 0)
                {
                    _element.ScrollingDownCommand?.Execute(null);
                }
                else if (dy < 0)
                {
                    _element.ScrollingUpCommand?.Execute(null);
                }

                var infiniteListLoader = _element?.InfiniteListLoader;
                if (infiniteListLoader == null)
                {
                    return;
                }

                var linearLayoutManager = (LinearLayoutManager)recyclerView.GetLayoutManager();
                int lastVisibleItem = linearLayoutManager.FindLastVisibleItemPosition();
                if (_lastVisibleItemIndex == lastVisibleItem)
                {
                    return;
                }

                _lastVisibleItemIndex = lastVisibleItem;

                // InternalLogger.Info($"OnScrolled( lastVisibleItem: {lastVisibleItem} )");
                infiniteListLoader.OnScroll(lastVisibleItem);
            }

            public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
            {
                InternalLogger.Info($"OnScrollStateChanged( newState: {newState} )");
                switch (newState)
                {
                    case RecyclerView.ScrollStateDragging:
                    {
                        if (!_weakNativeView.TryGetTarget(out CollectionViewRenderer nativeView))
                        {
                            return;
                        }

                        if (nativeView.IsScrolling)
                        {
                            return;
                        }

                        if (_cts != null && !_cts.IsCancellationRequested)
                        {
                            // System.Diagnostics.Debug.WriteLine("DEBUG_SCROLL: Cancelling previous update index task");
                            _cts.Cancel();
                        }

                        nativeView.IsScrolling = true;

                        // System.Diagnostics.Debug.WriteLine("DEBUG_SCROLL: BeginScroll command");
                        _element.ScrollBeganCommand?.Execute(null);
                        break;
                    }

                    case RecyclerView.ScrollStateSettling:
                    {
                        if (!_weakNativeView.TryGetTarget(out CollectionViewRenderer nativeView))
                        {
                            return;
                        }

                        nativeView.IsScrolling = true;
                        break;
                    }

                    case RecyclerView.ScrollStateIdle:
                    {
                        if (!_weakNativeView.TryGetTarget(out CollectionViewRenderer nativeView))
                        {
                            return;
                        }

                        if (!nativeView.IsScrolling)
                        {
                            // System.Diagnostics.Debug.WriteLine("DEBUG_SCROLL: returning !nativeView.IsScrolling");
                            return;
                        }

                        if (nativeView.IsSnapHelperBusy)
                        {
                            // System.Diagnostics.Debug.WriteLine("DEBUG_SCROLL: returning nativeView.IsSnapHelperBusy");
                            return;
                        }

                        nativeView.IsScrolling = false;

                        _cts = new CancellationTokenSource();
                        UpdateCurrentIndex(nativeView, _cts.Token);
                        _element.ScrollEndedCommand?.Execute(null);

                        break;
                    }
                }
            }

            private void UpdateCurrentIndex(
                CollectionViewRenderer nativeView,
                CancellationToken token)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (nativeView?.LinearLayoutManager == null || _element == null)
                {
                    return;
                }

                nativeView._isCurrentIndexUpdateBackfire = true;
                try
                {
                    int newIndex = -1;
                    if (_element.SnapStyle != SnapStyle.None)
                    {
                        newIndex = nativeView.CurrentSnapIndex;
                    }
                    else
                    {
                        newIndex = nativeView.LinearLayoutManager.FindFirstCompletelyVisibleItemPosition();
                        if (newIndex == -1)
                        {
                            newIndex = nativeView.LinearLayoutManager.FindFirstVisibleItemPosition();
                        }
                    }

                    if (newIndex == -1)
                    {
                        InternalLogger.Warn(
                            "Failed to find the current index: UpdateCurrentIndex returns nothing");
                        return;
                    }

                    _element.CurrentIndex = newIndex;
                    InternalLogger.Info($"CurrentIndex: {_element.CurrentIndex}");
                }
                finally
                {
                    nativeView._isCurrentIndexUpdateBackfire = false;
                }
            }
        }
    }
}