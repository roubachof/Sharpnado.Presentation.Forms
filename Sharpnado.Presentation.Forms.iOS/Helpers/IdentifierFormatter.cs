using Sharpnado.Presentation.Forms.iOS.Renderers.HorizontalList;

namespace Sharpnado.Presentation.Forms.iOS.Helpers
{
    public static class IdentifierFormatter
    {
        public static string FormatDataTemplateCellIdentifier(int index)
        {
            return string.Concat(nameof(iOSViewCell), index);
        }
    }
}
