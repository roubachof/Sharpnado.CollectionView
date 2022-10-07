using Sharpnado.CollectionView;
using CollectionView = Sharpnado.CollectionView.CollectionView;

namespace MauiSample.Presentation.Views
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