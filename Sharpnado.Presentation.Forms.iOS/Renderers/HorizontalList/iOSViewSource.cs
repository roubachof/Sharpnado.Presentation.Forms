using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

using Foundation;

using Sharpnado.Presentation.Forms.RenderedViews;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace ClassLibrary1.Renderers.HorizontalList
{
    public class ViewCellHolder
    {
        public ViewCellHolder()
            : this(null)
        {
        }

        public ViewCellHolder(ViewCell viewCell)
        {
            FormsCell = viewCell;
        }

        public ViewCell FormsCell { get; set; }
    }

    public class UIViewCellHolder
    {
        public UIViewCellHolder(ViewCell formsCell, UIView view)
        {
            FormsCell = formsCell;
            CellContent = view;
        }

        public ViewCell FormsCell { get; }

        public UIView CellContent { get; }
    }

    public class iOSViewCell : UICollectionViewCell
    {
        public iOSViewCell(IntPtr p)
            : base(p)
        {
        }

        public bool IsInitialized => FormsCell != null;

        public ViewCell FormsCell { get; private set; }

        public void Initialize(ViewCell formsCell, UIView view)
        {
            FormsCell = formsCell;
            ContentView.AddSubview(view);
        }

        public void Bind(object dataContext)
        {
            FormsCell.BindingContext = dataContext;
        }
    }

    public class iOSViewSource : UICollectionViewDataSource
    {
        private readonly HorizontalListView _element;

        private readonly IList _dataSource;
        private readonly UIViewCellHolderQueue _viewCellHolderCellHolderQueue;

        public iOSViewSource(HorizontalListView element)
        {
            _element = element;

            var elementItemsSource = _element.ItemsSource;
            _dataSource = elementItemsSource?.Cast<object>().ToList();

            if (_dataSource == null)
            {
                return;
            }

            _viewCellHolderCellHolderQueue = new UIViewCellHolderQueue(element.ViewCacheSize, CreateViewCellHolder);
            _viewCellHolderCellHolderQueue.Build();
        }

        public void HandleNotifyCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
        {
            switch (eventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _dataSource.Insert(eventArgs.NewStartingIndex, eventArgs.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _dataSource.RemoveAt(eventArgs.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _dataSource.Clear();
                    break;
                case NotifyCollectionChangedAction.Move:
                    var item = _dataSource[eventArgs.OldStartingIndex];
                    _dataSource.RemoveAt(eventArgs.OldStartingIndex);
                    _dataSource.Insert(eventArgs.NewStartingIndex, item);
                    break;
            }
        }

        public override bool CanMoveItem(UICollectionView collectionView, NSIndexPath indexPath) =>
            _element.EnableDragAndDrop;

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
            var nativeCell = (iOSViewCell)collectionView.DequeueReusableCell(nameof(iOSViewCell), indexPath);

            if (!nativeCell.IsInitialized)
            {
                var holder = _viewCellHolderCellHolderQueue.Dequeue();
                nativeCell.Initialize(holder.FormsCell, holder.CellContent);
            }

            nativeCell.Bind(_dataSource[indexPath.Row]);

            return nativeCell;
        }

        protected override void Dispose(bool disposing)
        {
            _viewCellHolderCellHolderQueue?.Clear();

            base.Dispose(disposing);
        }

        private UIViewCellHolder CreateViewCellHolder()
        {
            ViewCell formsCell = null;
            if (_element.ItemTemplate is DataTemplateSelector selector)
            {
                // TODO: handle DataTemplateSelector
                // var template = selector.SelectTemplate(_dataSource[index], _element.Parent);
                // formsCell = (ViewCell)template.CreateContent();
                throw new NotSupportedException();
            }
            else
            {
                formsCell = (ViewCell)_element.ItemTemplate.CreateContent();
            }

            formsCell.View.Layout(new Rectangle(0, 0, _element.ItemWidth, _element.ItemHeight));

            if (Platform.GetRenderer(formsCell.View) == null)
            {
                IVisualElementRenderer renderer = null;
                renderer = Platform.CreateRenderer(formsCell.View);
                Platform.SetRenderer(formsCell.View, renderer);
            }

            var nativeView = Platform.GetRenderer(formsCell.View).NativeView;
            nativeView.ContentMode = UIViewContentMode.ScaleAspectFit;
            return new UIViewCellHolder(formsCell, nativeView);
        }
    }
}