using System.Collections;

using Foundation;

using Sharpnado.HorizontalListView.RenderedViews;
using Sharpnado.HorizontalListView.ViewModels;

using UIKit;

namespace Sharpnado.HorizontalListView.iOS.Renderers.HorizontalList
{
    public partial class iOSHorizontalListViewRenderer
    {
        private UIGestureRecognizer _dragAndDropGesture;

        private void EnableDragAndDrop(bool isEnabled, bool panGesture)
        {
            if (_dragAndDropGesture != null)
            {
                Control.RemoveGestureRecognizer(_dragAndDropGesture);
                _dragAndDropGesture = null;
                return;
            }

            if (!isEnabled)
            {
                return;
            }

            int from = -1;
            NSIndexPath pathTo = null;
            iOSViewCell draggedViewCell = null;

            // Create a custom gesture recognizer
            _dragAndDropGesture = panGesture
                ? new UIPanGestureRecognizer(
                    gesture =>
                    {
                        // Take action based on state
                        HandleDragByGestureState(gesture.State, gesture, ref from, ref pathTo, ref draggedViewCell);

                    })
                : new UILongPressGestureRecognizer(
                    gesture =>
                    {
                        // Take action based on state
                        HandleDragByGestureState(gesture.State, gesture, ref from, ref pathTo, ref draggedViewCell);

                    });

            // Add the custom recognizer to the collection view
            Control.AddGestureRecognizer(_dragAndDropGesture);
        }

        private void HandleDragByGestureState(
            UIGestureRecognizerState state,
            UIGestureRecognizer gesture,
            ref int from,
            ref NSIndexPath pathTo,
            ref iOSViewCell draggedViewCell)
        {
            switch (state)
            {
                case UIGestureRecognizerState.Began:
                    var selectedIndexPath = Control.IndexPathForItemAtPoint(gesture.LocationInView(Control));
                    if (selectedIndexPath != null)
                    {
                        draggedViewCell = (iOSViewCell)Control.CellForItem(selectedIndexPath);
                        if (draggedViewCell.FormsCell is DraggableViewCell draggableViewCell)
                        {
                            if (!draggableViewCell.IsDraggable)
                            {
                                Control.CancelInteractiveMovement();
                                return;
                            }

                            draggableViewCell.IsDragAndDropping = true;
                        }

                        from = (int)selectedIndexPath.Item;
                        Control.BeginInteractiveMovementForItem(selectedIndexPath);
                        Element.IsDragAndDropping = true;

                        Element.DragAndDropStartedCommand?.Execute(
                            new DragAndDropInfo(from, -1, Element.BindingContext));
                    }

                    break;

                case UIGestureRecognizerState.Changed:
                    if (draggedViewCell == null)
                    {
                        return;
                    }

                    var changedPath = Control.IndexPathForItemAtPoint(gesture.LocationInView(gesture.View));
                    if (changedPath != null)
                    {
                        draggedViewCell = (iOSViewCell)Control.CellForItem(changedPath);
                        if (draggedViewCell == null
                            || (draggedViewCell.FormsCell is DraggableViewCell draggableViewCell
                                && !draggableViewCell.IsDraggable))
                        {
                            pathTo = null;

                            // System.Diagnostics.Debug.WriteLine("Cancel change state");
                            return;
                        }

                        pathTo = changedPath;

                        // System.Diagnostics.Debug.WriteLine($"State changed to {pathTo.Item}");
                    }

                    Control.UpdateInteractiveMovement(gesture.LocationInView(gesture.View));
                    break;

                case UIGestureRecognizerState.Ended:
                    if (from < 0 || pathTo == null)
                    {
                        // System.Diagnostics.Debug.WriteLine($"Ended but cancelled cause incorrect parameters");
                        Control.CancelInteractiveMovement();
                        return;
                    }

                    var targetViewCell = (iOSViewCell)Control.CellForItem(pathTo);
                    if (targetViewCell?.FormsCell is DraggableViewCell targetDraggableViewCell
                        && !targetDraggableViewCell.IsDraggable)
                    {
                        // System.Diagnostics.Debug.WriteLine($"Ended but cancelled cause target is not draggable");
                        Control.CancelInteractiveMovement();
                        return;
                    }

                    // System.Diagnostics.Debug.WriteLine($"Ended from: {from} to: {pathTo.Item}");
                    Control.EndInteractiveMovement();
                    if (_itemsSource is IList itemsSourceList)
                    {
                        try
                        {
                            _isMovedBackfire = true;
                            var item = itemsSourceList[from];
                            itemsSourceList.RemoveAt(from);
                            itemsSourceList.Insert((int)pathTo.Item, item);
                            var to = itemsSourceList.IndexOf(item);
                            Element.IsDragAndDropping = false;

                            if (draggedViewCell?.FormsCell is DraggableViewCell draggableViewCell)
                            {
                                draggableViewCell.IsDragAndDropping = false;
                                Element.DragAndDropEndedCommand?.Execute(
                                    new DragAndDropInfo(from, to, draggableViewCell.BindingContext));
                            }

                            draggedViewCell = null;
                        }
                        finally
                        {
                            _isMovedBackfire = false;
                        }
                    }

                    break;

                default:
                    Control.CancelInteractiveMovement();
                    break;
            }
        }
    }
}