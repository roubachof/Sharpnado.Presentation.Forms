using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using CoreAnimation;

using CoreGraphics;

using Foundation;

using Sharpnado.Presentation.Forms.iOS.Renderers;
using Sharpnado.Presentation.Forms.RenderedViews;
using Sharpnado.Tasks;

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

        private UIVisualEffectView _blurView;

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

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Bounds.Width > 0)
            {
                UpdateBlurViewBounds();
            }
        }

        private void UpdateLayerBounds()
        {
            _intermediateLayer.Frame = new CGRect(0, 2, Bounds.Width, Bounds.Height - 2);
        }

        private void UpdateBlurViewBounds()
        {
            if (_blurView != null)
            {
                _blurView.Frame = new CGRect(0, 0, Bounds.Width, Bounds.Height);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _intermediateLayer?.RemoveFromSuperLayer();
                _intermediateLayer?.Dispose();
                _intermediateLayer = null;

                _blurView?.RemoveFromSuperview();
                _blurView?.Dispose();
                _blurView = null;
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

                case nameof(MaterialFrame.AcrylicGlowColor):
                    UpdateAcrylicGlowColor();
                    break;

                case nameof(MaterialFrame.MaterialTheme):
                    UpdateMaterialTheme();
                    break;

                case nameof(MaterialFrame.MaterialBlurStyle):
                    UpdateMaterialBlurStyle();
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

                case MaterialFrame.Theme.AcrylicBlur:
                case MaterialFrame.Theme.Dark:
                    return;

                case MaterialFrame.Theme.Light:
                    Layer.BackgroundColor = Element.LightThemeBackgroundColor.ToCGColor();
                    break;
            }
        }

        private void UpdateAcrylicGlowColor()
        {
            if (Element.MaterialTheme != MaterialFrame.Theme.Acrylic)
            {
                return;
            }

            Layer.BackgroundColor = Element.AcrylicGlowColor.ToCGColor();
        }

        private void UpdateElevation()
        {
            if (Element.MaterialTheme == MaterialFrame.Theme.AcrylicBlur)
            {
                Layer.ShadowOpacity = 0.0f;
                return;
            }

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

            if (_blurView != null)
            {
                _blurView.Layer.CornerRadius = radius;
            }
        }

        private void UpdateMaterialTheme()
        {
            switch (Element.MaterialTheme)
            {
                case MaterialFrame.Theme.Acrylic:
                    SetAcrylicTheme();
                    break;

                case MaterialFrame.Theme.AcrylicBlur:
                    SetAcrylicBlurTheme();
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

            DisableBlur();
        }

        private void SetLightTheme()
        {
            _intermediateLayer.BackgroundColor = Color.Transparent.ToCGColor();

            Layer.BackgroundColor = Element.LightThemeBackgroundColor.ToCGColor();

            UpdateCornerRadius();
            UpdateElevation();

            DisableBlur();
        }

        private void SetAcrylicTheme()
        {
            _intermediateLayer.BackgroundColor = Element.LightThemeBackgroundColor.ToCGColor();

            UpdateAcrylicGlowColor();

            UpdateCornerRadius();
            UpdateElevation();

            DisableBlur();

            SetNeedsDisplay();
        }

        private void SetAcrylicBlurTheme()
        {
            _intermediateLayer.BackgroundColor = Color.Transparent.ToCGColor();
            Layer.BackgroundColor = Color.Transparent.ToCGColor();

            EnableBlur();

            UpdateCornerRadius();
            UpdateElevation();

            LayoutSubviews();
        }

        private void UpdateMaterialBlurStyle()
        {
            if (_blurView != null)
            {
                _blurView.Effect = UIBlurEffect.FromStyle(ConvertBlurStyle());
            }
        }

        private void EnableBlur()
        {
            if (_blurView == null)
            {
                _blurView = new UIVisualEffectView() { ClipsToBounds = true, BackgroundColor = UIColor.Clear };
            }

            UpdateMaterialBlurStyle();

            if (Subviews.Length > 0 && ReferenceEquals(Subviews[0], _blurView))
            {
                return;
            }

            _blurView.Frame = new CGRect(0, 0, Bounds.Width, Bounds.Height);
            InsertSubview(_blurView, 0);
        }

        private void DisableBlur()
        {
            _blurView?.RemoveFromSuperview();
        }

        private UIBlurEffectStyle ConvertBlurStyle()
        {
            switch (Element.MaterialBlurStyle)
            {
                case MaterialFrame.BlurStyle.ExtraLight:
                    return UIBlurEffectStyle.ExtraLight;
                case MaterialFrame.BlurStyle.Dark:
                    return UIBlurEffectStyle.Dark;

                default:
                    return UIBlurEffectStyle.Light;
            }
        }
    }
}