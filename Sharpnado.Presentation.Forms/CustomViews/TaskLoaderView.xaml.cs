using Sharpnado.Presentation.Forms.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using BindableObject = Xamarin.Forms.BindableObject;

namespace Sharpnado.Presentation.Forms.CustomViews
{
    [ContentProperty("Child")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskLoaderView : ContentView
    {
        public static readonly BindableProperty ViewModelLoaderProperty = BindableProperty.Create(
            nameof(ViewModelLoader),
            typeof(IViewModelLoader),
            typeof(TaskLoaderView),
            propertyChanged: ViewModelLoaderChanged);

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            nameof(FontFamily),
            typeof(string),
            typeof(TaskLoaderView),
            null,
            BindingMode.OneWay,
            propertyChanged: FontFamilyChanged);

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(TaskLoaderView),
            defaultValue: Color.Black,
            propertyChanged: TextColorChanged);

        public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
            nameof(AccentColor),
            typeof(Color),
            typeof(TaskLoaderView),
            defaultValue: Color.Accent,
            propertyChanged: AccentColorChanged);

        public static readonly BindableProperty RetryButtonTextProperty = BindableProperty.Create(
            nameof(RetryButtonText),
            typeof(string),
            typeof(TaskLoaderView),
            propertyChanged: RetryButtonTextChanged);

        public static readonly BindableProperty EmptyStateImageUrlProperty = BindableProperty.Create(
            nameof(EmptyStateImageUrl),
            typeof(string),
            typeof(TaskLoaderView),
            propertyChanged: EmptyStateImageUrlChanged);

        public static readonly BindableProperty ErrorImageConverterProperty = BindableProperty.Create(
            nameof(ErrorImageConverter),
            typeof(IValueConverter),
            typeof(TaskLoaderView));

        public TaskLoaderView()
        {
            InitializeComponent();
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public Color AccentColor
        {
            get => (Color)GetValue(AccentColorProperty);
            set => SetValue(AccentColorProperty, value);
        }

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public string RetryButtonText
        {
            get => (string)GetValue(RetryButtonTextProperty);
            set => SetValue(RetryButtonTextProperty, value);
        }

        public string EmptyStateImageUrl
        {
            get => (string)GetValue(EmptyStateImageUrlProperty);
            set => SetValue(EmptyStateImageUrlProperty, value);
        }

        public IValueConverter ErrorImageConverter
        {
            get => (IValueConverter)GetValue(ErrorImageConverterProperty);
            set => SetValue(ErrorImageConverterProperty, value);
        }

        public IViewModelLoader ViewModelLoader
        {
            get => (IViewModelLoader)GetValue(ViewModelLoaderProperty);
            set => SetValue(ViewModelLoaderProperty, value);
        }

        public View Child
        {
            get => ContainerView.Content;
            set
            {
                if (Grid.Children.Contains(value))
                {
                    return;
                }

                ContainerView.Content = value;
            }
        }

        private static void FontFamilyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var taskLoader = (TaskLoaderView)bindable;
            var fontFamily = (string)newvalue;

            taskLoader.ErrorNotificationViewLabel.FontFamily = fontFamily;
            taskLoader.ErrorViewLabel.FontFamily = fontFamily;
            taskLoader.EmptyStateLabel.FontFamily = fontFamily;
        }

        private static void TextColorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            ((TaskLoaderView)bindable).ErrorViewLabel.TextColor = (Color)newvalue;
            ((TaskLoaderView)bindable).EmptyStateLabel.TextColor = (Color)newvalue;
        }

        private static void AccentColorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var taskLoader = (TaskLoaderView)bindable;
            taskLoader.Loader.Color = (Color)newvalue;
            taskLoader.ErrorViewButton.BackgroundColor = (Color)newvalue;
        }

        private static void RetryButtonTextChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            ((TaskLoaderView)bindable).ErrorViewButton.Text = (string)newvalue;
        }

        private static void EmptyStateImageUrlChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var emptyStateImage = ((TaskLoaderView)bindable).EmptyStateImage;
            if (newvalue == null || string.IsNullOrWhiteSpace((string)newvalue))
            {
                return;
            }

            emptyStateImage.Source = (string)newvalue;
        }

        private static void ViewModelLoaderChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var taskLoader = (TaskLoaderView)bindable;
            taskLoader.SetBindings();
        }

        private void SetBindings()
        {
            if (ViewModelLoader != null)
            {
                ContainerView.SetBinding(
                    ContentView.IsVisibleProperty,
                    new Binding(nameof(ViewModelLoader.ShowResult), source: ViewModelLoader));

                Loader.SetBinding(
                    ActivityIndicator.IsRunningProperty,
                    new Binding(nameof(ViewModelLoader.ShowLoader), source: ViewModelLoader));

                ErrorView.SetBinding(
                    StackLayout.IsVisibleProperty,
                    new Binding(nameof(ViewModelLoader.ShowError), source: ViewModelLoader));

                ErrorViewImage.IsVisible = ErrorImageConverter != null;
                if (ErrorViewImage.IsVisible)
                {
                    ErrorViewImage.SetBinding(
                        Image.SourceProperty,
                        new Binding(
                            nameof(ViewModelLoader.Error),
                            source: ViewModelLoader,
                            converter: ErrorImageConverter));
                }

                ErrorViewLabel.SetBinding(
                    Label.TextProperty,
                    new Binding(nameof(ViewModelLoader.ErrorMessage), source: ViewModelLoader));

                ErrorViewButton.SetBinding(
                    Button.CommandProperty,
                    new Binding(nameof(ViewModelLoader.ReloadCommand), source: ViewModelLoader));

                ErrorNotificationView.SetBinding(
                    Frame.IsVisibleProperty,
                    new Binding(nameof(ViewModelLoader.ShowErrorNotification), source: ViewModelLoader));

                ErrorNotificationViewLabel.SetBinding(
                    Label.TextProperty,
                    new Binding(nameof(ViewModelLoader.ErrorMessage), source: ViewModelLoader));

                EmptyStateView.SetBinding(
                    Label.IsVisibleProperty,
                    new Binding(nameof(ViewModelLoader.ShowEmptyState), source: ViewModelLoader));

                EmptyStateLabel.SetBinding(
                    Label.TextProperty,
                    new Binding(nameof(ViewModelLoader.EmptyStateMessage), source: ViewModelLoader));

                EmptyStateImage.IsVisible = !string.IsNullOrWhiteSpace(EmptyStateImageUrl);
                if (EmptyStateImage.IsVisible)
                {
                    EmptyStateImage.Source = EmptyStateImageUrl;
                }
            }
        }
    }
}