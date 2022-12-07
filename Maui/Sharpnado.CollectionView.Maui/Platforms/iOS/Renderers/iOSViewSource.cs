using System.Collections.Specialized;
using System.Windows.Input;
using Foundation;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using ObjCRuntime;
using Sharpnado.CollectionView.iOS.Helpers;
using Sharpnado.Tasks;
using UIKit;
using Platform = Microsoft.Maui.Controls.Compatibility.Platform.iOS.Platform;

namespace Sharpnado.CollectionView.iOS.Renderers
{
    public struct UIViewCellHolder
    {
        public static UIViewCellHolder Empty = new(null, null, null);

        private readonly ICommand _tapCommand;

        private readonly WeakReference<UIView> _weakCellContent;

        public UIViewCellHolder(ViewCell formsCell, UIView view, ICommand tapCommand)
        {
            FormsCell = formsCell;
            _tapCommand = tapCommand;

            _weakCellContent = new WeakReference<UIView>(view);

            if (tapCommand != null)
            {
                view.AddGestureRecognizer(new UITapGestureRecognizer(OnTap));
            }
        }

        public ViewCell FormsCell { get; }

        public bool TryGetCellContent(out UIView cellContent)
        {
            return _weakCellContent.TryGetTarget(out cellContent);
        }

        public static bool operator ==(UIViewCellHolder c1, UIViewCellHolder c2)
        {
            if (!c1.TryGetCellContent(out var cellContent1) || !c2.TryGetCellContent(out var cellContent2))
            {
                return false;
            }

            return ReferenceEquals(c1.FormsCell, c2.FormsCell) && ReferenceEquals(cellContent1, cellContent2);
        }

        public static bool operator !=(UIViewCellHolder c1, UIViewCellHolder c2)
        {
            if (!c1.TryGetCellContent(out var cellContent1) || !c2.TryGetCellContent(out var cellContent2))
            {
                return true;
            }

            return !ReferenceEquals(c1.FormsCell, c2.FormsCell) || !ReferenceEquals(cellContent1, cellContent2);
        }

        public bool Equals(UIViewCellHolder other)
        {
            if (!TryGetCellContent(out var cellContent1) || !other.TryGetCellContent(out var cellContent2))
            {
                return false;
            }

            return Equals(FormsCell, other.FormsCell) && Equals(cellContent1, cellContent2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is UIViewCellHolder holder && Equals(holder);
        }

        public override int GetHashCode()
        {
            TryGetCellContent(out var cellContent);

            unchecked
            {
                return ((FormsCell != null ? FormsCell.GetHashCode() : 0) * 397) ^ (cellContent != null ? cellContent.GetHashCode() : 0);
            }
        }

        private void OnTap()
        {
            if (_tapCommand.CanExecute(null))
            {
                _tapCommand.Execute(FormsCell.BindingContext);
            }
        }
    }

    public class iOSViewCell : UICollectionViewCell
    {
        private const int InnerViewTag = 99;

        public iOSViewCell(NativeHandle p)
            : base(p)
        {
        }

        public bool IsInitialized => FormsCell != null;

        public ViewCell FormsCell { get; private set; }

        public void Reset()
        {
            if (!IsInitialized)
            {
                return;
            }

            FormsCell = null;
        }

        public void Initialize(ViewCell formsCell, UIView view, CollectionView parent)
        {
            FormsCell = formsCell;

            // A previous view may have been added in a different data source
            ContentView.ViewWithTag(InnerViewTag)?.RemoveFromSuperview();

            view.Tag = InnerViewTag;
            ContentView.AddSubview(view);
        }

        public void Bind(object dataContext, CollectionView parent)
        {
            InternalLogger.Debug(() => $"Bind with {dataContext}");
            FormsCell.BindingContext = dataContext;
            FormsCell.Parent = parent;
        }
    }

    public class iOSViewSource : UICollectionViewDataSource
    {
        private readonly WeakReference<CollectionView> _weakElement;

        private readonly List<object> _dataSource;
        private readonly UIViewCellHolderQueue _viewCellHolderCellHolderQueue;
        private readonly Dictionary<long, WeakReference<iOSViewCell>> _createdCells;
        private readonly List<DataTemplate> _dataTemplates;
        private readonly bool _multipleCellTemplates;

        private CancellationTokenSource _enableDragAndDropCts;

        private int _currentMaxPosition = -1;

        public iOSViewSource(CollectionView element, List<DataTemplate> dataTemplates)
        {
            _weakElement = new WeakReference<CollectionView>(element);
            _createdCells = new Dictionary<long, WeakReference<iOSViewCell>>();
            _dataTemplates = dataTemplates;

            var elementItemsSource = element.ItemsSource;
            _dataSource = elementItemsSource?.Cast<object>().ToList();

            if (_dataSource == null)
            {
                return;
            }

            _multipleCellTemplates = element.ItemTemplate is DataTemplateSelector;

            if (!_multipleCellTemplates)
            {
                // Cache only support single DataTemplate
                _viewCellHolderCellHolderQueue = new UIViewCellHolderQueue(
                    element.ViewCacheSize,
                    () => CreateViewCellHolder());
                _viewCellHolderCellHolderQueue.Build();
            }
        }

        public void HandleNotifyCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
        {
            switch (eventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _dataSource.InsertRange(eventArgs.NewStartingIndex, eventArgs.NewItems.Cast<object>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _dataSource.RemoveRange(eventArgs.OldStartingIndex, eventArgs.OldItems.Count);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    _dataSource[eventArgs.NewStartingIndex] = eventArgs.NewItems[0];
                    break;
                case NotifyCollectionChangedAction.Reset:
                    // Handled in the CollectionViewRenderer class, will just create a new adapter
                    break;
                case NotifyCollectionChangedAction.Move:
                    var item = _dataSource[eventArgs.OldStartingIndex];
                    _dataSource.RemoveAt(eventArgs.OldStartingIndex);
                    _dataSource.Insert(eventArgs.NewStartingIndex, item);
                    break;
            }
        }

        public override bool CanMoveItem(UICollectionView collectionView, NSIndexPath indexPath) =>
            _weakElement.TryGetTarget(out var element) && element.EnableDragAndDrop;

        public override void MoveItem(UICollectionView collectionView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            var item = _dataSource[(int)sourceIndexPath.Item];

            _dataSource.RemoveAt((int)sourceIndexPath.Item);
            _dataSource.Insert((int)destinationIndexPath.Item, item);
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return _dataSource?.Count ?? 0;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            InternalLogger.Debug(() => $"GetCell( index: {indexPath.Row} )");

            iOSViewCell nativeCell;
            if (!_multipleCellTemplates)
            {
                nativeCell = (iOSViewCell)collectionView.DequeueReusableCell(nameof(iOSViewCell), indexPath);
            }
            else
            {
                var cellType = GetOrCreateItemTemplateIdentifier(collectionView, indexPath);
                nativeCell = (iOSViewCell)collectionView.DequeueReusableCell(cellType, indexPath);
            }

            if (!_createdCells.ContainsKey(nativeCell.GetHashCode()))
            {
                _createdCells.Add(nativeCell.GetHashCode(), new WeakReference<iOSViewCell>(nativeCell));
            }

            if (!nativeCell.IsInitialized)
            {
                InternalLogger.Debug(() => "Initializing cell");
                UIViewCellHolder holder;
                if (_weakElement.TryGetTarget(out var element) && _multipleCellTemplates)
                {
                    holder = CreateViewCellHolder(indexPath.Row);
                }
                else
                {
                    holder = _viewCellHolderCellHolderQueue.Dequeue();
                }

                if (holder == UIViewCellHolder.Empty)
                {
                    return nativeCell;
                }

                if (_weakElement.TryGetTarget(out var p) && holder.TryGetCellContent(out var cellContent))
                {
                    nativeCell.Initialize(holder.FormsCell, cellContent, p);
                }
            }

            if (_weakElement.TryGetTarget(out var parent))
            {
                nativeCell.Bind(_dataSource[indexPath.Row], parent);

                if (indexPath.Row > _currentMaxPosition)
                {
                    _currentMaxPosition = indexPath.Row;
                    AnimateCell(nativeCell.FormsCell, parent);
                }
            }

            return nativeCell;
        }

        public void OnEnableDragAndDropUpdated(bool isEnabled)
        {
            _enableDragAndDropCts?.Cancel();
            _enableDragAndDropCts = new CancellationTokenSource();

            if (!isEnabled)
            {
                return;
            }

            foreach (var weakViewCell in _createdCells.Values)
            {
                if (!weakViewCell.TryGetTarget(out var nativeViewCell))
                {
                    continue;
                }

                HandleDragAndDropEnabledAnimation(nativeViewCell.FormsCell);
            }
        }

        public (double ItemWidth, double ItemHeight) GetSize(int itemIndex)
        {
            if (!_weakElement.TryGetTarget(out var element))
            {
                return (0, 0);
            }

            if (!element.IsLayoutLinear || element.ItemTemplate is not DataTemplateSelector dataTemplateSelector)
            {
                return (element.ItemWidth, element.ItemHeight);
            }

            var data = _dataSource[itemIndex];
            var dataTemplate = dataTemplateSelector.SelectTemplate(data, element);

            if (dataTemplate is SizedDataTemplate sizedDataTemplate)
            {
                return element.IsLayoutHorizontal
                    ? (sizedDataTemplate.Size, element.ItemHeight)
                    : (element.ItemWidth, sizedDataTemplate.Size);
            }

            if (dataTemplateSelector is SizedDataTemplateSelector sizedDataTemplateSelector)
            {
                return element.IsLayoutHorizontal
                    ? (sizedDataTemplateSelector.GetItemSize(data, element.ItemWidth), element.ItemHeight)
                    : (element.ItemWidth, sizedDataTemplateSelector.GetItemSize(data, element.ItemHeight));
            }

            return (element.ItemWidth, element.ItemHeight);
        }

        protected override void Dispose(bool disposing)
        {
            _viewCellHolderCellHolderQueue?.Clear();

            foreach (var weakCreatedCell in _createdCells.Values)
            {
                if (weakCreatedCell.TryGetTarget(out var createdCell))
                {
                    createdCell.Reset();
                }
            }

            _dataTemplates?.Clear();
            _createdCells?.Clear();
            _dataSource?.Clear();

            base.Dispose(disposing);
        }

        private void HandleDragAndDropEnabledAnimation(ViewCell viewCell)
        {
            if (!_weakElement.TryGetTarget(out var element))
            {
                return;
            }

            if (_enableDragAndDropCts != null
                && !_enableDragAndDropCts.IsCancellationRequested
                && element.EnableDragAndDrop
                && element.DragAndDropEnabledAnimationAsync != null)
            {
                InternalLogger.Debug("HandleDragAndDropEnabledAnimation");

                TaskMonitor.Create(element.DragAndDropEnabledAnimationAsync(viewCell, _enableDragAndDropCts.Token));
            }
        }

        private void AnimateCell(ViewCell cell, CollectionView element)
        {
            TaskMonitor.Create(
                async () =>
                    {
                        if (element.PreRevealAnimationAsync != null)
                        {
                            await element.PreRevealAnimationAsync(cell);
                        }

                        if (element.RevealAnimationAsync != null)
                        {
                            await element.RevealAnimationAsync(cell);
                        }

                        if (element.PostRevealAnimationAsync != null)
                        {
                            await element.PostRevealAnimationAsync(cell);
                        }
                    });
        }

        private string GetOrCreateItemTemplateIdentifier(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (_weakElement.TryGetTarget(out var element) && element.ItemTemplate is DataTemplateSelector dataTemplateSelector)
            {
                var item = _dataSource[(int)indexPath.Item];
                var template = dataTemplateSelector.SelectTemplate(item, element);
                var templateIndex = _dataTemplates.IndexOf(template);
                if (templateIndex == -1)
                {
                    // Rare cases where DataTemplate of the DataTemplateSelectors are not public properties
                    _dataTemplates.Add(template);
                    templateIndex = _dataTemplates.Count - 1;
                    collectionView.RegisterClassForCell(
                        typeof(iOSViewCell),
                        IdentifierFormatter.FormatDataTemplateCellIdentifier(templateIndex));
                }

                return IdentifierFormatter.FormatDataTemplateCellIdentifier(templateIndex);
            }

            return nameof(iOSViewCell);
        }

        private UIViewCellHolder CreateViewCellHolder(int itemIndex = -1)
        {
            ViewCell formsCell = null;

            if (!_weakElement.TryGetTarget(out var element))
            {
                return UIViewCellHolder.Empty;
            }

            (double itemWidth, double itemHeight) = GetSize(itemIndex);

            if (element.ItemTemplate is DataTemplateSelector selector)
            {
                if (itemIndex == -1)
                {
                    throw new InvalidOperationException("Cannot select a DataTemplate without an itemIndex");
                }

                var template = selector.SelectTemplate(_dataSource[itemIndex], element);
                if (template is SizedDataTemplate sizedDataTemplate)
                {
                    template = sizedDataTemplate.DataTemplate;
                }

                formsCell = (ViewCell)template.CreateContent();
            }
            else
            {
                formsCell = (ViewCell)element.ItemTemplate.CreateContent();
            }

            formsCell.Parent = element;
            formsCell.View.Layout(new Rect(0, 0, itemWidth, itemHeight));

            IVisualElementRenderer visualElementRenderer = Platform.GetRenderer(formsCell.View);
            if (visualElementRenderer != null)
            {
                DisposeRendererAndChildren(visualElementRenderer);
            }

            if (visualElementRenderer == null)
            {
                visualElementRenderer = Platform.CreateRenderer(formsCell.View);
                Platform.SetRenderer(formsCell.View, visualElementRenderer);
            }

            var nativeView = visualElementRenderer.NativeView;
            nativeView.Frame = formsCell.View.Bounds.ToRectangleF();

            HandleDragAndDropEnabledAnimation(formsCell);

            return new UIViewCellHolder(formsCell, nativeView, element.TapCommand);
        }

        internal static void DisposeRendererAndChildren(IVisualElementRenderer rendererToRemove)
        {
            if (rendererToRemove == null)
            {
                return;
            }

            if (rendererToRemove.NativeView != null)
            {
                var subviews = rendererToRemove.NativeView.Subviews;
                for (var i = 0; i < subviews.Length; i++)
                {
                    if (subviews[i] is IVisualElementRenderer childRenderer)
                    {
                        DisposeRendererAndChildren(childRenderer);
                    }
                }

                rendererToRemove.NativeView.RemoveFromSuperview();
            }

            rendererToRemove.Dispose();
        }
    }
}