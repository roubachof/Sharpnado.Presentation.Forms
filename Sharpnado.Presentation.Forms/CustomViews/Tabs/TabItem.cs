using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.CustomViews.Tabs
{
    public abstract class TabItem : ContentView
    {
        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
            nameof(IsSelected),
            typeof(bool),
            typeof(TabItem),
            false);

        public static readonly BindableProperty LabelProperty = BindableProperty.Create(
            nameof(Label),
            typeof(string),
            typeof(TabItem),
            string.Empty);

        public static readonly BindableProperty SelectedTabColorProperty = BindableProperty.Create(
            nameof(SelectedTabColor),
            typeof(Color),
            typeof(UnderlinedTabItem));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public Color SelectedTabColor
        {
            get => (Color)GetValue(SelectedTabColorProperty);
            set => SetValue(SelectedTabColorProperty, value);
        }
    }
}
