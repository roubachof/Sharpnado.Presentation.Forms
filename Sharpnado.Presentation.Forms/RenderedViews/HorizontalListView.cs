using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Sharpnado.Presentation.Forms.Paging;
using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.RenderedViews
{
    public enum HorizontalListViewLayout
    {
        Linear = 0,
        Grid,
    }

    public enum SnapStyle
    {
        None = 0,
        Start,
        Center,
    }

    public class DraggableViewCell : ViewCell
    {
        public static readonly BindableProperty IsDragAndDroppingProperty = BindableProperty.Create(
            nameof(IsDragAndDropping),
            typeof(bool),
            typeof(DraggableViewCell),
            defaultValue: false);

        public bool IsDragAndDropping
        {
            get => (bool)GetValue(IsDragAndDroppingProperty);
            set => SetValue(IsDragAndDroppingProperty, value);
        }
    }

    public class HorizontalListView : View
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(HorizontalListView),
            default(IEnumerable<object>),
            BindingMode.TwoWay,
            propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty InfiniteListLoaderProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IInfiniteListLoader),
            typeof(HorizontalListView));

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(HorizontalListView),
            default(DataTemplate));

        public static readonly BindableProperty ItemHeightProperty = BindableProperty.Create(
            nameof(ItemHeight),
            typeof(int),
            typeof(HorizontalListView),
            defaultValue: 100,
            defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty ItemWidthProperty = BindableProperty.Create(
            nameof(ItemWidth),
            typeof(int),
            typeof(HorizontalListView),
            defaultValue: 100,
            defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty CollectionPaddingProperty = BindableProperty.Create(
            nameof(CollectionPadding),
            typeof(Thickness),
            typeof(HorizontalListView),
            defaultValue: new Thickness(0, 0),
            defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty ItemSpacingProperty = BindableProperty.Create(
            nameof(ItemSpacing),
            typeof(int),
            typeof(HorizontalListView),
            defaultValue: 0,
            defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty ScrollBeganCommandProperty = BindableProperty.Create(
            nameof(ScrollBeganCommand),
            typeof(ICommand),
            typeof(HorizontalListView));

        public static readonly BindableProperty ScrollEndedCommandProperty = BindableProperty.Create(
            nameof(ScrollEndedCommand),
            typeof(ICommand),
            typeof(HorizontalListView));

        public static readonly BindableProperty DragAndDropEndedCommandProperty = BindableProperty.Create(
            nameof(DragAndDropEndedCommand),
            typeof(ICommand),
            typeof(HorizontalListView));

        public static readonly BindableProperty CurrentIndexProperty = BindableProperty.Create(
            nameof(CurrentIndex),
            typeof(int),
            typeof(HorizontalListView),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnCurrentIndexChanged);

        public static readonly BindableProperty VisibleCellCountProperty = BindableProperty.Create(
            nameof(VisibleCellCount),
            typeof(int),
            typeof(HorizontalListView),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnVisibleCellCountChanged);

        public static readonly BindableProperty DisableScrollProperty = BindableProperty.Create(
            nameof(DisableScroll),
            typeof(bool),
            typeof(HorizontalListView),
            defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty IsDragAndDroppingProperty = BindableProperty.Create(
            nameof(IsDragAndDropping),
            typeof(bool),
            typeof(HorizontalListView),
            defaultValue: false);

        public int CurrentIndex
        {
            get => (int)GetValue(CurrentIndexProperty);
            set => SetValue(CurrentIndexProperty, value);
        }

        /// <summary>
        /// The platform renderers doesn't handle changes on this property: this is OneWayToSource binding.
        /// This property is only bindable to allow styling.
        /// </summary>
        public int ItemHeight
        {
            get => (int)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        /// <summary>
        /// The platform renderers doesn't handle changes on this property: this is OneWayToSource binding.
        /// This property is only bindable to allow styling.
        /// </summary>
        public int ItemWidth
        {
            get => (int)GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }

        /// <summary>
        /// The platform renderers doesn't handle changes on this property: this is OneWayToSource binding.
        /// This property is only bindable to allow styling.
        /// </summary>
        public int ItemSpacing
        {
            get => (int)GetValue(ItemSpacingProperty);
            set => SetValue(ItemSpacingProperty, value);
        }

        /// <summary>
        /// The platform renderers doesn't handle changes on this property: this is OneWayToSource binding.
        /// This property is only bindable to allow styling.
        /// </summary>
        public Thickness CollectionPadding
        {
            get => (Thickness)GetValue(CollectionPaddingProperty);
            set => SetValue(CollectionPaddingProperty, value);
        }

        public int ViewCacheSize { get; set; } = 0;

        public bool EnableDragAndDrop { get; set; } = false;

        public bool IsDragAndDropping
        {
            get => (bool)GetValue(IsDragAndDroppingProperty);
            set => SetValue(IsDragAndDroppingProperty, value);
        }

        public HorizontalListViewLayout ListLayout { get; set; } = HorizontalListViewLayout.Linear;

        public SnapStyle SnapStyle { get; set; } = SnapStyle.None;

        public int GridColumnCount { get; set; } = 0;

        public event System.EventHandler<ScrollRequestedEventArgs> OnScrollRequested;

        public int VisibleCellCount
        {
            get => (int)GetValue(VisibleCellCountProperty);
            set => SetValue(VisibleCellCountProperty, value);
        }

        public bool DisableScroll
        {
            get => (bool)GetValue(DisableScrollProperty);
            set => SetValue(DisableScrollProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public IInfiniteListLoader InfiniteListLoader
        {
            get => (IInfiniteListLoader)GetValue(InfiniteListLoaderProperty);
            set => SetValue(InfiniteListLoaderProperty, value);
        }

        public ICommand ScrollBeganCommand
        {
            get => (ICommand)GetValue(ScrollBeganCommandProperty);
            set => SetValue(ScrollBeganCommandProperty, value);
        }

        public ICommand ScrollEndedCommand
        {
            get => (ICommand)GetValue(ScrollEndedCommandProperty);
            set => SetValue(ScrollEndedCommandProperty, value);
        }

        public ICommand DragAndDropEndedCommand
        {
            get => (ICommand)GetValue(DragAndDropEndedCommandProperty);
            set => SetValue(DragAndDropEndedCommandProperty, value);
        }


        public void ScrollToPosition(int position,ScrollType type, bool animate)
        {
            OnScrollRequested?.Invoke(this, new ScrollRequestedEventArgs { Animate = animate, Position = position, Type = type });
        }


        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
        }

        private static void OnCurrentIndexChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
        }

        private static void OnVisibleCellCountChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
        }
    }


    public enum ScrollType
    {
        Start,Center,End
    }

    public class ScrollRequestedEventArgs
    {
        public int Position { get; set; }
        public ScrollType Type { get; set; }
        public bool Animate { get; set; }
    }

}