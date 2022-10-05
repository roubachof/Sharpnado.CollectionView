using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Sharpnado.CollectionView.Paging;

using Xamarin.Forms;

namespace Sharpnado.CollectionView.RenderedViews
{
    public enum DragAndDropTrigger
    {
        LongTap = 0,
        Pan,
    }

    public enum DragAndDropDirection
    {
        Free = 0,
        VerticalOnly,
        HorizontalOnly,
    }

    public enum CollectionViewLayout
    {
        Horizontal = 0,
        Grid,
        Carousel,
        Vertical,
    }

    public enum SnapStyle
    {
        None = 0,
        Start,
        Center,
    }

    /// <summary>
    /// Slower and slowest have the same result on iOS.
    /// </summary>
    public enum ScrollSpeed
    {
        Normal = 0,
        Slower,
        Slowest,
    }

    public class CollectionLayoutChangedEventArgs : EventArgs
    {
        public CollectionLayoutChangedEventArgs(CollectionViewLayout listLayout)
        {
            ListLayout = listLayout;
        }

        public CollectionViewLayout ListLayout { get; }
    }

    public class DraggableViewCell : ViewCell
    {
        public static readonly BindableProperty IsDraggableProperty = BindableProperty.Create(
            nameof(IsDraggable),
            typeof(bool),
            typeof(DraggableViewCell),
            defaultValue: true);

        public static readonly BindableProperty IsDragAndDroppingProperty = BindableProperty.Create(
            nameof(IsDragAndDropping),
            typeof(bool),
            typeof(DraggableViewCell),
            defaultValue: false);

        public bool IsDraggable
        {
            get => (bool)GetValue(IsDraggableProperty);
            set => SetValue(IsDraggableProperty, value);
        }

        public bool IsDragAndDropping
        {
            get => (bool)GetValue(IsDragAndDroppingProperty);
            set => SetValue(IsDragAndDroppingProperty, value);
        }
    }

    public class CollectionView : View
    {
        public static readonly BindableProperty CollectionLayoutProperty = BindableProperty.Create(
            nameof(CollectionLayout),
            typeof(CollectionViewLayout),
            typeof(CollectionView),
            CollectionViewLayout.Vertical,
            propertyChanged: OnListLayoutChanged,
            propertyChanging: OnListLayoutChanging);

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(CollectionView),
            default(IEnumerable<object>),
            BindingMode.TwoWay,
            propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty InfiniteListLoaderProperty = BindableProperty.Create(
            nameof(InfiniteListLoader),
            typeof(IInfiniteListLoader),
            typeof(CollectionView));

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(CollectionView),
            default(DataTemplate));

        public static readonly BindableProperty ItemHeightProperty = BindableProperty.Create(
            nameof(ItemHeight),
            typeof(double),
            typeof(CollectionView),
            defaultValue: 0D,
            defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty ItemWidthProperty = BindableProperty.Create(
            nameof(ItemWidth),
            typeof(double),
            typeof(CollectionView),
            defaultValue: 0D,
            defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty CollectionPaddingProperty = BindableProperty.Create(
            nameof(CollectionPadding),
            typeof(Thickness),
            typeof(CollectionView),
            defaultValue: new Thickness(0, 0),
            defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty ItemSpacingProperty = BindableProperty.Create(
            nameof(ItemSpacing),
            typeof(int),
            typeof(CollectionView),
            defaultValue: 0,
            defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty TapCommandProperty = BindableProperty.Create(
            nameof(TapCommand),
            typeof(ICommand),
            typeof(CollectionView));

        public static readonly BindableProperty ScrollBeganCommandProperty = BindableProperty.Create(
            nameof(ScrollBeganCommand),
            typeof(ICommand),
            typeof(CollectionView));

        public static readonly BindableProperty ScrollEndedCommandProperty = BindableProperty.Create(
            nameof(ScrollEndedCommand),
            typeof(ICommand),
            typeof(CollectionView));

        public static readonly BindableProperty ScrollingLeftCommandProperty = BindableProperty.Create(
            nameof(ScrollingLeftCommand),
            typeof(ICommand),
            typeof(CollectionView));

        public static readonly BindableProperty ScrollingUpCommandProperty = BindableProperty.Create(
            nameof(ScrollingUpCommand),
            typeof(ICommand),
            typeof(CollectionView));

        public static readonly BindableProperty ScrollingRightCommandProperty = BindableProperty.Create(
            nameof(ScrollingRightCommand),
            typeof(ICommand),
            typeof(CollectionView));

        public static readonly BindableProperty ScrollingDownCommandProperty = BindableProperty.Create(
            nameof(ScrollingDownCommand),
            typeof(ICommand),
            typeof(CollectionView));

        public static readonly BindableProperty EnableDragAndDropProperty = BindableProperty.Create(
            nameof(EnableDragAndDrop),
            typeof(bool),
            typeof(CollectionView),
            default(bool));

        public static readonly BindableProperty DragAndDropStartedCommandProperty = BindableProperty.Create(
            nameof(DragAndDropStartedCommand),
            typeof(ICommand),
            typeof(CollectionView));

        public static readonly BindableProperty DragAndDropEndedCommandProperty = BindableProperty.Create(
            nameof(DragAndDropEndedCommand),
            typeof(ICommand),
            typeof(CollectionView));

        public static readonly BindableProperty CurrentIndexProperty = BindableProperty.Create(
            nameof(CurrentIndex),
            typeof(int),
            typeof(CollectionView),
            defaultValue: -1,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnCurrentIndexChanged);

        public static readonly BindableProperty VisibleCellCountProperty = BindableProperty.Create(
            nameof(VisibleCellCount),
            typeof(int),
            typeof(CollectionView),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnVisibleCellCountChanged);

        public static readonly BindableProperty DisableScrollProperty = BindableProperty.Create(
            nameof(DisableScroll),
            typeof(bool),
            typeof(CollectionView),
            defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty IsDragAndDroppingProperty = BindableProperty.Create(
            nameof(IsDragAndDropping),
            typeof(bool),
            typeof(CollectionView),
            defaultValue: false);

        public static readonly BindableProperty DragAndDropDirectionProperty = BindableProperty.Create(
          nameof(DragAndDropDirection),
          typeof(DragAndDropDirection),
          typeof(CollectionView),
          DragAndDropDirection.Free,
          propertyChanged: OnMovementDirectionChanged);

        public static readonly BindableProperty ColumnCountProperty = BindableProperty.Create(
            nameof(ColumnCount),
            typeof(int),
            typeof(CollectionView),
            1);

        public CollectionView()
        {
            // default layout is VerticalList
            SnapStyle = SnapStyle.None;
            ColumnCount = 1;
            ScrollSpeed = ScrollSpeed.Normal;
        }

        public event EventHandler<CollectionLayoutChangedEventArgs> CollectionLayoutChanging;

        public DragAndDropTrigger DragAndDropTrigger { get; set; } = DragAndDropTrigger.LongTap;

        public int CurrentIndex
        {
            get => (int)GetValue(CurrentIndexProperty);
            set => SetValue(CurrentIndexProperty, value);
        }

        /// <summary>
        /// The platform renderers doesn't handle changes on this property: this is OneWayToSource binding.
        /// This property is only bindable to allow styling.
        /// </summary>
        public double ItemHeight
        {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        /// <summary>
        /// The platform renderers doesn't handle changes on this property: this is OneWayToSource binding.
        /// This property is only bindable to allow styling.
        /// </summary>
        public double ItemWidth
        {
            get => (double)GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }

        /// <summary>
        /// The platform renderers doesn't handle changes on this property: this is OneWayToSource binding.
        /// This property is only bindable to allow styling.
        /// </summary>
        public int ItemSpacing
        {
            get => (int)GetValue(ItemSpacingProperty);
            set => SetValue(ItemSpacingProperty, value);
        }

        /// <summary>
        /// The platform renderers doesn't handle changes on this property: this is OneWayToSource binding.
        /// This property is only bindable to allow styling.
        /// </summary>
        public Thickness CollectionPadding
        {
            get => (Thickness)GetValue(CollectionPaddingProperty);
            set => SetValue(CollectionPaddingProperty, value);
        }

        public int VisibleCellCount
        {
            get => (int)GetValue(VisibleCellCountProperty);
            set => SetValue(VisibleCellCountProperty, value);
        }

        public bool DisableScroll
        {
            get => (bool)GetValue(DisableScrollProperty);
            set => SetValue(DisableScrollProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public IInfiniteListLoader InfiniteListLoader
        {
            get => (IInfiniteListLoader)GetValue(InfiniteListLoaderProperty);
            set => SetValue(InfiniteListLoaderProperty, value);
        }

        public ICommand TapCommand
        {
            get => (ICommand)GetValue(TapCommandProperty);
            set => SetValue(TapCommandProperty, value);
        }

        public ICommand ScrollBeganCommand
        {
            get => (ICommand)GetValue(ScrollBeganCommandProperty);
            set => SetValue(ScrollBeganCommandProperty, value);
        }

        public ICommand ScrollEndedCommand
        {
            get => (ICommand)GetValue(ScrollEndedCommandProperty);
            set => SetValue(ScrollEndedCommandProperty, value);
        }

        public ICommand ScrollingLeftCommand
        {
            get => (ICommand)GetValue(ScrollingLeftCommandProperty);
            set => SetValue(ScrollingLeftCommandProperty, value);
        }

        public ICommand ScrollingUpCommand
        {
            get => (ICommand)GetValue(ScrollingUpCommandProperty);
            set => SetValue(ScrollingUpCommandProperty, value);
        }

        public ICommand ScrollingDownCommand
        {
            get => (ICommand)GetValue(ScrollingDownCommandProperty);
            set => SetValue(ScrollingDownCommandProperty, value);
        }

        public ICommand ScrollingRightCommand
        {
            get => (ICommand)GetValue(ScrollingRightCommandProperty);
            set => SetValue(ScrollingRightCommandProperty, value);
        }

        public bool EnableDragAndDrop
        {
            get => (bool)GetValue(EnableDragAndDropProperty);
            set => SetValue(EnableDragAndDropProperty, value);
        }

        public ICommand DragAndDropStartedCommand
        {
            get => (ICommand)GetValue(DragAndDropStartedCommandProperty);
            set => SetValue(DragAndDropStartedCommandProperty, value);
        }

        public ICommand DragAndDropEndedCommand
        {
            get => (ICommand)GetValue(DragAndDropEndedCommandProperty);
            set => SetValue(DragAndDropEndedCommandProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public bool IsDragAndDropping
        {
            get => (bool)GetValue(IsDragAndDroppingProperty);
            set => SetValue(IsDragAndDroppingProperty, value);
        }

        public DragAndDropDirection DragAndDropDirection
        {
            get => (DragAndDropDirection)GetValue(DragAndDropDirectionProperty);
            set => SetValue(DragAndDropDirectionProperty, value);
        }

        public CollectionViewLayout CollectionLayout
        {
            get => (CollectionViewLayout)GetValue(CollectionLayoutProperty);
            set => SetValue(CollectionLayoutProperty, value);
        }

        public int ColumnCount
        {
            get => (int)GetValue(ColumnCountProperty);
            set => SetValue(ColumnCountProperty, value);
        }

        public Func<ViewCell, Task> PreRevealAnimationAsync { get; set; }

        public Func<ViewCell, Task> RevealAnimationAsync { get; set; }

        public Func<ViewCell, Task> PostRevealAnimationAsync { get; set; }

        public Func<ViewCell, CancellationToken, Task> DragAndDropEnabledAnimationAsync { get; set; }

        public int ViewCacheSize { get; set; } = 0;

        public SnapStyle SnapStyle { get; set; } = SnapStyle.None;

        public ScrollSpeed ScrollSpeed { get; set; } = ScrollSpeed.Normal;

        public bool IsLayoutHorizontal =>
            CollectionLayout == CollectionViewLayout.Horizontal || CollectionLayout == CollectionViewLayout.Carousel;

        public bool IsLayoutLinear =>
            CollectionLayout == CollectionViewLayout.Horizontal
            || CollectionLayout == CollectionViewLayout.Carousel
            || CollectionLayout == CollectionViewLayout.Vertical;

        public bool AnimateScroll { get; set; }

        public void ScrollTo(int index, bool animateScroll = true)
        {
            AnimateScroll = animateScroll;
            CurrentIndex = index;
        }

        public void CheckConsistency()
        {
            if (CollectionLayout == CollectionViewLayout.Carousel
                && (ColumnCount != 1 || SnapStyle != SnapStyle.Center))
            {
                throw new InvalidOperationException(
                    "When setting CollectionLayout to Carousel, you can only set ColumnCount to 1 and SnapStyle to Center");
            }
        }

        public bool IsInPullToRefresh()
        {
            if (Parent == null)
            {
                return false;
            }

            string parentTypeFullName = Parent.GetType().FullName;

            const string RefreshViewFullName = "Xamarin.Forms.RefreshView";
            const string PullToRefreshFullName = "Refractored.XamForms.PullToRefresh.PullToRefreshLayout ";

            return parentTypeFullName == RefreshViewFullName
                || parentTypeFullName == PullToRefreshFullName;
        }

        /// <summary>
        /// Automatically compute item width for a given parent width and a given column count.
        /// </summary>
        /// <remarks>This method is Pure.</remarks>
        [Pure]
        public double ComputeItemWidth(double availableWidth)
        {
            if (ColumnCount == 0)
            {
                throw new InvalidOperationException("ColumnCount should be greater than 0 in order to automatically compute item width");
            }

            int itemSpace = PlatformHelper.Instance.DpToPixels(ItemSpacing);
            int leftPadding = PlatformHelper.Instance.DpToPixels(CollectionPadding.Left);
            int rightPadding = PlatformHelper.Instance.DpToPixels(CollectionPadding.Right);

            int totalWidthSpacing = itemSpace * (ColumnCount - 1) + leftPadding + rightPadding;

            double spaceWidthLeft = availableWidth - totalWidthSpacing;

            return PlatformHelper.Instance.PixelsToDp(Math.Floor((spaceWidthLeft / ColumnCount) * 100) / 100);
        }

        /// <summary>
        /// Automatically compute item height for a given parent height.
        /// </summary>
        /// <remarks>This method is Pure.</remarks>
        [Pure]
        public double ComputeItemHeight(double availableHeight)
        {
            if (CollectionLayout == CollectionViewLayout.Grid || CollectionLayout == CollectionViewLayout.Vertical || ItemHeight > 0)
            {
                throw new InvalidOperationException(
                    "Can compute item height only if a height has not been specified and layout is horizontal linear");
            }

            int topPadding = PlatformHelper.Instance.DpToPixels(CollectionPadding.Top);
            int bottomPadding = PlatformHelper.Instance.DpToPixels(CollectionPadding.Bottom);

            int totalHeightSpacing = topPadding + bottomPadding;

            double spaceHeightLeft = availableHeight - totalHeightSpacing;

            return PlatformHelper.Instance.PixelsToDp(spaceHeightLeft);
        }

        private static void OnListLayoutChanging(BindableObject bindable, object oldvalue, object newvalue)
        {
            var horizontalListView = (CollectionView)bindable;

            var newLayout = (CollectionViewLayout)newvalue;
            switch (newLayout)
            {
                case CollectionViewLayout.Carousel:
                    horizontalListView.SnapStyle = SnapStyle.Center;
                    horizontalListView.ColumnCount = 1;
                    horizontalListView.ScrollSpeed = ScrollSpeed.Slowest;
                    break;

                case CollectionViewLayout.Vertical:
                    horizontalListView.SnapStyle = SnapStyle.None;
                    horizontalListView.ColumnCount = 1;
                    horizontalListView.ScrollSpeed = ScrollSpeed.Normal;
                    break;

                case CollectionViewLayout.Grid:
                    horizontalListView.SnapStyle = SnapStyle.None;
                    horizontalListView.ColumnCount = 0;
                    horizontalListView.ScrollSpeed = ScrollSpeed.Normal;
                    break;

                case CollectionViewLayout.Horizontal:
                    horizontalListView.SnapStyle = SnapStyle.None;
                    horizontalListView.ColumnCount = 0;
                    horizontalListView.ScrollSpeed = ScrollSpeed.Normal;
                    break;
            }

            horizontalListView.CollectionLayoutChanging?.Invoke(
                horizontalListView,
                new CollectionLayoutChangedEventArgs(newLayout));
        }

        private static void OnListLayoutChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
        }

        private static void OnCurrentIndexChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
        }

        private static void OnVisibleCellCountChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
        }

        private static void OnMovementDirectionChanged(BindableObject bindable, object oldValue, object newValue)
        {
        }
    }

    public class HorizontalListView : CollectionView
    {
        public HorizontalListView()
        {
            CollectionLayout = CollectionViewLayout.Horizontal;
        }
    }

    public class GridView : CollectionView
    {
        public GridView()
        {
            CollectionLayout = CollectionViewLayout.Grid;
        }
    }

    public class CarouselView : CollectionView
    {
        public CarouselView()
        {
            CollectionLayout = CollectionViewLayout.Carousel;
        }
    }
}