using System.Linq;
using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.Effects
{
    public static class ListViewEffect
    {
        public static readonly BindableProperty DisableSelectionProperty = BindableProperty.CreateAttached(
            "DisableSelection",
            typeof(bool),
            typeof(ListViewEffect),
            default(bool),
            BindingMode.TwoWay,
            propertyChanged: AttachEffect);

        public static bool GetDisableSelection(BindableObject element)
        {
            return (bool)element.GetValue(DisableSelectionProperty);
        }

        public static void SetDisableSelection(BindableObject element, bool value)
        {
            element.SetValue(DisableSelectionProperty, value);
        }

        private static void AttachEffect(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is ListView listView))
            {
                return;
            }

            var effect = listView.Effects.FirstOrDefault(x => x is ListViewStyleEffect);
            if (effect == null)
            {
                listView.Effects.Add(new ListViewStyleEffect());
            }
        }
    }

    public class ListViewStyleEffect : RoutingEffect
    {
        public static readonly string Name = $"Silly.{nameof(ListViewStyleEffect)}";

        public ListViewStyleEffect()
            : base(Name)
        {
        }
    }
}
