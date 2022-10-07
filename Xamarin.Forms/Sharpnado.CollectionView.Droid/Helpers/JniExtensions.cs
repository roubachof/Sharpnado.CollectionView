using System;

namespace Sharpnado.CollectionView.Droid.Helpers
{
    public static class JniExtensions
    {
        public static bool IsNullOrDisposed(this Java.Lang.Object javaObject)
        {
            return javaObject == null || javaObject.Handle == IntPtr.Zero;
        }
    }
}