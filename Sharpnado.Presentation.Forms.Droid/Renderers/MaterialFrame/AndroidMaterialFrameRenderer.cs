using System.ComponentModel;

using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

#if __ANDROID_29__
using AndroidX.Core.View;
#else
using Android.Support.V4.View;
#endif

using Sharpnado.Presentation.Forms.Droid.Helpers;
using Sharpnado.Presentation.Forms.Droid.Renderers.MaterialFrame;
using Sharpnado.Presentation.Forms.RenderedViews;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using FrameRenderer = Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer;

[assembly: ExportRenderer(typeof(MaterialFrame), typeof(AndroidMaterialFrameRenderer))]

namespace Sharpnado.Presentation.Forms.Droid.Renderers.MaterialFrame
{
    /// <summary>
    /// Renderer to update all frames with better shadows matching material design standards.
    /// </summary>
    public partial class AndroidMaterialFrameRenderer : FrameRenderer
    {
        private GradientDrawable _mainDrawable;

        private GradientDrawable _acrylicLayer;

        public AndroidMaterialFrameRenderer(Context context)
            : base(context)
        {
        }

        private RenderedViews.MaterialFrame MaterialFrame => (RenderedViews.MaterialFrame)Element;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MaterialFrame.CornerRadius):
                    UpdateCornerRadius();
                    base.OnElementPropertyChanged(sender, e);
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

                default:
                    base.OnElementPropertyChanged(sender, e);
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _mainDrawable = null;

                _acrylicLayer?.Dispose();
                _acrylicLayer = null;
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            ((RenderedViews.MaterialFrame)e.OldElement)?.Unsubscribe();
            DestroyBlur();

            if (e.NewElement == null)
            {
                return;
            }

            _mainDrawable = (GradientDrawable)Background;

            UpdateMaterialTheme();
        }

        private void UpdateCornerRadius()
        {
            _acrylicLayer?.SetCornerRadius(Context.ToPixels(MaterialFrame.CornerRadius));
        }

        private void UpdateElevation()
        {
            if (MaterialFrame.MaterialTheme == RenderedViews.MaterialFrame.Theme.Dark || MaterialFrame.MaterialTheme == RenderedViews.MaterialFrame.Theme.AcrylicBlur)
            {
                ViewCompat.SetElevation(this, 0);
                return;
            }

            bool isAcrylicTheme = MaterialFrame.MaterialTheme == RenderedViews.MaterialFrame.Theme.Acrylic;

            // we need to reset the StateListAnimator to override the setting of Elevation on touch down and release.
            StateListAnimator = new Android.Animation.StateListAnimator();

            // set the elevation manually
            ViewCompat.SetElevation(this, isAcrylicTheme ? RenderedViews.MaterialFrame.AcrylicElevation : MaterialFrame.Elevation);
        }

        private void UpdateLightThemeBackgroundColor()
        {
            if (MaterialFrame.MaterialTheme == RenderedViews.MaterialFrame.Theme.Dark)
            {
                return;
            }

            _mainDrawable.SetColor(MaterialFrame.LightThemeBackgroundColor.ToAndroid());
        }

        private void UpdateMaterialTheme()
        {
            switch (MaterialFrame.MaterialTheme)
            {
                case RenderedViews.MaterialFrame.Theme.Acrylic:
                    SetAcrylicTheme();
                    break;

                case RenderedViews.MaterialFrame.Theme.Dark:
                    SetDarkTheme();
                    break;

                case RenderedViews.MaterialFrame.Theme.Light:
                    SetLightTheme();
                    break;

                case RenderedViews.MaterialFrame.Theme.AcrylicBlur:
                    SetAcrylicBlurTheme();
                    break;
            }
        }

        private void SetAcrylicBlurTheme()
        {
            _mainDrawable.SetColor(Color.Transparent.ToAndroid());

            this.SetBackground(_mainDrawable);

            UpdateElevation();

            EnableBlur();
        }

        private void SetDarkTheme()
        {
            DisableBlur();

            _mainDrawable.SetColor(MaterialFrame.ElevationToColor().ToAndroid());

            this.SetBackground(_mainDrawable);

            UpdateElevation();
        }

        private void SetLightTheme()
        {
            DisableBlur();

            _mainDrawable.SetColor(MaterialFrame.LightThemeBackgroundColor.ToAndroid());

            this.SetBackground(_mainDrawable);

            UpdateElevation();
        }

        private void SetAcrylicTheme()
        {
            if (_acrylicLayer == null)
            {
                _acrylicLayer = new GradientDrawable();
                _acrylicLayer.SetShape(ShapeType.Rectangle);
            }

            _acrylicLayer.SetColor(Android.Graphics.Color.White);
            _acrylicLayer.SetCornerRadius(_mainDrawable.CornerRadius);

            _mainDrawable.SetColor(MaterialFrame.LightThemeBackgroundColor.ToAndroid());

            LayerDrawable layer = new LayerDrawable(new Drawable[] { _acrylicLayer, _mainDrawable });
            layer.SetLayerInsetTop(1, (int)Context.ToPixels(2));

            this.SetBackground(layer);

            UpdateElevation();
        }
    }
}