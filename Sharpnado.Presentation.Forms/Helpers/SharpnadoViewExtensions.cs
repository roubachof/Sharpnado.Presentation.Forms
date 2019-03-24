using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.Helpers
{
    public static class SharpnadoViewExtensions
    {
        public static Task<bool> AnimateTo(
            this VisualElement view,
            double start,
            double end,
            string name,
            Action<VisualElement, double> updateAction,
            uint length = 250,
            Easing easing = null)
        {
            if (easing == null)
            {
                easing = Easing.Linear;
            }

            var tcs = new TaskCompletionSource<bool>();
            var weakView = new WeakReference<VisualElement>(view);
            new Animation(UpdateProperty, start, end, easing, null).Commit(
                view,
                name,
                16U,
                length,
                null,
                (f, a) => tcs.SetResult(a),
                null);
            return tcs.Task;

            void UpdateProperty(double f)
            {
                if (!weakView.TryGetTarget(out var target))
                {
                    return;
                }

                updateAction(target, f);
            }
        }

        public static Task<bool> HeightRequestTo(
            this VisualElement view,
            double height,
            uint length = 250,
            Easing easing = null)
        {
            return view.AnimateTo(
                view.HeightRequest,
                height,
                nameof(HeightRequestTo),
                (v, value) => v.HeightRequest = value,
                length,
                easing);
        }
    }
}