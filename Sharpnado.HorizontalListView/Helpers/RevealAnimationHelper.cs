using System;
using System.Threading.Tasks;
using Sharpnado.HorizontalListView.ViewModels;
using Xamarin.Forms;

namespace Sharpnado.HorizontalListView.Helpers
{
    public static class RevealAnimationHelper
    {
        #region Fade
        public static RevealAnimation RevealFadeAnimation()
        {
            return new RevealAnimation()
            {
                PreRevealAnimationAsync = async (viewCell) =>
                {
                    viewCell.View.Opacity = 0;
                    await Task.Delay(300);
                },
                RevealAnimationAsync = async (viewCell) =>
                {
                    await viewCell.View.FadeTo(1, 750);
                    await Task.Delay(200);
                },
                PostRevealAnimationAsync = NoAnim()
            };
        }

      

        #endregion

        #region Flip
        public static RevealAnimation RevealFlipAnimation()
        {
            return new RevealAnimation()
            {
                PreRevealAnimationAsync = async (viewCell) =>
                {
                    viewCell.View.RotationY = -90;
                    await Task.Delay(300);
                },
                RevealAnimationAsync = async (viewCell) =>
                {
                    await viewCell.View.RotateYTo(0, 500);
                    await Task.Delay(200);
                },
                PostRevealAnimationAsync = NoAnim()
            };
        }
        #endregion

        #region Reveal
        public static RevealAnimation RevealRotateAnimation()
        {
            return new RevealAnimation()
            {
                PreRevealAnimationAsync = async (viewCell) =>
                {
                    viewCell.View.Rotation = 0;
                    await Task.Delay(300);
                },
                RevealAnimationAsync = async (viewCell) =>
                {
                    await viewCell.View.RotateTo(720, 1000);
                    await Task.Delay(200);
                },
                PostRevealAnimationAsync = NoAnim()
            };
        }
        #endregion

        #region Others
        public static Func<ViewCell, Task> NoAnim()
        {
            return async (viewCell) =>
            {
                await Task.Delay(200);
            };
        }
        public static RevealAnimation Nothing()
        {
            return new RevealAnimation()
            {
                PreRevealAnimationAsync = NoAnim(),
                RevealAnimationAsync = NoAnim(),
                PostRevealAnimationAsync = NoAnim()
            };
        }
        #endregion
    }

    public enum RevealAnimationType
    {
        Fade,
        Rotation,
        Flip
    }
}