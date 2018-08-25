using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.Behaviors
{
    public class TimedVisibilityBehavior : Behavior<View>
    {
        private bool _lastVisibility;

        public TimedVisibilityBehavior()
        {
            VisibilityInSeconds = 5;
        }

        public int VisibilityInSeconds
        {
            get;
            set;
        }

        protected override void OnAttachedTo(View bindable)
        {
            _lastVisibility = bindable.IsVisible;
            bindable.PropertyChanged += this.ViewPropertyChanged;
        }

        private async void ViewPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var view = (View)sender;
            if (e.PropertyName == "IsVisible")
            {
                if (!_lastVisibility && view.IsVisible)
                {
                    await Task.Delay(TimeSpan.FromSeconds(VisibilityInSeconds));
                    Device.BeginInvokeOnMainThread(() => view.IsVisible = false);
                }
                else
                {
                    _lastVisibility = view.IsVisible;
                }
            }
        }
    }
}