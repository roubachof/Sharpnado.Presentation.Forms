using System;
using System.Collections.Generic;

namespace Sharpnado.Presentation.Forms.iOS.Renderers.HorizontalList
{
    public class UIViewCellHolderQueue
    {
        private readonly Func<UIViewCellHolder> _viewFactory;
        private readonly Queue<UIViewCellHolder> _views;
        private readonly int _initialSize;

        public UIViewCellHolderQueue(int initialSize, Func<UIViewCellHolder> viewFactory)
        {
            _initialSize = initialSize;
            _viewFactory = viewFactory;
            _views = new Queue<UIViewCellHolder>(initialSize);
        }

        public void Build()
        {
            // System.Diagnostics.Debug.WriteLine($"Build: creating {_initialSize} views");
            for (int i = 0; i < _initialSize; i++)
            {
                var view = _viewFactory();
                lock (_views)
                {
                    _views.Enqueue(view);
                }
            }
        }

        public void Clear()
        {
            lock (_views)
            {
                foreach (var view in _views)
                {
                    view.CellContent.Dispose();
                }

                _views.Clear();
            }
        }

        public UIViewCellHolder Dequeue()
        {
            lock (_views)
            {
                if (_views.Count > 0)
                {
                    // System.Diagnostics.Debug.WriteLine($"Dequeue: dequeueing cached view ({_views.Count} remaining)");
                    return _views.Dequeue();
                }
            }

            return _viewFactory();
        }
    }
}