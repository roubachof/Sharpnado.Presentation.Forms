using System.Linq;
using Foundation;
using Sharpnado.Presentation.Forms.Effects;
using Sharpnado.Presentation.Forms.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("Silly")]
[assembly: ExportEffect(typeof(iOSTintableImageEffect), nameof(TintableImageEffect))]

namespace Sharpnado.Presentation.Forms.iOS.Effects
{
    [Preserve]
    public class iOSTintableImageEffect : PlatformEffect
    {
        public static void Initialize()
        {
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if ((Element is Image) && args.PropertyName == Image.SourceProperty.PropertyName)
            {
                UpdateColor();
            }
        }

        protected override void OnAttached()
        {
            UpdateColor();
        }

        protected override void OnDetached()
        {
            if (Control is UIImageView imageView && imageView.Image != null)
            {
                imageView.Image = imageView.Image.ImageWithRenderingMode(UIImageRenderingMode.Automatic);
            }
        }

        private void UpdateColor()
        {
            var imageView = (UIImageView)Control;
            if (imageView?.Image == null)
            {
                return;
            }

            var effect =
                (TintableImageEffect)Element.Effects.FirstOrDefault(
                    x => x is TintableImageEffect);

            var color = effect?.TintColor.ToUIColor();
            if (color != null)
            {
                imageView.Image = imageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                Control.TintColor = color;
            }
        }
    }
}
