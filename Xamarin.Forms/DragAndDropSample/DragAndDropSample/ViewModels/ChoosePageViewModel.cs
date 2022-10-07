using Xamarin.Forms;

namespace DragAndDropSample.ViewModels
{
    public class ChoosePageViewModel
    {
        public ChoosePageViewModel()
        {
        }

        public LogoLetterVmo[] Logo { get; } =
        {
            new LogoLetterVmo("S", Color.FromHex("#FF0266"), "ThinAccentNeumorphism"),
            new LogoLetterVmo("H", Color.White, "ThinDarkerNeumorphism"),
            new LogoLetterVmo("V", Color.White, "ThinDarkerNeumorphism"),
            new LogoLetterVmo("L", Color.White, "ThinDarkerNeumorphism"),
            new LogoLetterVmo("G", Color.White, "ThinDarkerNeumorphism"),
            new LogoLetterVmo("", Color.White, "ThinDarkerNeumorphism"),
            new LogoLetterVmo("V", Color.White, "ThinDarkerNeumorphism"),
            new LogoLetterVmo("", Color.White, "ThinDarkerNeumorphism"),
            new LogoLetterVmo("V", Color.White, "ThinDarkerNeumorphism"),
        };
    }
}