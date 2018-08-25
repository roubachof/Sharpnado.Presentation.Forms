using System;
using System.Windows.Input;
using Sharpnado.Presentation.Forms.Commands;
using Sharpnado.Presentation.Forms.Effects;
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

        public TabHostView()
        {
            RowSpacing = 0;
            ColumnSpacing = 0;

            TabItemTappedCommand = new TapCommand(OnTabItemTapped);

            ChildAdded += OnChildAdded;
            ChildRemoved += OnChildRemoved;
        }

        public event EventHandler<SelectedPositionChangedEventArgs> SelectedTabIndexChanged;

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
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

        private void OnTabItemTapped(object tappedItem)
        {
            int selectedIndex = Children.IndexOf((TabItem)tappedItem);
            UpdateSelectedIndex(selectedIndex);
            RaiseSelectedTabIndexChanged(new SelectedPositionChangedEventArgs(selectedIndex));
        }

        private void UpdateSelectedIndex(int selectedIndex)
        {
            if (Children.Count == 0)
            {
                selectedIndex = 0;
            }

            if (selectedIndex > Children.Count)
            {
                selectedIndex = Children.Count - 1;
            }

            for (int index = 0; index < Children.Count; index++)
            {
                ((TabItem)Children[index]).IsSelected = selectedIndex == index;
            }

            SelectedIndex = selectedIndex;
        }

        private void OnChildAdded(object sender, ElementEventArgs e)
        {
            if (!(e.Element is TabItem tabItem))
            {
                throw new InvalidOperationException("TabHostView only support children of type TabItem");
            }

            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            Grid.SetColumn(tabItem, ColumnDefinitions.Count - 1);

            ViewEffect.SetTouchFeedbackColor(tabItem, tabItem.SelectedTabColor);
            TapCommandEffect.SetTap(tabItem, TabItemTappedCommand);
            TapCommandEffect.SetTapParameter(tabItem, tabItem);

            tabItem.Effects.Add(new ViewStyleEffect());
            tabItem.Effects.Add(new TapCommandRoutingEffect());

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