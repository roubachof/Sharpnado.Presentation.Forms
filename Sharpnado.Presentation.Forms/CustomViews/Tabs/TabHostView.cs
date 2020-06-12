using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sharpnado.Presentation.Forms.Commands;
using Sharpnado.Presentation.Forms.Effects;
using Sharpnado.Shades;

using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.CustomViews.Tabs
{
    public enum TabType
    {
        Fixed = 0,
        Scrollable,
    }

    [ContentProperty("TabHostContent")]
    public class TabHostView : Shadows
    {
        public static readonly BindableProperty TabsProperty = BindableProperty.Create(
            nameof(Tabs),
            typeof(ObservableCollection<TabItem>),
            typeof(TabHostView),
            defaultValueCreator: _ => new ObservableCollection<TabItem>());

        public static readonly BindableProperty IsSegmentedProperty = BindableProperty.Create(
            nameof(IsSegmented),
            typeof(bool),
            typeof(TabHostView),
            defaultValue: false,
            defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty SegmentedOutlineColorProperty = BindableProperty.Create(
            nameof(SegmentedOutlineColor),
            typeof(Color),
            typeof(TabHostView),
            defaultValue: Color.Default);

        public static readonly BindableProperty TabTypeProperty = BindableProperty.Create(
            nameof(TabType),
            typeof(TabType),
            typeof(TabHostView),
            defaultValue: TabType.Fixed,
            defaultBindingMode: BindingMode.OneWayToSource);

        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
            nameof(SelectedIndex),
            typeof(int),
            typeof(TabHostView),
            defaultValue: -1,
            propertyChanged: SelectedIndexPropertyChanged);

        public static readonly new BindableProperty BackgroundColorProperty = BindableProperty.Create(
            nameof(BackgroundColor),
            typeof(Color),
            typeof(TabHostView),
            Color.Transparent);

        private readonly Grid _grid;
        private readonly Frame _frame;
        private readonly List<TabItem> _selectableTabs = new List<TabItem>();

        private int _childRow = 0;

        private ScrollView _scrollView;

        private ColumnDefinition _lastFillingColumn;

        public TabHostView()
        {
            TabItemTappedCommand = new TapCommand(OnTabItemTapped);

            Tabs.CollectionChanged += OnTabsCollectionChanged;

            _frame = new Frame
            {
                Padding = 0,
                HasShadow = false,
                IsClippedToBounds = true,
                CornerRadius = this.CornerRadius,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Fill,
            };

            _grid = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Fill,
            };

            UpdateTabType();

            Shades = new List<Shade>();
        }

        public event EventHandler<SelectedPositionChangedEventArgs> SelectedTabIndexChanged;

        public ObservableCollection<TabItem> Tabs
        {
            get => (ObservableCollection<TabItem>)GetValue(TabsProperty);
            set => SetValue(TabsProperty, value);
        }

        public bool IsSegmented
        {
            get => (bool)GetValue(IsSegmentedProperty);
            set => SetValue(IsSegmentedProperty, value);
        }

        /// <summary>
        /// Only available if IsSegmented is true.
        /// </summary>
        public Color SegmentedOutlineColor
        {
            get => (Color)GetValue(SegmentedOutlineColorProperty);
            set => SetValue(SegmentedOutlineColorProperty, value);
        }

        public TabType TabType
        {
            get => (TabType)GetValue(TabTypeProperty);
            set => SetValue(TabTypeProperty, value);
        }

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public new Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public new View Content
        {
            get => base.Content;
            set =>
                throw new NotSupportedException(
                    "You can only add TabItem to the TabHostView through the Tabs property");
        }

        public View TabHostContent
        {
            set =>
                throw new NotSupportedException(
                    "Starting from version 1.3, you can only add TabItem to the TabHostView through the Tabs property");
        }

        public bool ShowScrollbar { get; set; }

        private ICommand TabItemTappedCommand { get; }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(BackgroundColor):
                    UpdateBackgroundColor();
                    break;
                case nameof(CornerRadius):
                    UpdateCornerRadius();
                    break;
                case nameof(IsSegmented):
                case nameof(TabType):
                    UpdateTabType();
                    break;
                case nameof(Tabs):
                    throw new NotSupportedException("Updating Tabs collection reference is not supported");
            }
        }

        private void UpdateBackgroundColor()
        {
            _grid.BackgroundColor = BackgroundColor;
        }

        private void UpdateCornerRadius()
        {
            _frame.CornerRadius = CornerRadius;
        }

        private static void SelectedIndexPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var tabHostView = (TabHostView)bindable;

            int selectedIndex = (int)newvalue;
            if (selectedIndex < 0)
            {
                return;
            }

            tabHostView.UpdateSelectedIndex(selectedIndex);
            tabHostView.RaiseSelectedTabIndexChanged(new SelectedPositionChangedEventArgs(selectedIndex));
        }

        private void UpdateSelectedIndex(int selectedIndex)
        {
            if (_selectableTabs.Count == 0)
            {
                selectedIndex = 0;
            }

            if (selectedIndex > _selectableTabs.Count)
            {
                selectedIndex = _selectableTabs.Count - 1;
            }

            for (int index = 0; index < _selectableTabs.Count; index++)
            {
                _selectableTabs[index].IsSelected = selectedIndex == index;
            }

            SelectedIndex = selectedIndex;
        }

        private void OnTabItemTapped(object tappedItem)
        {
            int selectedIndex = _selectableTabs.IndexOf((TabItem)tappedItem);

            UpdateSelectedIndex(selectedIndex);
            RaiseSelectedTabIndexChanged(new SelectedPositionChangedEventArgs(selectedIndex));
        }

        private void UpdateTabType()
        {
            if (TabType == TabType.Scrollable)
            {
                base.Content = _scrollView
                    ?? (_scrollView = new ScrollView()
                    {
                        Orientation = ScrollOrientation.Horizontal,
                        HorizontalScrollBarVisibility =
                            ShowScrollbar ? ScrollBarVisibility.Always : ScrollBarVisibility.Never,
                    });

                if (IsSegmented)
                {
                    _frame.Content = _grid;
                    _scrollView.Content = _frame;
                }
                else
                {
                    _scrollView.Content = _grid;
                }

                foreach (var definition in _grid.ColumnDefinitions)
                {
                    definition.Width = GridLength.Auto;
                }
            }
            else
            {
                if (IsSegmented)
                {
                    _frame.Content = _grid;
                    base.Content = _frame;
                }
                else
                {
                    base.Content = _grid;
                }

                foreach (var definition in _grid.ColumnDefinitions)
                {
                    definition.Width = GridLength.Star;
                }
            }
        }

        private void OnTabsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var tab in e.NewItems)
                    {
                        OnChildAdded((TabItem)tab);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var tab in e.OldItems)
                    {
                        OnChildRemoved((TabItem)tab);
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                default:
                    throw new NotSupportedException();
            }
        }

        private void AddTapCommand(TabItem tabItem)
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                tabItem.GestureRecognizers.Add(
                    new TapGestureRecognizer() { Command = TabItemTappedCommand, CommandParameter = tabItem });
            }
            else
            {
                ViewEffect.SetTouchFeedbackColor(tabItem, tabItem.SelectedTabColor);
                TapCommandEffect.SetTap(tabItem, TabItemTappedCommand);
                TapCommandEffect.SetTapParameter(tabItem, tabItem);

                tabItem.Effects.Add(new ViewStyleEffect());
                tabItem.Effects.Add(new TapCommandRoutingEffect());
            }
        }

        private void OnChildAdded(TabItem tabItem)
        {
            _grid.Children.Add(tabItem);

            _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = TabType == TabType.Fixed ? GridLength.Star : GridLength.Auto });

            if (TabType == TabType.Scrollable)
            {
                if (Tabs.Count == 1)
                {
                    // Add a last empty slot to fill remaining space
                    _lastFillingColumn = new ColumnDefinition { Width = GridLength.Star };
                    _grid.ColumnDefinitions.Add(_lastFillingColumn);
                }
                else
                {
                    _grid.ColumnDefinitions.Remove(_lastFillingColumn);
                    _grid.ColumnDefinitions.Add(_lastFillingColumn);
                }
            }

            Grid.SetColumn(tabItem, Tabs.Count - 1);
            Grid.SetRow(tabItem, _childRow);

            RaiseTabButtons();

            if (tabItem.IsSelectable)
            {
                AddTapCommand(tabItem);
                _selectableTabs.Add(tabItem);
            }

            if (TabType == TabType.Fixed)
            {
                tabItem.PropertyChanged += OnTabItemPropertyChanged;
                UpdateTabVisibility(tabItem);
            }

            UpdateSelectedIndex(SelectedIndex);
        }

        private void OnChildRemoved(TabItem tabItem)
        {
            if (_grid.ColumnDefinitions.Count == 0)
            {
                return;
            }

            if (TabType == TabType.Scrollable)
            {
                if (Tabs.Count == 0)
                {
                    _grid.ColumnDefinitions.Remove(_lastFillingColumn);
                }
            }

            if (tabItem.IsSelectable)
            {
                _selectableTabs.Remove(tabItem);
            }

            _grid.Children.Remove(tabItem);

            _grid.ColumnDefinitions.RemoveAt(_grid.ColumnDefinitions.Count - 1);

            tabItem.PropertyChanged -= OnTabItemPropertyChanged;

            UpdateSelectedIndex(SelectedIndex);
        }

        private void OnTabItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tabItem = (TabItem)sender;
            if (e.PropertyName == nameof(TabItem.IsVisible))
            {
                UpdateTabVisibility(tabItem);
            }
        }

        private void UpdateTabVisibility(TabItem tabItem)
        {
            int columnIndex = Grid.GetColumn(tabItem);
            var columnDefinition = _grid.ColumnDefinitions[columnIndex];
            columnDefinition.Width = tabItem.IsVisible ? GridLength.Star : 0;
        }

        private void RaiseSelectedTabIndexChanged(SelectedPositionChangedEventArgs e)
        {
            SelectedTabIndexChanged?.Invoke(this, e);
        }

        private void RaiseTabButtons()
        {
            foreach (var tabButton in Tabs.Where(t => t is TabButton))
            {
                // We always want our TabButton with the highest Z-index
                _grid.RaiseChild(tabButton);
            }
        }
    }
}