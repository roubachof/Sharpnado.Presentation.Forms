using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Sharpnado.Presentation.Forms.Effects
{
    [Preserve]
    public static class TapCommandEffect
    {
        public static readonly BindableProperty TapProperty = BindableProperty.CreateAttached(
            "Tap",
            typeof(ICommand),
            typeof(TapCommandEffect),
            default(ICommand),
            propertyChanged: PropertyChanged);

        public static readonly BindableProperty TapParameterProperty = BindableProperty.CreateAttached(
            "TapParameter",
            typeof(object),
            typeof(TapCommandEffect),
            default(object),
            propertyChanged: PropertyChanged);

        public static readonly BindableProperty LongTapProperty = BindableProperty.CreateAttached(
            "LongTap",
            typeof(ICommand),
            typeof(TapCommandEffect),
            default(ICommand),
            propertyChanged: PropertyChanged);

        public static readonly BindableProperty LongTapParameterProperty = BindableProperty.CreateAttached(
            "LongTapParameter",
            typeof(object),
            typeof(TapCommandEffect),
            default(object));

        public static void SetTap(BindableObject view, ICommand value)
        {
            view.SetValue(TapProperty, value);
        }

        public static ICommand GetTap(BindableObject view)
        {
            return (ICommand)view.GetValue(TapProperty);
        }

        public static void SetTapParameter(BindableObject view, object value)
        {
            view.SetValue(TapParameterProperty, value);
        }

        public static object GetTapParameter(BindableObject view)
        {
            return view.GetValue(TapParameterProperty);
        }

        public static void SetLongTap(BindableObject view, ICommand value)
        {
            view.SetValue(LongTapProperty, value);
        }

        public static ICommand GetLongTap(BindableObject view)
        {
            return (ICommand)view.GetValue(LongTapProperty);
        }

        public static void SetLongTapParameter(BindableObject view, object value)
        {
            view.SetValue(LongTapParameterProperty, value);
        }

        public static object GetLongTapParameter(BindableObject view)
        {
            return view.GetValue(LongTapParameterProperty);
        }

        private static void PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is View view))
            {
                return;
            }

            var eff = view.Effects.FirstOrDefault(e => e is TapCommandRoutingEffect);

            if (GetTap(bindable) != null || GetLongTap(bindable) != null)
            {
                if (eff == null)
                {
                    view.Effects.Add(new TapCommandRoutingEffect());
                }
            }
            else
            {
                if (eff != null && view.BindingContext != null)
                {
                    view.Effects.Remove(eff);
                }
            }
        }
    }

    public class TapCommandRoutingEffect : RoutingEffect
    {
        public static readonly string Name = $"Silly.{nameof(TapCommandEffect)}";

        public TapCommandRoutingEffect()
            : base(Name)
        {
        }
    }
}