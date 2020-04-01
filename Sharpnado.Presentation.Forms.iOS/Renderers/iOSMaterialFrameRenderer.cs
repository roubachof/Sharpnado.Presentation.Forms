using System;
using System.ComponentModel;

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
        private CALayer _intermediateLayer;

        public override void LayoutSublayersOfLayer(CALayer layer)
        {
            base.LayoutSublayersOfLayer(layer);

            if (Layer.Bounds.Width > 0)
            {
                UpdateLayerBounds();

                if (Layer.ShadowRadius > 0)
                {
                    Layer.ShadowPath = UIBezierPath.FromRect(Layer.Bounds).CGPath;
                }
            }
        }

        private void UpdateLayerBounds()
        {
            _intermediateLayer.Frame = new CGRect(0, 2, Bounds.Width, Bounds.Height - 2);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _intermediateLayer?.RemoveFromSuperLayer();
                _intermediateLayer?.Dispose();
                _intermediateLayer = null;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<MaterialFrame> e)
        {
            base.OnElementChanged(e);

            e.OldElement?.Unsubscribe();

            if (e.NewElement == null)
            {
                return;
            }

            _intermediateLayer = new CALayer { BackgroundColor = Color.Transparent.ToCGColor() };

            Layer.InsertSublayer(_intermediateLayer, 0);

            UpdateMaterialTheme();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MaterialFrame.CornerRadius):
                    UpdateCornerRadius();
                    break;

                case nameof(MaterialFrame.Elevation):
                    UpdateElevation();
                    break;

                case nameof(MaterialFrame.LightThemeBackgroundColor):
                    UpdateLightThemeBackgroundColor();
                    break;

                case nameof(MaterialFrame.MaterialTheme):
                    UpdateMaterialTheme();
                    break;
            }
        }

        private void UpdateLightThemeBackgroundColor()
        {
            switch (Element.MaterialTheme)
            {
                case MaterialFrame.Theme.Acrylic:
                    _intermediateLayer.BackgroundColor = Element.LightThemeBackgroundColor.ToCGColor();
                    break;

                case MaterialFrame.Theme.Dark:
                    return;

                case MaterialFrame.Theme.Light:
                    Layer.BackgroundColor = Element.LightThemeBackgroundColor.ToCGColor();
                    break;
            }
        }

        private void UpdateElevation()
        {
            if (Element.MaterialTheme == MaterialFrame.Theme.Dark)
            {
                Layer.ShadowOpacity = 0.0f;
                Layer.BackgroundColor = Element.ElevationToColor().ToCGColor();
                return;
            }

            bool isAcrylicTheme = Element.MaterialTheme == MaterialFrame.Theme.Acrylic;

            float adaptedElevation = isAcrylicTheme ? MaterialFrame.AcrylicElevation / 3 : Element.Elevation / 2;
            float opacity = isAcrylicTheme ? 0.12f : 0.24f;

            Layer.ShadowColor = UIColor.Black.CGColor;
            Layer.ShadowRadius = Math.Abs(adaptedElevation);
            Layer.ShadowOffset = new CGSize(0, adaptedElevation);
            Layer.ShadowOpacity = opacity;

            Layer.MasksToBounds = false;

            Layer.RasterizationScale = UIScreen.MainScreen.Scale;
            Layer.ShouldRasterize = true;
        }

        private void UpdateCornerRadius()
        {
            float radius = Element.CornerRadius;
            if (radius == -1.0f)
            {
                radius = 5f;
            }

            Layer.CornerRadius = radius;
            _intermediateLayer.CornerRadius = radius;
        }

        private void UpdateMaterialTheme()
        {
            switch (Element.MaterialTheme)
            {
                case MaterialFrame.Theme.Acrylic:
                    SetAcrylicTheme();
                    break;

                case MaterialFrame.Theme.Dark:
                    SetDarkTheme();
                    break;

                case MaterialFrame.Theme.Light:
                    SetLightTheme();
                    break;
            }
        }

        private void SetDarkTheme()
        {
            _intermediateLayer.BackgroundColor = Color.Transparent.ToCGColor();

            Layer.BackgroundColor = Element.ElevationToColor().ToCGColor();

            UpdateCornerRadius();
            UpdateElevation();
        }

        private void SetLightTheme()
        {
            _intermediateLayer.BackgroundColor = Color.Transparent.ToCGColor();

            Layer.BackgroundColor = Element.LightThemeBackgroundColor.ToCGColor();

            UpdateCornerRadius();
            UpdateElevation();
        }

        private void SetAcrylicTheme()
        {
            _intermediateLayer.BackgroundColor = Element.LightThemeBackgroundColor.ToCGColor();

            Layer.BackgroundColor = UIColor.White.CGColor;

            UpdateCornerRadius();
            UpdateElevation();

            LayoutIfNeeded();
        }
    }
}