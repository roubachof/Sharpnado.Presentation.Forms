using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.Support.V7.Widget;
using Android.Views;

using Sharpnado.Infrastructure.Tasks;
using Sharpnado.Presentation.Forms.Droid.Helpers;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public class ViewHolderQueue
    {
        private readonly Func<RecyclerView.ViewHolder> _viewFactory;
        private readonly Queue<JniWeakReference<RecyclerView.ViewHolder>> _views;
        private readonly int _initialSize;

        public ViewHolderQueue(int initialSize, Func<RecyclerView.ViewHolder> viewFactory)
        {
            _initialSize = initialSize;
            _viewFactory = viewFactory;
            _views = new Queue<JniWeakReference<RecyclerView.ViewHolder>>(initialSize);
        }

        public void Build()
        {
            NotifyTask.Create(BuildViewsAsync);
        }

        public void Clear()
        {
            lock (_views)
            {
                foreach (var view in _views)
                {
                    if (view.TryGetTarget(out var holder))
                    {
                        if (holder.ItemView is ViewGroup viewGroup)
                        {
                            viewGroup.RemoveAllViews();
                        }

                        holder.ItemView?.Dispose();
                    }
                }

                _views.Clear();
            }
        }

        public RecyclerView.ViewHolder Dequeue()
        {
            lock (_views)
            {
                if (_views.Count > 0 && _views.Dequeue().TryGetTarget(out var result))
                {
                    return result;
                }
            }

            return _viewFactory();
        }

        private Task BuildViewsAsync()
        {
            return Task.Run(
                () =>
                {
                    for (int i = 0; i < _initialSize; i++)
                    {
                        var view = _viewFactory();
                        lock (_views)
                        {
                            _views.Enqueue(new JniWeakReference<RecyclerView.ViewHolder>(view));
                        }
                    }
                });
        }
    }
}