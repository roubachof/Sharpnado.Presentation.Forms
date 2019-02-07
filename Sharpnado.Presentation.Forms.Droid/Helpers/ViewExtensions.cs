using System;

using Android.Views;

namespace Sharpnado.Presentation.Forms.Droid.Helpers
{
    public static class ViewExtensions
    {
        private const int IndentCount = 4;

        private const string StringFormat = "Dumping Android view hierarchy:{0}{1}";

        public static string DumpHierarchy(this View view)
        {
            var hierarchyStringBuilder = new ViewHierarchyStringBuilder(IndentCount);
            ViewVisitor.Visit(view, hierarchyStringBuilder.Add);
            return string.Format(StringFormat, Environment.NewLine, hierarchyStringBuilder);
        }
    }
}
