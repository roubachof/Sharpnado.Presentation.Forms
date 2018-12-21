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
            private readonly List<object> _dataSource;

            private readonly ViewHolderQueue _viewHolderQueue;

            private readonly List<DataTemplate> _dataTemplates = new List<DataTemplate>(3);

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

                if (!(_element.ItemTemplate is DataTemplateSelector))
                {
                    // Cache only support single DataTemplate
                    _viewHolderQueue = new ViewHolderQueue(element.ViewCacheSize, () => CreateViewHolder());
                    _viewHolderQueue.Build();
                }

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

            public override int GetItemViewType(int position)
            {
                if (_element.ItemTemplate is DataTemplateSelector dataTemplateSelector)
                {
                    var dataTemplate = dataTemplateSelector.SelectTemplate(_dataSource[position], _element.Parent);

                    int itemViewType = _dataTemplates.IndexOf(dataTemplate);
                    if (itemViewType == -1)
                    {
                        itemViewType = _dataTemplates.Count;
                        _dataTemplates.Add(dataTemplate);
                    }

                    return itemViewType;
                }

                return base.GetItemViewType(position);
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var item = (ViewHolder)holder;

                item.Bind(_dataSource[position]);
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                if (_element.ItemTemplate is DataTemplateSelector)
                {
                    return CreateViewHolder(viewType);
                }

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
                _viewHolderQueue?.Clear();

                if (_notifyCollectionChanged != null)
                {
                    _notifyCollectionChanged.CollectionChanged -= OnCollectionChanged;
                }

                base.Dispose(disposing);
            }

            private ViewHolder CreateViewHolder(int itemViewType = -1)
            {
                var view = CreateView(out var viewCell, itemViewType);

                if (_element.ListLayout == HorizontalListViewLayout.Grid && _element.ColumnCount == 0)
                {
                    var contentFrame = new FrameLayout(_context)
                    {
                        LayoutParameters = new FrameLayout.LayoutParams(
                            LayoutParams.MatchParent,
                            (int)(_element.ItemHeight * Resources.System.DisplayMetrics.Density)),
                    };

                    contentFrame.AddView(view);
                    view = contentFrame;
                }

                return new ViewHolder(view, viewCell);
            }

            private View CreateView(out ViewCell viewCell, int itemViewType)
            {
                viewCell = null;
                var dataTemplate = _element.ItemTemplate;

                if (itemViewType == -1)
                {
                    viewCell = (ViewCell)dataTemplate.CreateContent();
                }
                else
                {
                    viewCell = (ViewCell)_dataTemplates[itemViewType].CreateContent();
                }

                viewCell.Parent = _element;
                viewCell.View.Layout(new Rectangle(0, 0, _element.ItemWidth, _element.ItemHeight));

                LayoutParams layoutParams = null;
                if (_element.ListLayout == HorizontalListViewLayout.Grid && _element.ColumnCount == 0)
                {
                    layoutParams = new FrameLayout.LayoutParams(
                        (int)(_element.ItemWidth * Resources.System.DisplayMetrics.Density),
                        (int)(_element.ItemHeight * Resources.System.DisplayMetrics.Density))
                    {
                        Gravity = GravityFlags.CenterHorizontal,
                    };
                }
                else
                {
                    layoutParams = new LayoutParams(
                        (int)(_element.ItemWidth * Resources.System.DisplayMetrics.Density),
                        (int)(_element.ItemHeight * Resources.System.DisplayMetrics.Density));
                }

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
                        OnItemAdded(e.NewStartingIndex, e.NewItems);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        OnItemRemoved(e.OldStartingIndex, e.OldItems.Count);
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        _dataSource.Clear();
                        NotifyDataSetChanged();
                        break;
                }
            }

            private void OnItemAdded(int newIndex, IList items)
            {
                // System.Diagnostics.Debug.WriteLine($"OnItemAdded( newIndex: {newIndex} )");
                using (var h = new Handler(Looper.MainLooper))
                {
                    h.Post(
                        () =>
                        {
                            _dataSource.InsertRange(newIndex, items.Cast<object>());
                            if (items.Count == 1)
                            {
                                NotifyItemInserted(newIndex);
                            }
                            else
                            {
                                NotifyItemRangeInserted(newIndex, items.Count);
                            }
                        });
                }
            }

            private void OnItemRemoved(int removedIndex, int itemCount)
            {
                // System.Diagnostics.Debug.WriteLine($"OnItemRemoved( removedIndex: {removedIndex} )");
                using (var h = new Handler(Looper.MainLooper))
                {
                    h.Post(
                        () =>
                        {
                            for (int index = removedIndex; index < removedIndex + itemCount; index++)
                            {
                                var data = _dataSource[index];
                                Unbind(data);
                            }

                            _dataSource.RemoveRange(removedIndex, itemCount);
                            if (itemCount == 1)
                            {
                                NotifyItemRemoved(removedIndex);
                            }
                            else
                            {
                                NotifyItemRangeRemoved(removedIndex, itemCount);
                            }
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
                    viewCell.Parent = null;
                }
            }
        }
    }
}