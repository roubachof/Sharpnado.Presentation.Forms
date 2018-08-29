using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Sharpnado.Infrastructure.Tasks;
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
    [Preserve]
    public class iOSMaterialFrameRenderer : FrameRenderer
    {
        private bool _isDisposed;

        private MaterialFrame MaterialFrame => (MaterialFrame)Element;

        public static void Initialize()
        {
        }

        public static void AddShadow(CALayer layer, float elevation)
        {
            layer.ShadowColor = UIColor.Black.CGColor;
            layer.ShadowRadius = Math.Abs(elevation);
            layer.ShadowOffset = new CGSize(0, elevation);
            layer.ShadowOpacity = 0.24f;
            layer.ShadowPath = UIBezierPath.FromRect(layer.Bounds).CGPath;
            layer.MasksToBounds = false;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            float elevation = MaterialFrame.Elevation / 2f;
            if (elevation == 0)
            {
                return;
            }

            // Update shadow to match better material design standards of elevation
            AddShadow(Layer, elevation);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _isDisposed = true;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                NotifyTask.Create(async () =>
                {
                    await Task.Delay(100);
                    if (Element == null || _isDisposed)
                    {
                        return;
                    }

                    SetNeedsDisplay();
                    LayoutIfNeeded();
                });
            }
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
            LayoutIfNeeded();
        }
    }
}