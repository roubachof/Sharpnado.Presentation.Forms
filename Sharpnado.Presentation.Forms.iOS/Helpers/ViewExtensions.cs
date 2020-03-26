using System;

using UIKit;

namespace Sharpnado.Presentation.Forms.iOS.Helpers
{
    public static class ViewExtensions
    {
        private const int IndentCount = 4;

        private const string StringFormat = "Dumping iOS UIView hierarchy:{0}{1}";

        public static string DumpHierarchy(this UIView view, bool isVerbose)
        {
            var hierarchyStringBuilder = new ViewHierarchyStringBuilder(IndentCount);
            ViewVisitor.Visit(view, hierarchyStringBuilder.Add, isVerbose);
            return string.Format(StringFormat, Environment.NewLine, hierarchyStringBuilder);
        }

        public static string DumpInfo(this UIView view)
        {
            string indentSpace = $"{Environment.NewLine}    ";

            var result = $"Dumping info for {view.GetType().Name}:"
                + indentSpace + $"Bounds: ( {view.Bounds} )"
                + indentSpace + $"Frame: ( {view.Frame} )"
                + indentSpace + $"LayoutMargins: ( {view.LayoutMargins} )";

            return result;
        }
    }
}
