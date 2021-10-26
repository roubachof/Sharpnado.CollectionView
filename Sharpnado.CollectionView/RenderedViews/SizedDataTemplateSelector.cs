using Xamarin.Forms;

namespace Sharpnado.CollectionView.RenderedViews
{
    public abstract class SizedDataTemplateSelector : DataTemplateSelector
    {
        public abstract double GetItemSize(object item, double defaultSize);
    }
}