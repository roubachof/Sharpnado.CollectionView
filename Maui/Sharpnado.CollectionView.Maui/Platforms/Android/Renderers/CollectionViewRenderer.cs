using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;

#if NET6_0_OR_GREATER
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
#endif

using Sharpnado.CollectionView;
using Sharpnado.CollectionView.Droid.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using CollectionView = Sharpnado.CollectionView.CollectionView;
using GridLayoutManager = AndroidX.RecyclerView.Widget.GridLayoutManager;

#if !NET6_0_OR_GREATER
[assembly: ExportRenderer(typeof(CollectionView), typeof(CollectionViewRenderer))]
#endif

namespace Sharpnado.CollectionView.Droid.Renderers
{
#if !NET6_0_OR_GREATER
    [Xamarin.Forms.Internals.Preserve]
#endif
    public partial class CollectionViewRenderer : ViewRenderer<CollectionView, RecyclerView>
    {
        private bool _isCurrentIndexUpdateBackfire;
        private bool _isLandscape;
        private bool _isFirstInitialization = true;

        private bool _forceLayout = false;
        private IEnumerable _itemsSource;
        private ItemTouchHelper _dragHelper;
        private SpaceItemDecoration _itemDecoration;
        private PreDrawListener _preDrawListener;

        public CollectionViewRenderer(Context context)
            : base(context)
        {
        }

        public CustomLinearLayoutManager HorizontalLinearLayoutManager => Control?.GetLayoutManager() as CustomLinearLayoutManager;

        public GridLayoutManager GridLayoutManager => Control?.GetLayoutManager() as GridLayoutManager;

        public LinearLayoutManager LinearLayoutManager => Control?.GetLayoutManager() as LinearLayoutManager;

        public bool IsScrolling { get; set; }

        public bool IsSnapHelperBusy { get; set; }

        public int CurrentSnapIndex { get; set; }

        public static void Initialize()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CollectionView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                if (_dragHelper != null)
                {
                    _dragHelper.AttachToRecyclerView(null);
                    _dragHelper = null;
                }

                if (!Control.IsNullOrDisposed())
                {
                    Control.ClearOnScrollListeners();
                    var treeViewObserver = Control.ViewTreeObserver;
                    if (treeViewObserver != null && _preDrawListener != null)
                    {
                        treeViewObserver.RemoveOnPreDrawListener(_preDrawListener);
                    }

                    Control.GetAdapter()?.Dispose();
                    Control.GetLayoutManager()?.Dispose();
                }

                if (_itemsSource is INotifyCollectionChanged oldNotifyCollection)
                {
                    oldNotifyCollection.CollectionChanged -= OnCollectionChanged;
                }
            }

            if (e.NewElement != null)
            {
                CreateView(e.NewElement);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CollectionView.ItemsSource):
                    UpdateItemsSource();
                    break;
                case nameof(CollectionView.CurrentIndex) when !_isCurrentIndexUpdateBackfire:
                    ScrollToCurrentItem();
                    break;
                case nameof(CollectionView.DisableScroll):
                    ProcessDisableScroll();
                    break;
                case nameof(CollectionView.ColumnCount):
                case nameof(CollectionView.CollectionLayout):
                    UpdateListLayout();
                    break;
                case nameof(CollectionView.EnableDragAndDrop):
                    UpdateEnableDragAndDrop();
                    break;
            }
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            if ((!changed && !_forceLayout)
                || Control == null
                || Element == null
                || (Element.ColumnCount == 0
                    && (Element.IsLayoutHorizontal && Element.ItemHeight > 0)))
            {
                base.OnLayout(changed, left, top, right, bottom);
                return;
            }

            int width = right - left;
            int height = bottom - top;

            base.OnLayout(changed, left, top, right, bottom);

            if (ComputeItemSize(width, height))
            {
                UpdateItemsSource();
            }

            // Initially was here
            // base.OnLayout(changed, left, top, right, bottom);

            _forceLayout = false;
        }

        private bool ComputeItemSize(int width, int height)
        {
            if (Control == null
                || Element == null
                || (Element.ColumnCount == 0
                    && (Element.IsLayoutHorizontal && Element.ItemHeight > 0)))
            {
                return false;
            }

            bool widthChanged = false;
            bool heightChanged = false;
            if (Element.ColumnCount > 0)
            {
                double newItemWidth = Element.ComputeItemWidth(width);
                if (Element.ItemWidth != newItemWidth)
                {
                    Element.ItemWidth = newItemWidth;
                    widthChanged = true;
                }

                if (Element.CollectionLayout == CollectionViewLayout.Grid || Element.CollectionLayout == CollectionViewLayout.Vertical)
                {
                    if (Control.GetLayoutManager() is ResponsiveGridLayoutManager layoutManager)
                    {
                        layoutManager.ResetSpan();
                        Control.InvalidateItemDecorations();
                    }
                }
            }

            if (Element.IsLayoutHorizontal && Element.ItemHeight == 0)
            {
                double newItemHeight = Element.ComputeItemHeight(height);
                if (Element.ItemHeight != newItemHeight)
                {
                    Element.ItemHeight = newItemHeight;
                    heightChanged = true;
                }
            }

            return widthChanged || heightChanged;
        }

        private void CreateView(CollectionView collection)
        {
            if (_isFirstInitialization)
            {
                Element.CheckConsistency();
                _isFirstInitialization = false;
            }

            var recyclerView = new SlowRecyclerView(Context, Element.ScrollSpeed) { HasFixedSize = false };

            SetListLayout(recyclerView);

            SetNativeControl(recyclerView);

            if (Element.SnapStyle != SnapStyle.None)
            {
                LinearSnapHelper snapHelper = Element.SnapStyle == SnapStyle.Start
                    ? new StartSnapHelper(this)
                    : new CenterSnapHelper(this);
                snapHelper.AttachToRecyclerView(Control);
            }

            Control.HorizontalScrollBarEnabled = false;

            if (Element.ItemsSource != null)
            {
                UpdateItemsSource();
            }

            if (LinearLayoutManager != null)
            {
                Control.AddOnScrollListener(new OnControlScrollChangedListener(this, collection));

                ProcessDisableScroll();

                if (HorizontalLinearLayoutManager != null)
                {
                    ScrollToCurrentItem();
                }
            }

            _preDrawListener = new PreDrawListener(this);
            Control.ViewTreeObserver.AddOnPreDrawListener(_preDrawListener);
        }

        private void SetListLayout(RecyclerView recyclerView)
        {
            if (Element.CollectionLayout == CollectionViewLayout.Grid || Element.CollectionLayout == CollectionViewLayout.Vertical)
            {
                recyclerView.SetLayoutManager(new ResponsiveGridLayoutManager(Context, Element));
            }
            else
            {
                recyclerView.SetLayoutManager(new CustomLinearLayoutManager(Context, OrientationHelper.Horizontal, false));
            }

            if (Element.ItemSpacing > 0 || Element.CollectionPadding != new Thickness(0))
            {
                if (!_itemDecoration.IsNullOrDisposed())
                {
                    recyclerView.RemoveItemDecoration(_itemDecoration);
                    _itemDecoration = null;
                }

                _itemDecoration = new SpaceItemDecoration(Element.ItemSpacing);
                recyclerView.AddItemDecoration(_itemDecoration);
                recyclerView.SetPadding(
                    PlatformHelper.Instance.DpToPixels(Element.CollectionPadding.Left),
                    PlatformHelper.Instance.DpToPixels(Element.CollectionPadding.Top),
                    PlatformHelper.Instance.DpToPixels(Element.CollectionPadding.Right),
                    PlatformHelper.Instance.DpToPixels(Element.CollectionPadding.Bottom));

                recyclerView.SetClipToPadding(false);
                recyclerView.SetClipChildren(false);
            }
        }

        private class PreDrawListener : Java.Lang.Object, ViewTreeObserver.IOnPreDrawListener
        {
            private readonly WeakReference<CollectionViewRenderer> _renderer;

            public PreDrawListener(CollectionViewRenderer renderer)
            {
                _renderer = new WeakReference<CollectionViewRenderer>(renderer);
            }

            public bool OnPreDraw()
            {
                if (_renderer.TryGetTarget(out CollectionViewRenderer target))
                {
                    target.OnPreDraw();
                }

                return true;
            }
        }

        private void OnPreDraw()
        {
            if (Control.IsNullOrDisposed())
            {
                return;
            }

            bool orientationChanged = false;
            if (Control.Height < Control.Width)
            {
                if (!_isLandscape)
                {
                    orientationChanged = true;
                    _isLandscape = true;

                    // Has just rotated
                    if (HorizontalLinearLayoutManager != null)
                    {
                        ScrollToCurrentItem();
                    }
                }
            }
            else
            {
                orientationChanged = _isLandscape;
                _isLandscape = false;
            }

            if (orientationChanged)
            {
                if (Control.GetLayoutManager() is ResponsiveGridLayoutManager layoutManager)
                {
                    layoutManager.ResetSpan();
                }

                Control.InvalidateItemDecorations();
            }
        }

        private void ProcessDisableScroll()
        {
            if (Control.IsNullOrDisposed())
            {
                return;
            }

            if (LinearLayoutManager == null)
            {
                return;
            }

            if (HorizontalLinearLayoutManager != null)
            {
                HorizontalLinearLayoutManager.CanScroll = !Element.DisableScroll;
            }
            else if (GridLayoutManager != null
                && GridLayoutManager is ResponsiveGridLayoutManager responsiveGridLayoutManager)
            {
                responsiveGridLayoutManager.CanScroll = !Element.DisableScroll;
            }
        }

        private void ScrollToCurrentItem()
        {
            if (Control.IsNullOrDisposed())
            {
                return;
            }

            if (Element.CurrentIndex == -1 || Control.GetAdapter() == null || Element.CurrentIndex >= Control.GetAdapter().ItemCount)
            {
                return;
            }

            InternalLogger.Info($"ScrollToCurrentItem() => CurrentItem: {Element.CurrentIndex}");

            int offset = 0;
            if (HorizontalLinearLayoutManager != null)
            {
                int itemWidth = PlatformHelper.Instance.DpToPixels(
                    Element.ItemWidth
                    + Element.ItemSpacing
                    + Element.CollectionPadding.HorizontalThickness);

                int width = Control.MeasuredWidth;

                switch (Element.SnapStyle)
                {
                    case SnapStyle.Center:
                        offset = (width / 2) - (itemWidth / 2);
                        break;
                }
            }

            LinearLayoutManager?.ScrollToPositionWithOffset(Element.CurrentIndex, offset);
            GridLayoutManager?.ScrollToPositionWithOffset(Element.CurrentIndex, offset);
        }

        private void UpdateEnableDragAndDrop()
        {
            if (Control.IsNullOrDisposed() || Control.GetAdapter().IsNullOrDisposed())
            {
                return;
            }

            _dragHelper?.AttachToRecyclerView(null);

            if (Element.EnableDragAndDrop)
            {
                _dragHelper = new ItemTouchHelper(
                    new DragAnDropItemTouchHelperCallback(
                        Element,
                        (RecycleViewAdapter)Control.GetAdapter(),
                        Element.DragAndDropStartedCommand,
                        Element.DragAndDropEndedCommand));
                _dragHelper.AttachToRecyclerView(Control);
            }

            var adapter = Control.GetAdapter();
            ((RecycleViewAdapter)adapter)?.OnEnableDragAndDropUpdated(Element.EnableDragAndDrop);
        }

        private void UpdateItemsSource()
        {
            InternalLogger.Info($"UpdateItemsSource()");

            if (Control.IsNullOrDisposed())
            {
                return;
            }

            var oldAdapter = Control.GetAdapter();

            if (_itemsSource is INotifyCollectionChanged oldNotifyCollection)
            {
                oldNotifyCollection.CollectionChanged -= OnCollectionChanged;
            }

            _itemsSource = Element.ItemsSource;

            var mauiContext = Element.Handler?.MauiContext;
            if (mauiContext == null)
            {
                InternalLogger.Error($"Element handler is null: cannot return maui context");
                return;
            }

            Control.GetRecycledViewPool().Clear();
            var adapter = new RecycleViewAdapter(Element, this, Control, Context, mauiContext);
            Control.SetAdapter(adapter);

            if (!oldAdapter.IsNullOrDisposed())
            {
                oldAdapter.Dispose();
            }

            if (_itemsSource is INotifyCollectionChanged newNotifyCollection)
            {
                newNotifyCollection.CollectionChanged += OnCollectionChanged;
            }

            UpdateEnableDragAndDrop();

            ScrollToCurrentItem();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UpdateItemsSource();
            }
        }

        private void UpdateListLayout()
        {
            if (Control.IsNullOrDisposed())
            {
                return;
            }

            _forceLayout = true;

            SetListLayout(Control);
            UpdateItemsSource();
            ProcessDisableScroll();
            ScrollToCurrentItem();
        }

        private void StartDragging(RecyclerView.ViewHolder viewHolder)
        {
            _dragHelper.StartDrag(viewHolder);
        }
    }
}