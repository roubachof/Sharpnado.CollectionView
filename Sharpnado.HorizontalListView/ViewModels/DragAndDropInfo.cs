namespace Sharpnado.HorizontalListView.ViewModels
{
    public class DragAndDropInfo
    {
        public DragAndDropInfo(int to, int @from, object content)
        {
            To = to;
            From = @from;
            Content = content;
        }

        public int To { get; }

        public int From { get; }

        public object Content { get; }
    }
}