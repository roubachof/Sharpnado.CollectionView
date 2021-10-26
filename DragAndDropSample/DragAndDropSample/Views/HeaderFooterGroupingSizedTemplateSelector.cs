using DragAndDropSample.ViewModels;

using Sharpnado.CollectionView.RenderedViews;

using Xamarin.Forms;

namespace DragAndDropSample.Views
{
    public class HeaderFooterGroupingSizedTemplateSelector: SizedDataTemplateSelector
    {
        public DataTemplate HeaderTemplate { get; set; }

        public DataTemplate FooterTemplate { get; set; }

        public DataTemplate GroupHeaderTemplate { get; set; }

        public DataTemplate DudeTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            switch (item)
            {
                case DudeHeader header:
                    return HeaderTemplate;

                case DudeFooter footer:
                    return FooterTemplate;

                case DudeGroupHeader groupHeader:
                    return GroupHeaderTemplate;

                default:
                    return DudeTemplate;
            }
        }

        public override double GetItemSize(object item, double defaultSize)
        {
            switch (item)
            {
                case DudeHeader header:
                    return 60;

                case DudeFooter footer:
                    return 40;

                case DudeGroupHeader groupHeader:
                    return 60;

                default:
                    return defaultSize;
            }
        }
    }
}