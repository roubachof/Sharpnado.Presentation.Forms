using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sharpnado.Presentation.Forms.Commands;
using Sharpnado.Presentation.Forms.Effects;
using Sharpnado.Presentation.Forms.RenderedViews;
using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.CustomViews.Tabs
{
    public enum TabType
    {
        Fixed = 0,
        Scrollable,
    }

    public class TabHostView : ContentView
    {
        public static readonly BindableProperty TabsProperty = BindableProperty.Create(
            nameof(Tabs),
            typeof(ObservableCollection<TabItem>),
            typeof(TabHostView),
            defaultValue: new ObservableCollection<TabItem>());

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

        public static readonly BindableProperty ShadowTypeProperty = BindableProperty.Create(
            nameof(ShadowType),
            typeof(ShadowType),
            typeof(TabHostView),
            defaultBindingMode: BindingMode.OneWayToSource);

        private const int ShadowHeight = 6;

        private readonly Grid _grid;

        private int _childRow = 0;
        private bool _isInitialized = false;

        private bool _isTabTypeSet = false;
        private bool _isShadowTypeSet = false;

        private ScrollView _scrollView;
        private BoxView _contentBackgroundView;
        private ShadowBoxView _shadow;

        public TabHostView()
        {
            TabItemTappedCommand = new TapCommand(OnTabItemTapped);

            Tabs.CollectionChanged += OnTabsCollectionChanged;

            _grid = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
            };
        }

        public event EventHandler<SelectedPositionChangedEventArgs> SelectedTabIndexChanged;

        public ObservableCollection<TabItem> Tabs
        {
            get => (ObservableCollection<TabItem>)GetValue(TabsProperty);
            set => SetValue(TabsProperty, value);
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

        public ShadowType ShadowType
        {
            get => (ShadowType)GetValue(ShadowTypeProperty);
            set => SetValue(ShadowTypeProperty, value);
        }

        private ICommand TabItemTappedCommand { get; }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            switch (propertyName)
            {
                case nameof(TabType):
                    _isTabTypeSet = true;
                    break;
                case nameof(ShadowType):
                    _isShadowTypeSet = true;
                    break;
            }

            if (_isInitialized && _isShadowTypeSet && _isTabTypeSet)
            {
                Initialize();
            }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            return base.OnMeasure(widthConstraint, heightConstraint);
        }

        protected override void InvalidateMeasure()
        {
            base.InvalidateMeasure();
        }

        protected override void OnParentSet()
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            base.OnParentSet();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
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

        private void OnTabItemTapped(object tappedItem)
        {
            int selectedIndex = Tabs.IndexOf((TabItem)tappedItem);
            UpdateSelectedIndex(selectedIndex);
            RaiseSelectedTabIndexChanged(new SelectedPositionChangedEventArgs(selectedIndex));
        }

        private void UpdateSelectedIndex(int selectedIndex)
        {
            if (Tabs.Count == 0)
            {
                selectedIndex = 0;
            }

            if (selectedIndex > Tabs.Count)
            {
                selectedIndex = Tabs.Count - 1;
            }

            for (int index = 0; index < Tabs.Count; index++)
            {
                Tabs[index].IsSelected = selectedIndex == index;
            }

            SelectedIndex = selectedIndex;
        }

        private void Initialize()
        {
            _isInitialized = true;

            if (TabType == TabType.Scrollable)
            {
                Content = _scrollView = new ScrollView() { Orientation = ScrollOrientation.Horizontal };
                _scrollView.Content = _grid;

                switch (ShadowType)
                {
                    case ShadowType.Top:
                        _scrollView.Margin = new Thickness(Margin.Left, Margin.Top - ShadowHeight, Margin.Right, Margin.Bottom);
                        break;
                    case ShadowType.Bottom:
                        _scrollView.Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom - ShadowHeight);
                        break;
                }
            }
            else
            {
                Content = _grid;

                switch (ShadowType)
                {
                    case ShadowType.Top:
                        _grid.Margin = new Thickness(Margin.Left, Margin.Top - ShadowHeight, Margin.Right, Margin.Bottom);
                        break;
                    case ShadowType.Bottom:
                        _grid.Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right, Margin.Bottom - ShadowHeight);
                        break;
                }
            }

            if (ShadowType != ShadowType.None)
            {
                _shadow = new ShadowBoxView { ShadowType = ShadowType };

                if (ShadowType == ShadowType.Top)
                {
                    _grid.RowDefinitions.Add(new RowDefinition { Height = ShadowHeight });
                    Grid.SetRow(_shadow, 0);
                    _childRow = 1;
                }

                _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

                if (ShadowType == ShadowType.Bottom)
                {
                    _grid.RowDefinitions.Add(new RowDefinition { Height = ShadowHeight });
                    _childRow = 0;
                    Grid.SetRow(_shadow, 1);
                }

                _contentBackgroundView = new BoxView { BackgroundColor = BackgroundColor };
                Grid.SetRow(_contentBackgroundView, _childRow);
                BackgroundColor = Color.Transparent;

                foreach (var element in _grid.Children)
                {
                    if (element is TabItem tabItem)
                    {
                        Grid.SetRow(tabItem, _childRow);
                    }
                }

                if (_grid.ColumnDefinitions.Count > 0)
                {
                    Grid.SetColumnSpan(_shadow, _grid.ColumnDefinitions.Count);
                    Grid.SetColumnSpan(_contentBackgroundView, _grid.ColumnDefinitions.Count);
                }

                _grid.Children.Add(_shadow);
                _grid.Children.Add(_contentBackgroundView);
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

        private void OnChildAdded(TabItem tabItem)
        {
            _grid.Children.Add(tabItem);

            _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = TabType == TabType.Fixed ? GridLength.Star : GridLength.Auto });
            Grid.SetColumn(tabItem, _grid.ColumnDefinitions.Count - 1);
            Grid.SetRow(tabItem, _childRow);

            ViewEffect.SetTouchFeedbackColor(tabItem, tabItem.SelectedTabColor);
            TapCommandEffect.SetTap(tabItem, TabItemTappedCommand);
            TapCommandEffect.SetTapParameter(tabItem, tabItem);

            tabItem.Effects.Add(new ViewStyleEffect());
            tabItem.Effects.Add(new TapCommandRoutingEffect());

            if (TabType == TabType.Fixed)
            {
                tabItem.PropertyChanged += OnTabItemPropertyChanged;
                UpdateTabVisibility(tabItem);
            }

            if (_shadow != null)
            {
                Grid.SetColumnSpan(_shadow, _grid.ColumnDefinitions.Count);
                Grid.SetColumnSpan(_contentBackgroundView, _grid.ColumnDefinitions.Count);
            }

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

        private void OnChildRemoved(TabItem tabItem)
        {
            if (_grid.ColumnDefinitions.Count == 0)
            {
                return;
            }

            _grid.Children.Remove(tabItem);

            _grid.ColumnDefinitions.RemoveAt(_grid.ColumnDefinitions.Count - 1);

            tabItem.PropertyChanged -= OnTabItemPropertyChanged;

            UpdateSelectedIndex(SelectedIndex);
        }

        private void RaiseSelectedTabIndexChanged(SelectedPositionChangedEventArgs e)
        {
            SelectedTabIndexChanged?.Invoke(this, e);
        }
    }
}