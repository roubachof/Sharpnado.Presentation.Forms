using System.Collections;
using Sharpnado.Presentation.Forms.RenderedViews;
using UIKit;

namespace Sharpnado.Presentation.Forms.iOS.Renderers.HorizontalList
{
    public partial class iOSHorizontalListViewRenderer
    {
        private void EnableDragAndDrop()
        {
            int from = -1;
            int to = -1;
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
                                from = (int)selectedIndexPath.Item;
                                Control.BeginInteractiveMovementForItem(selectedIndexPath);
                                Element.IsDragAndDropping = true;

                                draggedViewCell = (iOSViewCell)Control.CellForItem(selectedIndexPath);
                                if (draggedViewCell.FormsCell is DraggableViewCell draggableViewCell)
                                {
                                    draggableViewCell.IsDragAndDropping = true;
                                }
                            }

                            break;

                        case UIGestureRecognizerState.Changed:
                            var changedPath = Control.IndexPathForItemAtPoint(gesture.LocationInView(gesture.View));
                            if (changedPath != null)
                            {
                                to = (int)changedPath.Item;
                            }

                            Control.UpdateInteractiveMovement(gesture.LocationInView(gesture.View));
                            break;

                        case UIGestureRecognizerState.Ended:
                            Control.EndInteractiveMovement();
                            if (_itemsSource is IList itemsSourceList)
                            {
                                try
                                {
                                    _isMovedBackfire = true;
                                    var item = itemsSourceList[from];
                                    itemsSourceList.RemoveAt(from);
                                    itemsSourceList.Insert(to, item);
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