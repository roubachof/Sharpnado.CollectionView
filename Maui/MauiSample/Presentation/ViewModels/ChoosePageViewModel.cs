using System.Collections.ObjectModel;

namespace MauiSample.Presentation.ViewModels
{
    public class ChoosePageViewModel
    {
        public ChoosePageViewModel()
        {
        }

        public ObservableCollection<LogoLetterVmo> Logo { get; } = new()
            {
            new LogoLetterVmo("S", Color.FromHex("#FF0266"), "ShadowAccentBottom"),
            new LogoLetterVmo("H", Colors.White, "ShadowNeumorphismBottom"),
            new LogoLetterVmo("V", Colors.White, "ShadowNeumorphismBottom"),
            new LogoLetterVmo("L", Colors.White, "ShadowNeumorphismBottom"),
            new LogoLetterVmo("G", Colors.White, "ShadowNeumorphismBottom"),
            new LogoLetterVmo("", Color.FromHex("#FF0266"), "ShadowNeumorphismBottom", 32, "island.png"),
            new LogoLetterVmo("V", Colors.White, "ShadowNeumorphismBottom"),
            new LogoLetterVmo("", Color.FromHex("#FF0266"), "ShadowNeumorphismBottom", 32, "cocktail.png"),
            new LogoLetterVmo("V", Colors.White, "ShadowNeumorphismBottom"),
        };
    }
}
