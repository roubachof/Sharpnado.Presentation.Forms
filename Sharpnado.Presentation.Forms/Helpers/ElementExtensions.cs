using System;

using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.Helpers
{
    public static class ElementExtensions
    {
        private const int IndentCount = 4;

        private const string StringFormat = "Dumping element hierarchy:{0}{1}";

        public static string DumpHierarchy(this Element element)
        {
            var hierarchyStringBuilder = new ElementHierarchyStringBuilder(IndentCount);
            ElementVisitor.Visit(element, hierarchyStringBuilder.Add);
            return string.Format(StringFormat, Environment.NewLine, hierarchyStringBuilder);
        }
    }
}
