using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using CoreGraphics;

using Foundation;

using Sharpnado.CollectionView.iOS.Helpers;
using Sharpnado.CollectionView.RenderedViews;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using CollectionView = Sharpnado.CollectionView.RenderedViews.CollectionView;
using CollectionViewRenderer = Sharpnado.CollectionView.iOS.Renderers.CollectionViewRenderer;

[assembly: ExportRenderer(typeof(CollectionView), typeof(CollectionViewRenderer))]

namespace Sharpnado.CollectionView.iOS.Renderers
{
    [Foundation.Preserve]
    public partial class CollectionViewRenderer : ViewRenderer<CollectionView.RenderedViews.CollectionView, UICollectionView>
    {
        private readonly List<DataTemplate> _registeredDataTemplates = new ();

        private IEnumerable _itemsSource;
        private UICollectionView _collectionView;

        private bool _isMovedBackfire;
        private bool _isFirstInitialization = true;
        private bool _isRefreshViewUserEnabled = false;

        private int _lastVisibleItemIndex = -1;

        public static void Initialize()
        {
        }

        public override void LayoutSubviews()
        {
            double height = Bounds.Height;
            double width = Bounds.Width;

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
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(RenderedViews.CollectionView.ItemsSource):
                    UpdateItemsSource();
                    break;
                case nameof(RenderedViews.CollectionView.CurrentIndex) when Control?.Delegate is SizedFlowLayout
                {
                    IsCurrentIndexUpdateBackfire: false
                }:
                    ScrollToCurrentItem();
                    break;
                case nameof(RenderedViews.CollectionView.DisableScroll):
                    ProcessDisableScroll();
                    break;
                case nameof(RenderedViews.CollectionView.ColumnCount):
                case nameof(RenderedViews.CollectionView.CollectionLayout):
                    UpdateListLayout();
                    break;
                case nameof(RenderedViews.CollectionView.EnableDragAndDrop):
                    UpdateEnableDragAndDrop();
                    break;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<RenderedViews.CollectionView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
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

                _registeredDataTemplates.Clear();
            }

            if (e.NewElement != null)
            {
                CreateView();
            }
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

            return widthChanged || heightChanged;
        }

        private void CreateView()
        {
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
            if ((Element.CollectionLayout != CollectionViewLayout.Grid || Element.CollectionLayout == CollectionViewLayout.Vertical) && Element.ItemHeight > 0)
            {
                Element.HeightRequest = Element.ItemHeight
                    + Element.CollectionPadding.VerticalThickness
                    + Element.Margin.VerticalThickness;
            }

            var rect = new CGRect(0, 0, 100, Element.HeightRequest);
            _collectionView = new UICollectionView(rect, layout)
            {
                Delegate = new SizedFlowLayout(Element),
                DecelerationRate =
                    Element.ScrollSpeed == ScrollSpeed.Normal
                        ? UIScrollView.DecelerationRateNormal
                        : UIScrollView.DecelerationRateFast,
                BackgroundColor = Element?.BackgroundColor.ToUIColor(),
                ShowsHorizontalScrollIndicator = false,
                ContentInset = new UIEdgeInsets(0, 0, 0, 0),
            };
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
                (nfloat)Element.CollectionPadding.Top,
                (nfloat)Element.CollectionPadding.Left,
                (nfloat)Element.CollectionPadding.Bottom,
                (nfloat)Element.CollectionPadding.Right);

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

            oldDataSource?.Dispose();

            if (_itemsSource is INotifyCollectionChanged newNotifyCollection)
            {
                newNotifyCollection.CollectionChanged += OnCollectionChanged;
            }
        }

        private void UpdateListLayout()
        {
            if (_collectionView == null)
            {
                return;
            }

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

            if (Control == null)
            {
                return;
            }

            if (Control.NumberOfItemsInSection(0) == ((IList)_itemsSource).Count)
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
