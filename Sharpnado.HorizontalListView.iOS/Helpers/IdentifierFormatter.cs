using Sharpnado.HorizontalListView.iOS.Renderers.HorizontalList;

namespace Sharpnado.HorizontalListView.iOS.Helpers
{
    public static class IdentifierFormatter
    {
        public static string FormatDataTemplateCellIdentifier(int index)
        {
            return string.Concat(nameof(iOSViewCell), index);
        }
    }
}
