using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.RenderedViews
{
    /// <summary>
    /// From https://alexdunn.org/2017/05/01/xamarin-tips-making-your-ios-frame-shadows-more-material/
    /// Correct Frame Material shadow implementation for iOS, so that we can use this type of Frame to use automatic shadowing.
    /// </summary>
    public class MaterialFrame : Frame
    {
        public static readonly BindableProperty ElevationProperty = BindableProperty.Create(
            nameof(Elevation),
            typeof(int),
            typeof(MaterialFrame),
            defaultValue: 2);

        public MaterialFrame()
        {
            HasShadow = false;
            CornerRadius = 0;
        }

        public int Elevation
        {
            get => (int)GetValue(ElevationProperty);
            set => SetValue(ElevationProperty, value);
        }
    }
}