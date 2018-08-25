using System;
using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.CustomViews
{
    public interface ILazyView
    {
        View Content { get; }

        bool IsLoaded { get; }

        void LoadView();
    }

    public class LazyView<TView> : ContentView, ILazyView, IDisposable
        where TView : View, new()
    {
        public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
            nameof(AccentColor),
            typeof(Color),
            typeof(ILazyView),
            Color.Accent,
            propertyChanged: AccentColorChanged);

        public LazyView()
        {
            Content = new ActivityIndicator
            {
                Color = AccentColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsRunning = true,
            };
        }

        public Color AccentColor
        {
            get => (Color)GetValue(AccentColorProperty);
            set => SetValue(AccentColorProperty, value);
        }

        public bool IsLoaded { get; private set; }

        public void LoadView()
        {
            IsLoaded = true;

            View view = new TView { BindingContext = BindingContext };

            Content = view;
        }

        public void Dispose()
        {
            if (Content is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private static void AccentColorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var lazyView = (ILazyView)bindable;
            if (lazyView.Content is ActivityIndicator activityIndicator)
            {
                activityIndicator.Color = (Color)newvalue;
            }
        }
    }
}