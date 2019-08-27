using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using Sharpnado.Presentation.Forms.Paging;
using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.RenderedViews
{
    public enum HorizontalListViewLayout
    {
        Linear = 0,
        Grid,
        Carousel,
    }

    public enum SnapStyle
    {
        None = 0,
        Start,
        Center,
    }

    /// <summary>
    /// Slower and slowest have the same result on iOS.
    /// </summary>
    public enum ScrollSpeed
    {
        Normal = 0,
        Slower,
        Slowest,
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
        public static readonly BindableProperty ListLayoutProperty = BindableProperty.Create(
            nameof(ListLayout),
            typeof(HorizontalListViewLayout),
            typeof(HorizontalListView),
            HorizontalListViewLayout.Linear,
            propertyChanged: OnListLayoutChanged);

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(HorizontalListView),
            default(IEnumerable<object>),
            BindingMode.TwoWay,
            propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty InfiniteListLoaderProperty = BindableProperty.Create(
            nameof(InfiniteListLoader),
            typeof(IInfiniteListLoader),
            typeof(HorizontalListView));

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(HorizontalListView),
            default(DataTemplate));

        public static readonly BindableProperty ItemHeightProperty = BindableProperty.Create(
            nameof(ItemHeight),
            typeof(double),
            typeof(HorizontalListView),
            defaultValue: 0D,
            defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty ItemWidthProperty = BindableProperty.Create(
            nameof(ItemWidth),
            typeof(double),
            typeof(HorizontalListView),
            defaultValue: 0D,
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

        public static readonly BindableProperty TapCommandProperty = BindableProperty.Create(
            nameof(TapCommand),
            typeof(ICommand),
            typeof(HorizontalListView));

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
            defaultValue: -1,
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

        private HorizontalListViewLayout _layout = HorizontalListViewLayout.Linear;

        public int CurrentIndex
        {
            get => (int)GetValue(CurrentIndexProperty);
            set => SetValue(CurrentIndexProperty, value);
        }

        /// <summary>
        /// The platform renderers doesn't handle changes on this property: this is OneWayToSource binding.
        /// This property is only bindable to allow styling.
        /// </summary>
        public double ItemHeight
        {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        /// <summary>
        /// The platform renderers doesn't handle changes on this property: this is OneWayToSource binding.
        /// This property is only bindable to allow styling.
        /// </summary>
        public double ItemWidth
        {
            get => (double)GetValue(ItemWidthProperty);
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

        public ICommand TapCommand
        {
            get => (ICommand)GetValue(TapCommandProperty);
            set => SetValue(TapCommandProperty, value);
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

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public bool IsDragAndDropping
        {
            get => (bool)GetValue(IsDragAndDroppingProperty);
            set => SetValue(IsDragAndDroppingProperty, value);
        }

        public int ViewCacheSize { get; set; } = 0;

        public bool EnableDragAndDrop { get; set; } = false;

        public HorizontalListViewLayout ListLayout
        {
            get => (HorizontalListViewLayout)GetValue(ListLayoutProperty);
            set => SetValue(ListLayoutProperty, value);
        }

        public SnapStyle SnapStyle { get; set; } = SnapStyle.None;

        public int ColumnCount { get; set; } = 0;

        public ScrollSpeed ScrollSpeed { get; set; } = ScrollSpeed.Normal;

        public bool IsLayoutLinear =>
            ListLayout == HorizontalListViewLayout.Linear || ListLayout == HorizontalListViewLayout.Carousel;

        public void CheckConsistency()
        {
            if (ListLayout == HorizontalListViewLayout.Carousel
                && (ColumnCount != 1 || SnapStyle != SnapStyle.Center))
            {
                throw new InvalidOperationException(
                    "When setting ListLayout to Carousel, you can only set ColumnCount to 1 and SnapStyle to Center");
            }
        }

        /// <summary>
        /// Automatically compute item width for a given parent width and a given column count.
        /// </summary>
        /// <remarks>This method is Pure.</remarks>
        [Pure]
        public double ComputeItemWidth(double availableWidth)
        {
            if (ColumnCount == 0)
            {
                throw new InvalidOperationException("ColumnCount should be greater than 0 in order to automatically compute item width");
            }

            int itemSpace = PlatformHelper.Instance.DpToPixels(ItemSpacing);
            int leftPadding = PlatformHelper.Instance.DpToPixels(CollectionPadding.Left);
            int rightPadding = PlatformHelper.Instance.DpToPixels(CollectionPadding.Right);

            int totalWidthSpacing = itemSpace * (ColumnCount - 1) + leftPadding + rightPadding;

            double spaceWidthLeft = availableWidth - totalWidthSpacing;

            return PlatformHelper.Instance.PixelsToDp(Math.Floor((spaceWidthLeft / ColumnCount) * 100) / 100);
        }

        /// <summary>
        /// Automatically compute item height for a given parent height.
        /// </summary>
        /// <remarks>This method is Pure.</remarks>
        [Pure]
        public double ComputeItemHeight(double availableHeight)
        {
            if (ListLayout == HorizontalListViewLayout.Grid || ItemHeight > 0)
            {
                throw new InvalidOperationException(
                    "Can compute item height only if a height has not been specified and layout is horizontal linear");
            }

            int topPadding = PlatformHelper.Instance.DpToPixels(CollectionPadding.Top);
            int bottomPadding = PlatformHelper.Instance.DpToPixels(CollectionPadding.Bottom);

            int totalHeightSpacing = topPadding + bottomPadding;

            double spaceHeightLeft = availableHeight - totalHeightSpacing;

            return PlatformHelper.Instance.PixelsToDp(spaceHeightLeft);
        }

        private static void OnListLayoutChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var horizontalListView = (HorizontalListView)bindable;
            var newLayout = (HorizontalListViewLayout)newvalue;
            if (newLayout == HorizontalListViewLayout.Carousel)
            {
                horizontalListView.SnapStyle = SnapStyle.Center;
                horizontalListView.ColumnCount = 1;
                horizontalListView.ScrollSpeed = ScrollSpeed.Slowest;
            }
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
}