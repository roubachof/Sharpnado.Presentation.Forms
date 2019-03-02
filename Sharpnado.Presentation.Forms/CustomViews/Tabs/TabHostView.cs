using System;
using System.Collections.Generic;
using System.Windows.Input;
using Sharpnado.Presentation.Forms.Commands;
using Sharpnado.Presentation.Forms.Effects;
using Sharpnado.Presentation.Forms.RenderedViews;
using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.CustomViews.Tabs
{
    public class TabHostView : Grid
    {
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
            propertyChanged: ShadowTypePropertyChanged);

        private const int ShadowHeight = 6;

        private readonly List<TabItem> _tabs = new List<TabItem>();

        private int _childRow = 0;
        private bool _isInitialized = false;

        private BoxView _contentBackgroundView;
        private ShadowBoxView _shadow;

        public TabHostView()
        {
            RowSpacing = 0;
            ColumnSpacing = 0;

            TabItemTappedCommand = new TapCommand(OnTabItemTapped);

            // ChildAdded += OnChildAdded;
            ChildRemoved += OnChildRemoved;
        }

        public event EventHandler<SelectedPositionChangedEventArgs> SelectedTabIndexChanged;

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

        private static void ShadowTypePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var tabHostView = (TabHostView)bindable;

            if (!tabHostView._isInitialized)
            {
                tabHostView.Initialize();
            }
        }


        private void OnTabItemTapped(object tappedItem)
        {
            int selectedIndex = _tabs.IndexOf((TabItem)tappedItem);
            UpdateSelectedIndex(selectedIndex);
            RaiseSelectedTabIndexChanged(new SelectedPositionChangedEventArgs(selectedIndex));
        }

        private void UpdateSelectedIndex(int selectedIndex)
        {
            if (_tabs.Count == 0)
            {
                selectedIndex = 0;
            }

            if (selectedIndex > _tabs.Count)
            {
                selectedIndex = _tabs.Count - 1;
            }

            for (int index = 0; index < _tabs.Count; index++)
            {
                _tabs[index].IsSelected = selectedIndex == index;
            }

            SelectedIndex = selectedIndex;
        }

        private void Initialize()
        {
            _isInitialized = true;

            if (ShadowType == ShadowType.Top)
            {
                Margin = new Thickness(Margin.Left, Margin.Top - ShadowHeight, Margin.Right, Margin.Bottom);

                RowDefinitions.Add(new RowDefinition { Height = ShadowHeight });
                RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

                _shadow = new ShadowBoxView { ShadowType = ShadowType };
                Grid.SetRow(_shadow, 0);

                _childRow = 1;

                _contentBackgroundView = new BoxView { BackgroundColor = BackgroundColor };
                Grid.SetRow(_contentBackgroundView, _childRow);
                BackgroundColor = Color.Transparent;

                foreach (var element in Children)
                {
                    if (element is TabItem tabItem)
                    {
                        Grid.SetRow(tabItem, _childRow);
                    }
                }

                if (ColumnDefinitions.Count > 0)
                {
                    Grid.SetColumnSpan(_shadow, ColumnDefinitions.Count);
                    Grid.SetColumnSpan(_contentBackgroundView, ColumnDefinitions.Count);
                }

                Children.Add(_shadow);
                Children.Add(_contentBackgroundView);
            }
        }

        protected override void OnChildAdded(Element child)
        {
            if (!(child is TabItem tabItem))
            {
                return;
            }

            _tabs.Add(tabItem);

            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            Grid.SetColumn(tabItem, ColumnDefinitions.Count - 1);
            Grid.SetRow(tabItem, _childRow);

            ViewEffect.SetTouchFeedbackColor(tabItem, tabItem.SelectedTabColor);
            TapCommandEffect.SetTap(tabItem, TabItemTappedCommand);
            TapCommandEffect.SetTapParameter(tabItem, tabItem);

            tabItem.Effects.Add(new ViewStyleEffect());
            tabItem.Effects.Add(new TapCommandRoutingEffect());

            if (_shadow != null)
            {
                Grid.SetColumnSpan(_shadow, ColumnDefinitions.Count);
                Grid.SetColumnSpan(_contentBackgroundView, ColumnDefinitions.Count);
            }

            UpdateSelectedIndex(SelectedIndex);
        }

        private void OnChildRemoved(object sender, ElementEventArgs e)
        {
            if (ColumnDefinitions.Count == 0)
            {
                return;
            }

            ColumnDefinitions.RemoveAt(ColumnDefinitions.Count - 1);

            UpdateSelectedIndex(SelectedIndex);
        }

        private void RaiseSelectedTabIndexChanged(SelectedPositionChangedEventArgs e)
        {
            SelectedTabIndexChanged?.Invoke(this, e);
        }
    }
}