using Sharpnado.CollectionView.RenderedViews;

using Xamarin.Forms;

using CollectionView = Sharpnado.CollectionView.RenderedViews.CollectionView;

namespace DragAndDropSample.Views
{
    public class DudeTemplateSelector: DataTemplateSelector
    {
        public DataTemplate GridTemplate { get; set; }

        public DataTemplate HorizontalTemplate { get; set; }

        public DataTemplate VerticalTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var horizontalList = (CollectionView)container;
            CollectionViewLayout layout = horizontalList.CollectionLayout;

            switch (layout)
            {
                case CollectionViewLayout.Grid:
                    return GridTemplate;

                case CollectionViewLayout.Horizontal:
                    return HorizontalTemplate;

                default:
                    return VerticalTemplate;
            }
        }
    }
}