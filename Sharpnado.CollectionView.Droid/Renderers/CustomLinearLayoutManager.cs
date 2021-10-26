using System;

using Android.Content;
using Android.Runtime;

using AndroidX.RecyclerView.Widget;

#if __ANDROID_29__

#else
using Android.Support.V7.Widget;
#endif

namespace Sharpnado.CollectionView.Droid.Renderers
{
    public class CustomLinearLayoutManager : LinearLayoutManager
    {
        public CustomLinearLayoutManager(IntPtr javaReference, JniHandleOwnership transfer)
            : base(
            javaReference,
            transfer)
        {
        }

        public CustomLinearLayoutManager(Context context, int orientation, bool reverseLayout)
            : base(
            context,
            orientation,
            reverseLayout)
        {
        }

        public bool CanScroll { get; set; }

        public override bool CanScrollHorizontally()
        {
            return CanScroll && base.CanScrollHorizontally();
        }
    }
}