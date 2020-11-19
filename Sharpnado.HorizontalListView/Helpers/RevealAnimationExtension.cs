using System;
using Sharpnado.HorizontalListView.ViewModels;
using Xamarin.Forms.Xaml;

namespace Sharpnado.HorizontalListView.Helpers
{
    public class RevealAnimationExtension : IMarkupExtension<RevealAnimation>
    {
        public RevealAnimationType Animation { get; set; }
        public RevealAnimation ProvideValue(IServiceProvider serviceProvider)
        {
            switch (Animation)
            {
                case RevealAnimationType.Fade:
                    return RevealAnimationHelper.RevealFadeAnimation();
                    break;
                case RevealAnimationType.Rotation:
                    return RevealAnimationHelper.RevealRotateAnimation();
                    break;
                case RevealAnimationType.Flip:
                    return RevealAnimationHelper.RevealFlipAnimation();
                    break;
                default:
                    return RevealAnimationHelper.Nothing();
                    throw new ArgumentOutOfRangeException($"RevealAnimationEnum ({Animation}) is not handled");
            }
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<RevealAnimation>).ProvideValue(serviceProvider);
        }
    }
}
