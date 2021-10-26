namespace DragAndDropSample.ViewModels
{
    public interface IDudeItem
    {
    }

    public class DudeHeader : IDudeItem
    {
    }

    public class DudeFooter : IDudeItem
    {
    }

    public class DudeGroupHeader : IDudeItem
    {
        public int StarCount { get; set; }

        public string Text => $"{StarCount} Stars";
    }
}