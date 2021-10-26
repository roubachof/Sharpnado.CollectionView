using Foundation;

using Sharpnado.CollectionView.Effects;
using Sharpnado.CollectionView.iOS.Effects;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(iOSListViewStyleEffect), nameof(ListViewStyleEffect))]

namespace Sharpnado.CollectionView.iOS.Effects
{
    [Preserve]
    public class iOSListViewStyleEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var listView = (UIKit.UITableView)Control;

            if (ListViewEffect.GetDisableSelection(Element))
            {
                listView.AllowsSelection = false;
            }
        }

        protected override void OnDetached()
        {
        }
    }
}