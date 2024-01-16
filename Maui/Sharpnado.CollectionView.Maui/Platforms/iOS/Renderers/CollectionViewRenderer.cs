using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

using CoreGraphics;

using Foundation;

using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;

using Sharpnado.CollectionView.iOS.Helpers;
using Sharpnado.Tasks;
using UIKit;

using ContentView = Microsoft.Maui.Controls.ContentView;

namespace Sharpnado.CollectionView.iOS.Renderers
{
    [Foundation.Preserve]
    public partial class CollectionViewRenderer : Microsoft.Maui.Controls.Handlers.Compatibility.ViewRenderer<CollectionView, UICollectionView>
    {
        private readonly List<DataTemplate> _registeredDataTemplates = new ();

        private IEnumerable _itemsSource;
        private UICollectionView _collectionView;

        private bool _isMovedBackfire;
        private bool _isFirstInitialization = true;
        private bool _isRefreshViewUserEnabled = false;

        private bool _isDisposed = false;

        private int _lastVisibleItemIndex = -1;

        public CollectionViewRenderer()
        {
            AutoPackage = false;
        }

        public static void Initialize()
        {
        }

        public override void LayoutSubviews()
        {
            double height = Bounds.Height;
            double width = Bounds.Width;

            InternalLogger.Debug($"LayoutSubviews( bounds: {Bounds} )");

            if (_collectionView == null || height <= 0 || width <= 0)
            {
                return;
            }

            _collectionView.Frame = new CGRect(0, 0, width, height);

            if (Control == null)
            {
                SetCollectionView(_collectionView);
            }

            if (ComputeItemSize(width, height) && _collectionView.CollectionViewLayout is UICollectionViewFlowLayout flowLayout)
            {
                flowLayout.ItemSize = new CGSize(Element.ItemWidth, Element.ItemHeight);
                UpdateItemsSource();
                ScrollToCurrentItem();
            }

            base.LayoutSubviews();

            InternalLogger.Debug($"collectionView bounds: {_collectionView.Bounds}");
        }

        protected override void Dispose(bool disposing)
        {
            _isDisposed = true;

            CleanUp();

            base.Dispose(disposing);
        }

#if NET6_0_OR_GREATER
        protected override void DisconnectHandler(UICollectionView oldNativeView)
        {
            CleanUp();

            base.DisconnectHandler(oldNativeView);
        }
#endif

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CollectionView.Height):
                case nameof(CollectionView.Width):
                    InternalLogger.Debug(() => $"Maui width: {Element.Width}, height: {Element.Height}");
                    break;

                case nameof(CollectionView.ItemsSource):
                    UpdateItemsSource();
                    break;
                case nameof(CollectionView.CurrentIndex) when Control?.Delegate is SizedFlowLayout
                {
                    IsCurrentIndexUpdateBackfire: false
                }:
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

        protected override void OnElementChanged(ElementChangedEventArgs<CollectionView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                CleanUp();
            }

            if (e.NewElement != null)
            {
                CreateView();
            }
        }

        private void CleanUp()
        {
            if (_collectionView != null)
            {
                _collectionView.Dispose();
                _collectionView.DataSource?.Dispose();
                _collectionView.CollectionViewLayout?.Dispose();
                _collectionView.Delegate?.Dispose();
                _collectionView = null;
            }

            if (_itemsSource is INotifyCollectionChanged oldNotifyCollection)
            {
                oldNotifyCollection.CollectionChanged -= OnCollectionChanged;
            }

            _dragAndDropGesture?.Dispose();
            _dragAndDropGesture = null;
            _itemsSource = null;
            _registeredDataTemplates.Clear();
        }

        private bool ComputeItemSize(double width, double height)
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
            }

            if (Element.IsLayoutHorizontal && Element.ItemHeight == 0)
            {
                double newItemHeight = Element.ComputeItemHeight(height);
                if (Element.ItemHeight != newItemHeight)
                {
                    Element.ItemHeight = newItemHeight;
                    heightChanged = true;

                    Element.HeightRequest = Element.ItemHeight
                        + Element.CollectionPadding.VerticalThickness
                        + Element.Margin.VerticalThickness;
                }
            }

            InternalLogger.Info($"ComputeItemSize: w{Element.ItemWidth}, h{Element.ItemHeight}, heightRequest: {Element.HeightRequest}");
            return widthChanged || heightChanged;
        }

        private void CreateView()
        {
            InternalLogger.Info("CreateView");

            if (_isFirstInitialization)
            {
                Element.CheckConsistency();
                _isFirstInitialization = false;
            }

            if (Element.IsInPullToRefresh() && Element.Parent is ContentView refreshView)
            {
                _isRefreshViewUserEnabled = refreshView.IsEnabled;
                refreshView.IsEnabled = false;
            }

            Control?.DataSource?.Dispose();
            Control?.Delegate?.Dispose();
            Control?.CollectionViewLayout?.Dispose();
            Control?.Dispose();

            var layout = BuildListLayout();

            // Otherwise the UICollectionView doesn't seem to take enough space
            //if ((Element.CollectionLayout != CollectionViewLayout.Grid || Element.CollectionLayout == CollectionViewLayout.Vertical) && Element.ItemHeight > 0)
            //{
            //    Element.HeightRequest = Element.ItemHeight
            //        + Element.CollectionPadding.VerticalThickness
            //        + Element.Margin.VerticalThickness;
            //}

            var rect = new CGRect(0, 0, 100, 100);
            _collectionView = new UICollectionView(rect, layout)
            {
                Delegate = new SizedFlowLayout(Element),
                DecelerationRate =
                    Element.ScrollSpeed == ScrollSpeed.Normal
                        ? UIScrollView.DecelerationRateNormal
                        : UIScrollView.DecelerationRateFast,
                BackgroundColor = Element?.BackgroundColor?.ToPlatform(),
                ShowsHorizontalScrollIndicator = false,
                ContentInset = new UIEdgeInsets(0, 0, 0, 0),
            };

            SetCollectionView(_collectionView);

            Element.UpdateLayout = new Command(UpdateListLayout);
        }

        private void SetCollectionView(UICollectionView collectionView)
        {
            SetNativeControl(collectionView);
            UpdateItemsSource();

            if (Element.IsInPullToRefresh() && Element.Parent is ContentView refreshView && _isRefreshViewUserEnabled)
            {
                refreshView.IsEnabled = true;
            }

            EnableDragAndDrop(Element.EnableDragAndDrop, Element.DragAndDropTrigger);

            ScrollToCurrentItem();
            ProcessDisableScroll();
        }

        private UICollectionViewFlowLayout BuildListLayout()
        {
            var sectionInset = new UIEdgeInsets(
                (NFloat)Element.CollectionPadding.Top,
                (NFloat)Element.CollectionPadding.Left,
                (NFloat)Element.CollectionPadding.Bottom,
                (NFloat)Element.CollectionPadding.Right);

            return Element.CollectionLayout == CollectionViewLayout.Grid || Element.CollectionLayout == CollectionViewLayout.Vertical
                       ? new UICollectionViewFlowLayout
                       {
                           ScrollDirection = UICollectionViewScrollDirection.Vertical,
                           ItemSize = new CGSize(Element.ItemWidth, Element.ItemHeight),
                           MinimumInteritemSpacing = Element.ItemSpacing,
                           MinimumLineSpacing = Element.ItemSpacing,
                           SectionInset = sectionInset,
                       }
                       : new SnappingCollectionViewLayout(Element.SnapStyle)
                       {
                           ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                           ItemSize = new CGSize(Element.ItemWidth, Element.ItemHeight),
                           MinimumInteritemSpacing = 500,
                           MinimumLineSpacing = Element.ItemSpacing,
                           SectionInset = sectionInset,
                       };
        }

        private void ScrollToCurrentItem()
        {
            if (Control == null
                || Element.CurrentIndex == -1
                || Element.CurrentIndex >= Control.NumberOfItemsInSection(0)
                || Control.NumberOfItemsInSection(0) == 0)
            {
                return;
            }

            InternalLogger.Info($"ScrollToCurrentItem( Element.CurrentIndex = {Element.CurrentIndex} )");
            if (Control.Delegate is SizedFlowLayout sizedFlowLayout)
            {
                sizedFlowLayout.IsInternalScroll = true;
            }

            Control.LayoutIfNeeded();

            UICollectionViewScrollPosition position = UICollectionViewScrollPosition.Top;
            if (Element.IsLayoutHorizontal)
            {
                switch (Element.SnapStyle)
                {
                    case SnapStyle.Center:
                        position = UICollectionViewScrollPosition.CenteredHorizontally;
                        break;
                    case SnapStyle.Start:
                        position = UICollectionViewScrollPosition.Left;
                        break;
                }
            }

            Control.ScrollToItem(
                NSIndexPath.FromRowSection(Element.CurrentIndex, 0),
                position,
                Element.AnimateScroll);
        }

        private void ProcessDisableScroll()
        {
            if (Control == null)
            {
                return;
            }

            Control.ScrollEnabled = !Element.DisableScroll;
        }

        private void UpdateEnableDragAndDrop()
        {
            if (Control == null)
            {
                return;
            }

            EnableDragAndDrop(Element.EnableDragAndDrop, Element.DragAndDropTrigger);

            ((iOSViewSource)Control.DataSource).OnEnableDragAndDropUpdated(Element.EnableDragAndDrop);
        }

        private void UpdateItemsSource()
        {
            if (Control == null)
            {
                return;
            }

            InternalLogger.Info("UpdateItemsSource");

            var oldDataSource = Control.DataSource;

            if (_itemsSource is INotifyCollectionChanged oldNotifyCollection)
            {
                oldNotifyCollection.CollectionChanged -= OnCollectionChanged;
            }

            _itemsSource = Element.ItemsSource;

            List<DataTemplate> dataTemplates = null;
            if (Element.ItemTemplate is DataTemplateSelector dataTemplateSelector)
            {
                dataTemplates = RegisterCellDataTemplates(dataTemplateSelector);
            }
            else
            {
                Control.RegisterClassForCell(typeof(iOSViewCell), nameof(iOSViewCell));
            }

            Control.DataSource = new iOSViewSource(Element, dataTemplates);

            /* Prevents 'Invalid batch updates detected' error. */
            Control.NumberOfItemsInSection(0);

            oldDataSource?.Dispose();

            if (_itemsSource is INotifyCollectionChanged newNotifyCollection)
            {
                newNotifyCollection.CollectionChanged += OnCollectionChanged;
            }
        }

        private CancellationTokenSource _changeLayoutCts;

        private void UpdateListLayout()
        {
            if (_collectionView == null || _isDisposed)
            {
                return;
            }

            _changeLayoutCts?.Cancel();
            _changeLayoutCts = new CancellationTokenSource();

            TaskMonitor.Create(() => ChangeLayoutThrottleAsync(_changeLayoutCts.Token));
        }

        private async Task ChangeLayoutThrottleAsync(CancellationToken token)
        {
            await Task.Delay(50, token);
            InternalLogger.Info("UpdateListLayout");

            var oldCollectionViewLayout = _collectionView.CollectionViewLayout;

            ComputeItemSize(Bounds.Width, Bounds.Height);

            var newLayout = BuildListLayout();
            _collectionView.CollectionViewLayout = newLayout;
            oldCollectionViewLayout?.Dispose();

            UpdateItemsSource();
            ProcessDisableScroll();
            ScrollToCurrentItem();
        }


        private List<DataTemplate> RegisterCellDataTemplates(DataTemplateSelector dataTemplateSelector)
        {
            var selectorTypeInfo = dataTemplateSelector.GetType().GetTypeInfo();
            var selectorDataTemplatesProperties =
                selectorTypeInfo
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                    .Where(p => typeof(DataTemplate).IsAssignableFrom(p.PropertyType));

            foreach (var selectorDataTemplateProperty in selectorDataTemplatesProperties)
            {
                var dataTemplate = (DataTemplate)selectorDataTemplateProperty.GetValue(dataTemplateSelector);

                if (!_registeredDataTemplates.Contains(dataTemplate))
                {
                    _registeredDataTemplates.Add(dataTemplate);
                    Control.RegisterClassForCell(
                        typeof(iOSViewCell),
                        IdentifierFormatter.FormatDataTemplateCellIdentifier(_registeredDataTemplates.Count - 1));
                }
            }

            return _registeredDataTemplates;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isMovedBackfire)
            {
                return;
            }

            if (Control == null || _itemsSource == null)
            {
                return;
            }

            ((iOSViewSource)Control.DataSource).HandleNotifyCollectionChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var addedIndexPaths = new NSIndexPath[e.NewItems.Count];
                    for (int addedIndex = e.NewStartingIndex, index = 0;
                        index < addedIndexPaths.Length;
                        addedIndex++, index++)
                    {
                        addedIndexPaths[index] = NSIndexPath.FromRowSection(addedIndex, 0);
                    }

                    Control.InsertItems(addedIndexPaths);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    var removedIndexPaths = new NSIndexPath[e.OldItems.Count];
                    for (int removedIndex = e.OldStartingIndex, index = 0;
                        index < removedIndexPaths.Length;
                        removedIndex++, index++)
                    {
                        removedIndexPaths[index] = NSIndexPath.FromRowSection(removedIndex, 0);
                    }

                    Control.DeleteItems(removedIndexPaths);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    UpdateItemsSource();
                    break;

                case NotifyCollectionChangedAction.Move:
                    Control.MoveItem(
                        NSIndexPath.FromRowSection(e.OldStartingIndex, 0),
                        NSIndexPath.FromRowSection(e.NewStartingIndex, 0));
                    break;

                case NotifyCollectionChangedAction.Replace:
                    Control.ReloadItems(new[] { NSIndexPath.FromRowSection(e.NewStartingIndex, 0) });
                    break;
            }
        }
    }
}
