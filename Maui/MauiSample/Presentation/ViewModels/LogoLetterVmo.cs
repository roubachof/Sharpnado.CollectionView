namespace MauiSample.Presentation.ViewModels
{
    public class LogoLetterVmo
    {
        public LogoLetterVmo(
            string text,
            Color color,
            string shadowResourceName,
            double fontSize = 90,
            string imageSource = null)
        {
            Text = text;
            TextColor = color;
            ShadowResourceName = shadowResourceName;
            FontSize = fontSize;
            ImageSource = imageSource;
        }

        public string Text { get; }

        public Color TextColor { get; }

        public string ShadowResourceName { get; }

        public double FontSize { get; }

        public string ImageSource { get; }

        public bool HasImage => ImageSource != null;
    }
}