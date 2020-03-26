using System.Text;

using UIKit;

namespace Sharpnado.Presentation.Forms.iOS.Helpers
{
    public class ViewHierarchyStringBuilder
    {
        private readonly StringBuilder _stringBuilder;

        private readonly string _indentSpace;

        public ViewHierarchyStringBuilder(int indentCount)
        {
            _stringBuilder = new StringBuilder();
            _indentSpace = string.Empty.PadLeft(indentCount);
        }

        public void Add(UIView view, int depth, bool isVerbose = false)
        {
            for (int i = 0; i < depth; i++)
            {
                _stringBuilder.Append(_indentSpace);
            }

            _stringBuilder.Append($"<{view.GetType().Name}");

            if (isVerbose)
            {
                _stringBuilder.Append($" Frame={view.Frame}");
            }

            _stringBuilder.AppendLine(">");
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}
