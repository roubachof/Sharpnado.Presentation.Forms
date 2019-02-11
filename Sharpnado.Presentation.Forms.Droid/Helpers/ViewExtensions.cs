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

        public static string DumpInfo(this View view)
        {
            string indentSpace = $"{Environment.NewLine}    ";

            var result = $"Dumping info for {view.GetType().Name}:"
                + indentSpace + $"MeasuredSize: ( {view.MeasuredWidth}w, {view.MeasuredHeight}h )"
                + indentSpace + $"Padding: ( {view.PaddingLeft}l, {view.PaddingTop}t, {view.PaddingRight}r, {view.PaddingBottom}b )";

            if (view.LayoutParameters is ViewGroup.MarginLayoutParams marginParams)
            {
                result += indentSpace
                    + $"Margins: ( {marginParams.LeftMargin}l, {marginParams.TopMargin}t, {marginParams.RightMargin}r, {marginParams.BottomMargin}b )";
            }

            return result;
        }
    }
}
