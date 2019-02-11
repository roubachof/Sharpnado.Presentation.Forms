using System.Text;

using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.Helpers
{
    public class ElementHierarchyStringBuilder
    {
        private readonly StringBuilder _stringBuilder;

        private readonly string _indentSpace;

        public ElementHierarchyStringBuilder(int indentCount)
        {
            _stringBuilder = new StringBuilder();
            _indentSpace = string.Empty.PadLeft(indentCount);
        }

        public void Add(Element element, int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                _stringBuilder.Append(_indentSpace);
            }

            _stringBuilder.Append('<');
            _stringBuilder.Append($"{element.GetType().Name}");

            if (element is VisualElement visualElement)
            {
                if (!visualElement.IsVisible)
                {
                    _stringBuilder.Append(" Hidden");
                }

                _stringBuilder.Append($" Size({visualElement.Width}w, {visualElement.Height}h)");
            }

            _stringBuilder.Append('>');
            _stringBuilder.AppendLine();
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}
