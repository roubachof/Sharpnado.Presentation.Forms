using System.Collections;

using Foundation;

using Sharpnado.Presentation.Forms.RenderedViews;
using Sharpnado.Presentation.Forms.ViewModels;
using UIKit;

namespace Sharpnado.Presentation.Forms.iOS.Renderers.HorizontalList
{
    public partial class iOSHorizontalListViewRenderer
    {
        private UILongPressGestureRecognizer _dragAndDropGesture;
        
        private void DisableDragAndDrop()
        {
            if (_dragAndDropGesture != null)
            {
                Control.RemoveGestureRecognizer(_dragAndDropGesture);
                _dragAndDropGesture = null;
            }
        }
        
        private void EnableDragAndDrop()
        {
            int from = -1;
            NSIndexPath pathTo = null;
            iOSViewCell draggedViewCell = null;

            // Create a custom gesture recognizer
            _dragAndDropGesture = new UILongPressGestureRecognizer(
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
                                
                                Element.DragAndDropStartCommand?.Execute(new DragAndDropInfo()
                                {
                                    From = from,
                                    To = -1,
                                    Content = Element.BindingContext
                                });
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
                                    var to = itemsSourceList.IndexOf(item);
                                    Element.IsDragAndDropping = false;

                                    if (draggedViewCell?.FormsCell is DraggableViewCell draggableViewCell)
                                    {
                                        draggableViewCell.IsDragAndDropping = false;
                                        Element.DragAndDropEndedCommand?.Execute(new DragAndDropInfo()
                                        {
                                            From = from,
                                            To = to,
                                            Content = draggableViewCell.BindingContext 
                                        });
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
            Control.AddGestureRecognizer(_dragAndDropGesture);
        }
    }
}