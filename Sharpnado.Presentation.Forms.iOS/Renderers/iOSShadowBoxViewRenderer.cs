using CoreAnimation;

using CoreGraphics;

using Sharpnado.Presentation.Forms.Droid.Renderers;
using Sharpnado.Presentation.Forms.RenderedViews;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ShadowBoxView), typeof(iOSShadowBoxViewRenderer))]

namespace Sharpnado.Presentation.Forms.Droid.Renderers
{
    /// <summary>
    /// Renderer to update all frames with better shadows matching material design standards.
    /// </summary>
    public class iOSShadowBoxViewRenderer : VisualElementRenderer<ShadowBoxView>
    {
        private CGSize _previousSize;

        public override void LayoutSubviews()
        {
            if (_previousSize != Bounds.Size)
            {
                SetNeedsDisplay();
            }

            base.LayoutSubviews();
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if (NativeView.Layer.Sublayers != null
                && NativeView.Layer.Sublayers.Length > 0
                && NativeView.Layer.Sublayers[0] is CAGradientLayer)
            {
                return;
            }

            if (Element.ShadowType != ShadowType.Top && Element.ShadowType != ShadowType.Bottom)
            {
                return;
            }

            var gradientsPoints = ComputationHelper.RadiusGradientToPoints(180);

            // Top shadow
            var startColor = Color.FromHex("30000000");
            var endColor = Color.FromHex("10ffffff");
            if (Element.ShadowType == ShadowType.Bottom)
            {
                var tmpColor = startColor;
                startColor = endColor;
                endColor = tmpColor;
            }

            var gradientLayer = new CAGradientLayer
            {
                Frame = rect,
                StartPoint = gradientsPoints.startPoint.ToPointF(),
                EndPoint = gradientsPoints.endPoint.ToPointF(),
                Colors = new[]
                {
                    startColor.ToCGColor(),
                    endColor.ToCGColor(),
                },
            };

            NativeView.Layer.InsertSublayer(gradientLayer, 0);
            _previousSize = Bounds.Size;
        }
    }
}