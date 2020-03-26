using System.ComponentModel;

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

        private CAGradientLayer _gradientLayer;

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

            if (Element.ShadowType == ShadowType.None)
            {
                _gradientLayer?.RemoveFromSuperLayer();
                return;
            }

            if (Element.ShadowType == ShadowType.AcrylicTop)
            {
                _gradientLayer?.RemoveFromSuperLayer();
                NativeView.Layer.BackgroundColor = new CGColor(1, 1, 1);
                return;
            }

            if (NativeView.Layer.Sublayers != null
                && NativeView.Layer.Sublayers.Length > 0
                && NativeView.Layer.Sublayers[0] is CAGradientLayer)
            {
                return;
            }

            var gradientsPoints = ComputationHelper.RadiusGradientToPoints(180);

            // Top shadow
            var startColor = Color.FromHex("30000000");
            var endColor = Color.FromHex("00ffffff");
            if (Element.ShadowType == ShadowType.Bottom)
            {
                var tmpColor = startColor;
                startColor = endColor;
                endColor = tmpColor;
            }

            _gradientLayer = new CAGradientLayer
            {
                Frame = rect,
                StartPoint = gradientsPoints.StartPoint.ToPointF(),
                EndPoint = gradientsPoints.EndPoint.ToPointF(),
                Colors = new[]
                {
                    startColor.ToCGColor(),
                    endColor.ToCGColor(),
                },
            };

            NativeView.Layer.InsertSublayer(_gradientLayer, 0);
            _previousSize = Bounds.Size;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(Element.ShadowType))
            {
                NativeView.Layer.SetNeedsLayout();
            }
        }
    }
}