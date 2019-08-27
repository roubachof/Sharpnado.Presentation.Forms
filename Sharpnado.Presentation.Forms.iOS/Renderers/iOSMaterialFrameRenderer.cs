using System;
using System.ComponentModel;
using System.Drawing;

using CoreAnimation;

using CoreGraphics;

using Foundation;

using Sharpnado.Presentation.Forms.iOS.Renderers;
using Sharpnado.Presentation.Forms.RenderedViews;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Color = Xamarin.Forms.Color;

[assembly: ExportRenderer(typeof(MaterialFrame), typeof(iOSMaterialFrameRenderer))]

namespace Sharpnado.Presentation.Forms.iOS.Renderers
{
    /// <summary>
    ///     Renderer to update all frames with better shadows matching material design standards.
    /// </summary>
    [Preserve]
    public class iOSMaterialFrameRenderer : VisualElementRenderer<MaterialFrame>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<MaterialFrame> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null)
            {
                return;
            }

            SetupLayer();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName != VisualElement.BackgroundColorProperty.PropertyName
                && e.PropertyName != Xamarin.Forms.Frame.HasShadowProperty.PropertyName
                && e.PropertyName != Xamarin.Forms.Frame.CornerRadiusProperty.PropertyName
                && e.PropertyName != MaterialFrame.ElevationProperty.PropertyName)
            {
                return;
            }

            SetupLayer();
        }

        private void SetupLayer()
        {
            float num = Element.CornerRadius;
            if (num == -1.0)
            {
                num = 5f;
            }

            Layer.CornerRadius = num;
            Layer.BackgroundColor = Element.BackgroundColor == Color.Default
                ? UIColor.White.CGColor
                : Element.BackgroundColor.ToCGColor();

            if (Element.HasShadow)
            {
                Layer.ShadowRadius = 5;
                Layer.ShadowColor = UIColor.Black.CGColor;
                Layer.ShadowOpacity = 0.8f;
                Layer.ShadowOffset = new SizeF();
            }
            else
            {
                Layer.ShadowOpacity = 0.0f;
            }

            if (Element.Elevation > 0)
            {
                float adaptedElevation = Element.Elevation / 2;

                Layer.ShadowColor = UIColor.Black.CGColor;
                Layer.ShadowRadius = Math.Abs(adaptedElevation);
                Layer.ShadowOffset = new CGSize(0, adaptedElevation);
                Layer.ShadowOpacity = 0.24f;
                // Layer.ShadowPath = UIBezierPath.FromRect(Layer.Bounds).CGPath;
                Layer.MasksToBounds = false;
            }
            else
            {
                Layer.ShadowOpacity = 0.0f;
            }

            Layer.RasterizationScale = UIScreen.MainScreen.Scale;
            Layer.ShouldRasterize = true;
        }
    }
}