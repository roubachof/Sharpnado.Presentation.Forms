using System.ComponentModel;
using Android.Content;
using Android.Support.V4.View;
using Sharpnado.Presentation.Forms.Droid.Renderers;
using Sharpnado.Presentation.Forms.RenderedViews;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using FrameRenderer = Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer;

[assembly: ExportRenderer(typeof(MaterialFrame), typeof(AndroidMaterialFrameRenderer))]
namespace Sharpnado.Presentation.Forms.Droid.Renderers
{
    /// <summary>
    /// Renderer to update all frames with better shadows matching material design standards.
    /// </summary>
    public class AndroidMaterialFrameRenderer : FrameRenderer
    {
        public AndroidMaterialFrameRenderer(Context context)
            : base(context)
        {
        }

        private MaterialFrame MaterialFrame => (MaterialFrame)Element;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(MaterialFrame.Elevation)
                || e.PropertyName == nameof(MaterialFrame.MaterialTheme)
                || (e.PropertyName == nameof(MaterialFrame.LightThemeBackgroundColor)
                    && MaterialFrame.MaterialTheme == MaterialFrame.Theme.Light))
            {
                UpdateElevation();
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            ((MaterialFrame)e.OldElement)?.Unsubscribe();

            if (e.NewElement == null)
            {
                return;
            }

            UpdateElevation();
        }

        private void UpdateElevation()
        {
            if (MaterialFrame.MaterialTheme == MaterialFrame.Theme.Dark)
            {
                MaterialFrame.BackgroundColor = MaterialFrame.ElevationToColor();
                ViewCompat.SetElevation(this, 0);
                ViewCompat.SetElevation(Control, 0);

                base.OnElementPropertyChanged(this, new PropertyChangedEventArgs(VisualElement.BackgroundColorProperty.PropertyName));
                return;
            }

            // we need to reset the StateListAnimator to override the setting of Elevation on touch down and release.
            Control.StateListAnimator = new Android.Animation.StateListAnimator();

            MaterialFrame.BackgroundColor = MaterialFrame.LightThemeBackgroundColor;

            // set the elevation manually
            ViewCompat.SetElevation(this, MaterialFrame.Elevation);
            ViewCompat.SetElevation(Control, MaterialFrame.Elevation);

            base.OnElementPropertyChanged(this, new PropertyChangedEventArgs(VisualElement.BackgroundColorProperty.PropertyName));
        }
    }
}