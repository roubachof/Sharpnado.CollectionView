using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sharpnado.HorizontalListView.ViewModels
{
    public class RevealAnimation
    {
        public Func<ViewCell, Task> PreRevealAnimationAsync { get; set; }

        public Func<ViewCell, Task> RevealAnimationAsync { get; set; }

        public Func<ViewCell, Task> PostRevealAnimationAsync { get; set; }
    }
}
