using System.Collections;

using Foundation;

using Sharpnado.Presentation.Forms.RenderedViews;
using UIKit;

namespace Sharpnado.Presentation.Forms.iOS.Renderers.HorizontalList
{
    public partial class iOSHorizontalListViewRenderer
    {
        private void EnableDragAndDrop()
        {
            int from = -1;
            NSIndexPath pathTo = null;
            iOSViewCell draggedViewCell = null;

            // Create a custom gesture recognizer
            var longPressGesture = new UILongPressGestureRecognizer(
                gesture =>
                {
                    // Take action based on state
                    switch (gesture.State)
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
                                if (draggedViewCell == null || (draggedViewCell.FormsCell is DraggableViewCell draggableViewCell && !draggableViewCell.IsDraggable))
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
                            if (targetViewCell?.FormsCell is DraggableViewCell targetDraggableViewCell && !targetDraggableViewCell.IsDraggable)
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
                                    Element.IsDragAndDropping = false;

                                    if (draggedViewCell?.FormsCell is DraggableViewCell draggableViewCell)
                                    {
                                        draggableViewCell.IsDragAndDropping = false;
                                        Element.DragAndDropEndedCommand?.Execute(null);
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
                });

            // Add the custom recognizer to the collection view
            Control.AddGestureRecognizer(longPressGesture);
        }
    }
}