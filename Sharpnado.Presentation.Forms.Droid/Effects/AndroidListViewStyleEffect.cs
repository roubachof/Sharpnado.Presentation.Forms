using Android.Widget;
using Sharpnado.Presentation.Forms.Droid.Effects;
using Sharpnado.Presentation.Forms.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(AndroidListViewStyleEffect), nameof(ListViewStyleEffect))]

namespace Sharpnado.Presentation.Forms.Droid.Effects
{
    [Preserve]
    public class AndroidListViewStyleEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var listView = (Android.Widget.ListView)Control;

            if (ListViewEffect.GetDisableSelection(Element))
            {
                listView.ChoiceMode = ChoiceMode.None;
            }
        }

        protected override void OnDetached()
        {
        }
    }
}