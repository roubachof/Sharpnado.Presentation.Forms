using System;
using System.Collections;
using System.Linq;

using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.Helpers
{
    public static class ElementVisitor
    {
        public static void Visit(Element element, Action<Element, int> action)
        {
            Visit(element, action, 0);
        }

        private static void Visit(object @object, Action<Element, int> action, int currentDepth)
        {
            if (!(@object is Element element))
            {
                return;
            }

            action(element, currentDepth);

            if (element is ContentView contentView)
            {
                Visit(contentView.Content, action, currentDepth + 1);
            }

            var elementType = element.GetType();

            var childrenProperty = elementType.GetProperties()
                .Where(x => x.Name == nameof(IViewContainer<VisualElement>.Children))
                .FirstOrDefault(x => x.DeclaringType == typeof(Layout));

            if (childrenProperty?.GetValue(element) is IList children)
            {
                foreach (var child in children)
                {
                    Visit(child, action, currentDepth + 1);
                }
            }
        }
    }
}
