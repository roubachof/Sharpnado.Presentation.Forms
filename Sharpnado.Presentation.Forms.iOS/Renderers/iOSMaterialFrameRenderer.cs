using System;
using System.ComponentModel;
using CoreGraphics;
using Sharpnado.Presentation.Forms.iOS.Renderers;
using Sharpnado.Presentation.Forms.RenderedViews;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MaterialFrame), typeof(iOSMaterialFrameRenderer))]
namespace Sharpnado.Presentation.Forms.iOS.Renderers
{
    /// <summary>
    /// Renderer to update all frames with better shadows matching material design standards.
    /// </summary>
    public class iOSMaterialFrameRenderer : FrameRenderer
    {
        private MaterialFrame MaterialFrame => (MaterialFrame)Element;

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            float elevation = MaterialFrame.Elevation;
            if (elevation == 0)
            {
                return;
            }

            // Update shadow to match better material design standards of elevation
            Layer.ShadowColor = UIColor.Black.CGColor;
            Layer.ShadowRadius = Math.Abs(elevation);
            Layer.ShadowOffset = new CGSize(0, elevation);
            Layer.ShadowOpacity = 0.24f;
            Layer.ShadowPath = UIBezierPath.FromRect(Layer.Bounds).CGPath;
            Layer.MasksToBounds = false;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(MaterialFrame.Elevation))
            {
                UpdateElevation();
            }
        }

        private void UpdateElevation()
        {
            System.Diagnostics.Debug.WriteLine($">>>>> UpdateElevation( elevation: {MaterialFrame.Elevation} )");
            SetNeedsDisplay();
        }
    }
}