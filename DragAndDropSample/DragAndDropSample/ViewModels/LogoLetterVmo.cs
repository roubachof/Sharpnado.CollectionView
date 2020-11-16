using Xamarin.Forms;

namespace DragAndDropSample.ViewModels
{
    public class LogoLetterVmo
    {
        public LogoLetterVmo(string text, Color color, string shadowResourceName)
        {
            Text = text;
            TextColor = color;
            ShadowResourceName = shadowResourceName;
        }

        public string Text { get; }

        public Color TextColor { get; }

        public string ShadowResourceName { get; }
    }
}