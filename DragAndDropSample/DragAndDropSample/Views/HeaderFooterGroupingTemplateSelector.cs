using DragAndDropSample.ViewModels;

using Sharpnado.CollectionView.RenderedViews;

using Xamarin.Forms;

namespace DragAndDropSample.Views
{
    public class HeaderFooterGroupingTemplateSelector: DataTemplateSelector
    {
        public SizedDataTemplate HeaderTemplate { get; set; }

        public SizedDataTemplate FooterTemplate { get; set; }

        public SizedDataTemplate GroupHeaderTemplate { get; set; }

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
    }
}