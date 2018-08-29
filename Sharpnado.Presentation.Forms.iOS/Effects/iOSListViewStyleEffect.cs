using Foundation;
using Sharpnado.Presentation.Forms.Effects;
using Sharpnado.Presentation.Forms.iOS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(iOSListViewStyleEffect), nameof(ListViewStyleEffect))]

namespace Sharpnado.Presentation.Forms.iOS.Effects
{
    [Preserve]
    public class iOSListViewStyleEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var listView = (UIKit.UITableView)Control;

            if (ListViewEffect.GetDisableSelection(Element))
            {
                listView.AllowsSelection = false;
            }
        }

        protected override void OnDetached()
        {
        }
    }
}