using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.RenderedViews
{
    public enum ShadowType
    {
        None = 0,
        Top,
        Bottom,
    }

    public class ShadowBoxView : BoxView
    {
        public static readonly BindableProperty ShadowTypeProperty = BindableProperty.Create(
            nameof(ShadowType),
            typeof(ShadowType),
            typeof(ShadowBoxView));

        public ShadowType ShadowType
        {
            get => (ShadowType)GetValue(ShadowTypeProperty);
            set => SetValue(ShadowTypeProperty, value);
        }
    }
}
