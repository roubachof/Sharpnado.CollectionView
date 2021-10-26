namespace Sharpnado.CollectionView.ViewModels
{
    public class DragAndDropInfo
    {
        public DragAndDropInfo(int from, int to, object content)
        {
            From = from;
            To = to;
            Content = content;
        }

        public int To { get; }

        public int From { get; }

        public object Content { get; }
    }
}