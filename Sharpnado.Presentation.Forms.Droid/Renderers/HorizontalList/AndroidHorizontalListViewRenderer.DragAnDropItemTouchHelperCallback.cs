using System;
using System.Windows.Input;

using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;

using Sharpnado.Presentation.Forms.RenderedViews;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public partial class AndroidHorizontalListViewRenderer
    {
        private class DragAnDropItemTouchHelperCallback : ItemTouchHelper.Callback
        {
            private readonly HorizontalListView _horizontalList;

            private readonly RecycleViewAdapter _recycleViewAdapter;
            private readonly ICommand _onDragAndDropdEnded;

            private int _from = -1;
            private int _to = -1;

            private DraggableViewCell _draggedViewCell;

            public DragAnDropItemTouchHelperCallback(IntPtr handle, JniHandleOwnership transfer)
                : base(handle, transfer)
            {
            }

            public DragAnDropItemTouchHelperCallback(HorizontalListView horizontalList, RecycleViewAdapter recycleViewAdapter, ICommand onDragAndDropdEnded = null)
            {
                _horizontalList = horizontalList;
                _recycleViewAdapter = recycleViewAdapter;
                _onDragAndDropdEnded = onDragAndDropdEnded;
            }

            public override bool IsLongPressDragEnabled => true;

            public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
            {
                return MakeMovementFlags(ItemTouchHelper.Left | ItemTouchHelper.Right | ItemTouchHelper.Up | ItemTouchHelper.Down, 0);
            }

            public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
            {
                base.OnSelectedChanged(viewHolder, actionState);

                if (actionState == ItemTouchHelper.ActionStateDrag)
                {
                    _horizontalList.IsDragAndDropping = true;
                    if (((ViewHolder)viewHolder).ViewCell is DraggableViewCell draggableViewCell)
                    {
                        // System.Diagnostics.Debug.WriteLine($">>>>> OnSelectedChanged( {draggableViewCell.BindingContext} IsDragAndDropping: true )");
                        draggableViewCell.IsDragAndDropping = true;
                        _draggedViewCell = draggableViewCell;
                    }
                }
                else if (actionState == ItemTouchHelper.ActionStateIdle)
                {
                    _horizontalList.IsDragAndDropping = false;
                    if (_draggedViewCell != null)
                    {
                        // System.Diagnostics.Debug.WriteLine($">>>>> OnSelectedChanged( {_draggedViewCell.BindingContext} IsDragAndDropping: false )");
                        _draggedViewCell.IsDragAndDropping = false;
                        _draggedViewCell = null;
                    }
                }
            }

            public override bool OnMove(
                RecyclerView recyclerView,
                RecyclerView.ViewHolder viewHolder,
                RecyclerView.ViewHolder target)
            {
                if (_from == -1)
                {
                    _from = viewHolder.AdapterPosition;
                }

                _to = target.AdapterPosition;

                // System.Diagnostics.Debug.WriteLine($">>>>> OnMove( from: {viewHolder.AdapterPosition}, to: {target.AdapterPosition} )");
                _recycleViewAdapter.OnItemMoving(viewHolder.AdapterPosition, target.AdapterPosition);

                return true;
            }

            public override void OnMoved(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, int fromPos, RecyclerView.ViewHolder target, int toPos, int x, int y)
            {
                base.OnMoved(recyclerView, viewHolder, fromPos, target, toPos, x, y);

                // recompute items offsets
                recyclerView.InvalidateItemDecorations();
            }

            public override int InterpolateOutOfBoundsScroll(
                RecyclerView recyclerView,
                int viewSize,
                int viewSizeOutOfBounds,
                int totalSize,
                long msSinceStartScroll)
            {
                int result = Math.Sign(viewSizeOutOfBounds) * 30;
                return result;
            }

            public override float GetMoveThreshold(RecyclerView.ViewHolder viewHolder)
            {
                float result = 1f;
                return result;
            }

            public override void ClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
            {
                base.ClearView(recyclerView, viewHolder);

                if (_from > -1 && _to > -1)
                {
                    _recycleViewAdapter.OnItemMoved(_from, _to);
                    _onDragAndDropdEnded?.Execute(null);
                    _from = _to = -1;
                }
            }

            public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
            {
                throw new NotSupportedException();
            }
        }
    }
}