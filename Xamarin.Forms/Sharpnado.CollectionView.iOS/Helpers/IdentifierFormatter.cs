using Sharpnado.CollectionView.iOS.Renderers;

namespace Sharpnado.CollectionView.iOS.Helpers
{
    public static class IdentifierFormatter
    {
        public static string FormatDataTemplateCellIdentifier(int index)
        {
            return string.Concat(nameof(iOSViewCell), index);
        }
    }
}
