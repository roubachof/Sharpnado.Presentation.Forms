using System;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.RenderedViews
{
    /// <summary>
    /// From https://alexdunn.org/2017/05/01/xamarin-tips-making-your-ios-frame-shadows-more-material/
    /// Correct Frame Material shadow implementation for iOS, so that we can use this type of Frame to use automatic shadowing.
    /// </summary>
    public class MaterialFrame : Frame
    {
        public const int AcrylicElevation = 20;

        public static readonly BindableProperty MaterialThemeProperty = BindableProperty.Create(
            nameof(MaterialTheme),
            typeof(Theme),
            typeof(MaterialFrame),
            defaultValue: Theme.Light);

        public static readonly BindableProperty LightThemeBackgroundColorProperty = BindableProperty.Create(
            nameof(LightThemeBackgroundColor),
            typeof(Color),
            typeof(MaterialFrame),
            defaultValue: Color.White);

        public static readonly BindableProperty ElevationProperty = BindableProperty.Create(
            nameof(Elevation),
            typeof(int),
            typeof(MaterialFrame),
            defaultValue: 2);

        // https://material.io/design/color/dark-theme.html#properties
        private static readonly Color[] DarkColors = new[]
        {
            Color.FromHex("121212"), // 00dp
            Color.FromHex("1D1D1D"),
            Color.FromHex("212121"),
            Color.FromHex("242424"),
            Color.FromHex("272727"), // 04dp
            Color.FromHex("272727"),
            Color.FromHex("2C2C2C"), // 06dp
            Color.FromHex("2C2C2C"),
            Color.FromHex("2D2D2D"), // 08dp
            Color.FromHex("2D2D2D"),
            Color.FromHex("2D2D2D"),
            Color.FromHex("2D2D2D"),
            Color.FromHex("323232"), // 12dp
            Color.FromHex("323232"),
            Color.FromHex("323232"),
            Color.FromHex("323232"),
            Color.FromHex("353535"), // 16dp
            Color.FromHex("353535"),
            Color.FromHex("353535"),
            Color.FromHex("353535"),
            Color.FromHex("353535"),
            Color.FromHex("353535"),
            Color.FromHex("353535"),
            Color.FromHex("353535"),
            Color.FromHex("373737"), // 24dp
        };

        private static Theme globalTheme;

        public MaterialFrame()
        {
            HasShadow = false;
            CornerRadius = 5;
            ThemeChanged += OnThemeChanged;

            OnThemeChanged(this, null);
        }

        public static event EventHandler ThemeChanged;

        public enum Theme
        {
            Light = 0,
            Dark,
            Acrylic,
        }

        public Theme MaterialTheme
        {
            get => (Theme)GetValue(MaterialThemeProperty);
            set => SetValue(MaterialThemeProperty, value);
        }

        public Color LightThemeBackgroundColor
        {
            get => (Color)GetValue(LightThemeBackgroundColorProperty);
            set => SetValue(LightThemeBackgroundColorProperty, value);
        }

        public int Elevation
        {
            get => (int)GetValue(ElevationProperty);
            set => SetValue(ElevationProperty, value);
        }

        public static void ChangeGlobalTheme(Theme newTheme)
        {
            var previousTheme = globalTheme;
            globalTheme = newTheme;

            if (previousTheme != globalTheme)
            {
                ThemeChanged?.Invoke(null, new EventArgs());
            }
        }

        public void Unsubscribe()
        {
            ThemeChanged -= OnThemeChanged;
        }

        public Color ElevationToColor()
        {
            if (MaterialTheme == Theme.Light)
            {
                return Color.Default;
            }

            if (Elevation < 0)
            {
                return Color.Default;
            }

            int index = Elevation > 24 ? 24 : Elevation;
            return DarkColors[index];
        }

        private void OnThemeChanged(object sender, EventArgs eventArgs)
        {
            MaterialTheme = globalTheme;
        }
    }
}