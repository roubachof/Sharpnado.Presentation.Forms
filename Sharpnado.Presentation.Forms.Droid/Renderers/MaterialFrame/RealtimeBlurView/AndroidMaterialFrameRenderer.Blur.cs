using System;
using Android.Views;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Color = Xamarin.Forms.Color;
using View = Android.Views.View;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.MaterialFrame
{
    public partial class AndroidMaterialFrameRenderer
    {
        private const double StyledBlurRadius = 64;

        private static readonly Color DarkBlurOverlayColor = Color.FromHex("#80000000");

        private static readonly Color LightBlurOverlayColor = Color.FromHex("#40FFFFFF");

        private static readonly Color ExtraLightBlurOverlayColor = Color.FromHex("#B0FFFFFF");

        private static int blurProcessDelayMilliseconds = 100;

        private RealtimeBlurView.RealtimeBlurView _realtimeBlurView;

        private View _blurRootView;

        /// <summary>
        /// When a page visibility changes we activate or deactivate blur updates.
        /// Setting a bigger delay could improve performance and rendering.
        /// </summary>
        public static int BlurProcessDelayMilliseconds
        {
            get => blurProcessDelayMilliseconds;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(
                        "The blur processing delay cannot be negative",
                        nameof(BlurProcessDelayMilliseconds));
                }

                blurProcessDelayMilliseconds = value;
            }
        }

        /// <summary>
        /// If set to <see langword="true"/>, the rendering result could be better (clearer blur not mixing front elements).
        /// However due to a bug in the Xamarin framework https://github.com/xamarin/xamarin-android/issues/4548, debugging is impossible with this mode (causes SIGSEGV).
        /// A suggestion would be to set it to false for debug, and to true for releases.
        /// </summary>
        public static bool ThrowStopExceptionOnDraw { get; set; } = false;

        private bool IsAndroidBlurPropertySet => MaterialFrame.AndroidBlurRadius > 0;

        private double CurrentBlurRadius =>
            IsAndroidBlurPropertySet ? MaterialFrame.AndroidBlurRadius : StyledBlurRadius;

        private string FormsId => MaterialFrame.StyleId;

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            if (MaterialFrame.AndroidBlurRootElement != null && _blurRootView == null)
            {
                UpdateAndroidBlurRootElement();
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            InternalLogger.Info(FormsId,$"Renderer::OnSizeChanged(w: {w}, h: {h}, oldw: {oldw}, oldh: {oldh})");
            LayoutBlurView();
        }

        private void LayoutBlurView()
        {
            if (MeasuredWidth == 0 || MeasuredHeight == 0 || _realtimeBlurView == null)
            {
                return;
            }

            InternalLogger.Info(FormsId,$"Renderer::LayoutBlurView(element: {MaterialFrame.StyleId})");
            int width = MeasuredWidth;
            int height = MeasuredHeight;

            _realtimeBlurView.Measure(width, height);
            _realtimeBlurView.Layout(0, 0, width, height);
        }

        private void DestroyBlur()
        {
            _realtimeBlurView?.Release();
            _realtimeBlurView = null;
        }

        private void UpdateAndroidBlurRootElement()
        {
            if (MaterialFrame.AndroidBlurRootElement == null)
            {
                return;
            }

            var formsView = MaterialFrame.AndroidBlurRootElement;
            var renderer = Platform.GetRenderer(formsView);
            if (renderer == null)
            {
                return;
            }

            bool IsAncestor(Element child, Layout parent)
            {
                if (child.Parent == null)
                {
                    return false;
                }

                if (child.Parent == parent)
                {
                    return true;
                }

                return IsAncestor(child.Parent, parent);
            }

            if (!IsAncestor(MaterialFrame, MaterialFrame.AndroidBlurRootElement))
            {
                throw new InvalidOperationException(
                    "The AndroidBlurRootElement of the MaterialFrame should be an ancestor of the MaterialFrame.");
            }

            Platform.SetRenderer(formsView, renderer);
            _blurRootView = renderer.View;

            _realtimeBlurView?.SetRootView(_blurRootView);
        }

        private void UpdateAndroidBlurOverlayColor()
        {
            if (IsAndroidBlurPropertySet)
            {
                InternalLogger.Info(FormsId,"UpdateAndroidBlurOverlayColor()");
                _realtimeBlurView?.SetOverlayColor(MaterialFrame.AndroidBlurOverlayColor.ToAndroid());
            }
        }

        private void UpdateAndroidBlurRadius()
        {
            if (IsAndroidBlurPropertySet)
            {
                InternalLogger.Info( FormsId, "Renderer::UpdateAndroidBlurRadius()");
                _realtimeBlurView?.SetBlurRadius(Context.ToPixels(MaterialFrame.AndroidBlurRadius));
            }
        }

        private void UpdateMaterialBlurStyle()
        {
            if (_realtimeBlurView == null || IsAndroidBlurPropertySet)
            {
                return;
            }

            InternalLogger.Info(FormsId,"Renderer::UpdateMaterialBlurStyle()");

            _realtimeBlurView.SetBlurRadius(Context.ToPixels(StyledBlurRadius));

            switch (MaterialFrame.MaterialBlurStyle)
            {
                case RenderedViews.MaterialFrame.BlurStyle.ExtraLight:
                    _realtimeBlurView.SetOverlayColor(ExtraLightBlurOverlayColor.ToAndroid());
                    break;
                case RenderedViews.MaterialFrame.BlurStyle.Dark:
                    _realtimeBlurView.SetOverlayColor(DarkBlurOverlayColor.ToAndroid());
                    break;

                default:
                    _realtimeBlurView.SetOverlayColor(LightBlurOverlayColor.ToAndroid());
                    break;
            }
        }

        private void EnableBlur()
        {
            InternalLogger.Info(FormsId,"Renderer::EnableBlur()");

            if (_realtimeBlurView == null)
            {
                // _preDrawListener = new PreDrawListener(this);
                _realtimeBlurView = new RealtimeBlurView.RealtimeBlurView(Context, MaterialFrame.StyleId);
            }

            UpdateAndroidBlurRadius();
            UpdateAndroidBlurOverlayColor();
            UpdateMaterialBlurStyle();
            UpdateAndroidBlurRootElement();

            _realtimeBlurView.SetDownsampleFactor(CurrentBlurRadius <= 10 ? 1 : 2);

            UpdateCornerRadius();

            if (ChildCount > 0 && ReferenceEquals(GetChildAt(0), _realtimeBlurView))
            {
                // Already added
                return;
            }

            InternalLogger.Info(FormsId,"Renderer::EnableBlur() => adding pre draw listener");
            AddView(
                _realtimeBlurView,
                0,
                new LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent,
                    GravityFlags.NoGravity));

            LayoutBlurView();
        }

        private void DisableBlur()
        {
            if (ChildCount == 0 || !ReferenceEquals(GetChildAt(0), _realtimeBlurView))
            {
                return;
            }

            InternalLogger.Info(FormsId,"Renderer::DisableBlur() => removing pre draw listener");
            RemoveView(_realtimeBlurView);
        }
    }
}