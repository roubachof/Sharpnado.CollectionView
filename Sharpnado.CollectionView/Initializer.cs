namespace Sharpnado.CollectionView
{
    public static class Initializer
    {
        public static void Initialize(bool loggerEnable, bool debugLogEnable)
        {
            InternalLogger.EnableLogger(debugLogEnable, debugLogEnable);
        }
    }
}