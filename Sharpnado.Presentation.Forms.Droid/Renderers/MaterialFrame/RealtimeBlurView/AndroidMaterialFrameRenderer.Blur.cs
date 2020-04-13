using System;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms.Platform.Android;

namespace Sharpnado.Presentation.Forms.Droid.Renderers.MaterialFrame
{
    public partial class AndroidMaterialFrameRenderer
    {
        private RealtimeBlurView.RealtimeBlurView _realtimeBlurView;

        private bool IsBlurInitialized => _realtimeBlurView != null;

        private PreDrawListener _preDrawListener;
        private GlobalLayoutListener _globalLayoutListener;

        private bool InitializeBlurIfNeeded()
        {
            if (_realtimeBlurView != null)
            {
                return false;
            }

            _preDrawListener = new PreDrawListener(this);
            _globalLayoutListener = new GlobalLayoutListener(this);

            _realtimeBlurView = new RealtimeBlurView.RealtimeBlurView(Context);
            return true;
        }

        private void DestroyBlur()
        {
            _realtimeBlurView?.Release();
            _realtimeBlurView = null;

            ViewTreeObserver.RemoveOnPreDrawListener(null);
        }

        private void EnableBlur()
        {
            InitializeBlurIfNeeded();
            if (MeasuredWidth == 0 || MeasuredHeight == 0)
            {
                ViewTreeObserver.AddOnGlobalLayoutListener(_globalLayoutListener);
                return;
            }

            Init();
        }

        private void Init()
        {
            ViewTreeObserver.AddOnPreDrawListener(_preDrawListener);
            _realtimeBlurView.SetBlurRadius(16);
            _realtimeBlurView.SetDownsampleFactor(2);
            AddView(
                _realtimeBlurView,
                0,
                new FrameLayout.LayoutParams(
                    FrameLayout.LayoutParams.MatchParent,
                    FrameLayout.LayoutParams.MatchParent,
                    GravityFlags.NoGravity));
        }

        private void DisableBlur()
        {
            ViewTreeObserver.RemoveOnPreDrawListener(_preDrawListener);
            RemoveView(_realtimeBlurView);
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            //if (MaterialFrame.MaterialTheme == RenderedViews.MaterialFrame.Theme.AcrylicBlur)
            //{
            //    _realtimeBlurView.RequestLayout();
            //}
        }

        private class GlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
        {
            private readonly WeakReference<AndroidMaterialFrameRenderer> _weakFrame;

            public GlobalLayoutListener(AndroidMaterialFrameRenderer frame)
            {
                _weakFrame = new WeakReference<AndroidMaterialFrameRenderer>(frame);
            }

            public GlobalLayoutListener(IntPtr handle, JniHandleOwnership transfer)
                : base(handle, transfer)
            {
            }

            public void OnGlobalLayout()
            {
                if (!_weakFrame.TryGetTarget(out var frame))
                {
                    return;
                }

                frame.ViewTreeObserver.RemoveOnGlobalLayoutListener(this);

                frame.Init();
            }
        }

        private class PreDrawListener : Java.Lang.Object, ViewTreeObserver.IOnPreDrawListener
        {
            private readonly WeakReference<AndroidMaterialFrameRenderer> _weakFrame;

            public PreDrawListener(AndroidMaterialFrameRenderer frame)
            {
                _weakFrame = new WeakReference<AndroidMaterialFrameRenderer>(frame);
            }

            public PreDrawListener(IntPtr handle, JniHandleOwnership transfer)
                : base(handle, transfer)
            {
            }

            public bool OnPreDraw()
            {
                if (!_weakFrame.TryGetTarget(out var frame))
                {
                    return false;
                }

                if (frame.MeasuredWidth == 0 || frame.MeasuredHeight == 0)
                {
                    return false;
                }

                int width = frame.MeasuredWidth;
                int height = frame.MeasuredHeight;

                frame._realtimeBlurView.Measure(width, height);
                frame._realtimeBlurView.Layout(0, 0, width, height);

                frame._realtimeBlurView.ScaleX = frame.ScaleX;
                frame._realtimeBlurView.ScaleY = frame.ScaleY;

                frame._realtimeBlurView.RotationX = frame.RotationX;
                frame._realtimeBlurView.RotationY = frame.RotationY;

                frame._realtimeBlurView.TranslationX = frame.TranslationX;
                frame._realtimeBlurView.TranslationY = frame.TranslationY;

                return true;
            }
        }
    }
}