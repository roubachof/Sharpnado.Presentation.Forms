using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using Sharpnado.Infrastructure;
using Sharpnado.Presentation.Forms.RenderedViews;

using Xamarin.Forms;

using IList = System.Collections.IList;
using View = Android.Views.View;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.HorizontalList
{
    public partial class AndroidHorizontalListViewRenderer
    {
        private class ViewHolder : RecyclerView.ViewHolder
        {
            private readonly ViewCell _viewCell;

            public ViewHolder(View itemView, ViewCell viewCell)
                : base(itemView)
            {
                _viewCell = viewCell;
            }

            public ViewCell ViewCell => _viewCell;

            public object BindingContext => ViewCell?.BindingContext;

            public void Bind(object context)
            {
                _viewCell.BindingContext = context;
            }
        }

        private class RecycleViewAdapter : RecyclerView.Adapter
        {
            private readonly Context _context;
            private readonly HorizontalListView _element;
            private readonly IEnumerable _elementItemsSource;
            private readonly INotifyCollectionChanged _notifyCollectionChanged;
            private readonly IList<object> _dataSource;

            private readonly ViewHolderQueue _viewHolderQueue;

            private readonly List<WeakReference<ViewCell>> _formsViews;

            private bool _collectionChangedBackfire;

            public RecycleViewAdapter(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public RecycleViewAdapter(HorizontalListView element, Context context)
            {
                _element = element;
                _context = context;

                _elementItemsSource = element.ItemsSource;

                _dataSource = _elementItemsSource?.Cast<object>().ToList() ?? new List<object>();

                _formsViews = new List<WeakReference<ViewCell>>();
                _viewHolderQueue = new ViewHolderQueue(element.ViewCacheSize, CreateViewHolder);
                _viewHolderQueue.Build();

                _notifyCollectionChanged = _elementItemsSource as INotifyCollectionChanged;
                if (_notifyCollectionChanged != null)
                {
                    _notifyCollectionChanged.CollectionChanged += OnCollectionChanged;
                }
            }

            public override int ItemCount => _dataSource.Count;

            public override long GetItemId(int position)
            {
                return position;
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var item = (ViewHolder)holder;

                item.Bind(_dataSource[position]);
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                return _viewHolderQueue.Dequeue();
            }

            public void OnItemMoved(int from, int to)
            {
                if (_elementItemsSource is IList collection)
                {
                    try
                    {
                        _collectionChangedBackfire = true;

                        var item = collection[from];
                        collection.RemoveAt(from);
                        collection.Insert(to, item);
                    }
                    finally
                    {
                        _collectionChangedBackfire = false;
                    }
                }
            }

            public void OnItemMoving(int from, int to)
            {
                using (var h = new Handler(Looper.MainLooper))
                {
                    h.Post(
                        () =>
                        {
                            if (from < to)
                            {
                                for (int i = from; i < to; i++)
                                {
                                    _dataSource.Swap(i, i + 1);
                                }
                            }
                            else
                            {
                                for (int i = from; i > to; i--)
                                {
                                    _dataSource.Swap(i, i - 1);
                                }
                            }

                            NotifyItemMoved(from, to);
                        });
                }
            }

            protected override void Dispose(bool disposing)
            {
                _viewHolderQueue.Clear();

                if (_notifyCollectionChanged != null)
                {
                    _notifyCollectionChanged.CollectionChanged -= OnCollectionChanged;
                }

                base.Dispose(disposing);
            }

            private ViewHolder CreateViewHolder()
            {
                var view = CreateView(out var viewCell);

                var contentFrame = new FrameLayout(_context)
                {
                    LayoutParameters =
                        new FrameLayout.LayoutParams(
                            (int)(_element.ItemWidth * Resources.System.DisplayMetrics.Density),
                            (int)(_element.ItemHeight * Resources.System.DisplayMetrics.Density)),
                };

                contentFrame.AddView(view);

                return new ViewHolder(contentFrame, viewCell);
            }

            private View CreateView(out ViewCell viewCell)
            {
                viewCell = null;
                var dataTemplate = _element.ItemTemplate;
                if (dataTemplate is DataTemplateSelector selector)
                {
                    // TODO: support templateSelector
                    // var template = selector.SelectTemplate(_dataSource[position], _element.Parent);
                    // viewCell = (ViewCell)template.CreateContent();
                    throw new NotSupportedException();
                }
                else
                {
                    viewCell = (ViewCell)dataTemplate.CreateContent();
                }

                viewCell.View.Layout(new Rectangle(0, 0, _element.ItemWidth, _element.ItemHeight));

                var layoutParams = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

                if (Xamarin.Forms.Platform.Android.Platform.GetRenderer(viewCell.View) == null)
                {
                    Xamarin.Forms.Platform.Android.Platform.SetRenderer(
                        viewCell.View,
                        Xamarin.Forms.Platform.Android.Platform.CreateRendererWithContext(viewCell.View, _context));
                }

                var renderer = Xamarin.Forms.Platform.Android.Platform.GetRenderer(viewCell.View);

                var view = renderer.View;
                view.LayoutParameters = layoutParams;

                _formsViews.Add(new WeakReference<ViewCell>(viewCell));
                return view;
            }

            private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (_collectionChangedBackfire)
                {
                    return;
                }

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        OnItemAdded(e.NewStartingIndex, e.NewItems[0]);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        OnItemRemoved(e.OldStartingIndex);
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        _dataSource.Clear();
                        NotifyDataSetChanged();
                        break;
                }
            }

            private void OnItemAdded(int newIndex, object item)
            {
                // System.Diagnostics.Debug.WriteLine($"OnItemAdded( newIndex: {newIndex} )");
                using (var h = new Handler(Looper.MainLooper))
                {
                    h.Post(
                        () =>
                        {
                            _dataSource.Insert(newIndex, item);
                            NotifyItemInserted(newIndex);
                        });
                }
            }

            private void OnItemRemoved(int removedIndex)
            {
                // System.Diagnostics.Debug.WriteLine($"OnItemRemoved( removedIndex: {removedIndex} )");
                using (var h = new Handler(Looper.MainLooper))
                {
                    h.Post(
                        () =>
                        {
                            var data = _dataSource[removedIndex];
                            Unbind(data);
                            _dataSource.RemoveAt(removedIndex);
                            NotifyItemRemoved(removedIndex);
                        });
                }
            }

            private void Unbind(object data)
            {
                // System.Diagnostics.Debug.WriteLine($"Unbind( data: {data} )");
                var weakViewCell = _formsViews.FirstOrDefault(
                    weakView => weakView.TryGetTarget(out ViewCell cell) && cell.BindingContext == data);

                if (weakViewCell == null)
                {
                    return;
                }

                if (weakViewCell.TryGetTarget(out var viewCell))
                {
                    viewCell.BindingContext = null;
                }
            }
        }
    }
}