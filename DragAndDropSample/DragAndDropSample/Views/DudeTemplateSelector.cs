using Sharpnado.HorizontalListView.RenderedViews;

using Xamarin.Forms;

namespace DragAndDropSample.Views
{
    public class DudeTemplateSelector: DataTemplateSelector
    {
        public DataTemplate GridTemplate { get; set; }

        public DataTemplate HorizontalTemplate { get; set; }

        public DataTemplate VerticalTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var horizontalList = (HorizontalListView)container;
            HorizontalListViewLayout layout = horizontalList.ListLayout;

            switch (layout)
            {
                case HorizontalListViewLayout.Grid:
                    return GridTemplate;

                case HorizontalListViewLayout.Linear:
                    return HorizontalTemplate;

                default:
                    return VerticalTemplate;
            }
        }
    }
}