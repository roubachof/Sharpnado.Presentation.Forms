using System.Text;

using Android.Views;

namespace Sharpnado.Presentation.Forms.Droid.Helpers
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

        public void Add(View view, int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                _stringBuilder.Append(_indentSpace);
            }

            _stringBuilder.AppendLine($"<{view.GetType().Name}>");
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}
