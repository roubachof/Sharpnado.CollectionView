namespace MauiSample.Presentation.ViewModels
{
    public class ChoosePageViewModel
    {
        public ChoosePageViewModel()
        {
        }

        public LogoLetterVmo[] Logo { get; } =
        {
            new LogoLetterVmo("S", Color.FromHex("#FF0266"), "ShadowAccentBottom"),
            new LogoLetterVmo("H", Colors.White, "ShadowNeumorphismBottom"),
            new LogoLetterVmo("V", Colors.White, "ShadowNeumorphismBottom"),
            new LogoLetterVmo("L", Colors.White, "ShadowNeumorphismBottom"),
            new LogoLetterVmo("G", Colors.White, "ShadowNeumorphismBottom"),
            new LogoLetterVmo("", Colors.White, "ShadowNeumorphismBottom"),
            new LogoLetterVmo("V", Colors.White, "ShadowNeumorphismBottom"),
            new LogoLetterVmo("", Colors.White, "ShadowNeumorphismBottom"),
            new LogoLetterVmo("V", Colors.White, "ShadowNeumorphismBottom"),
        };
    }
}
