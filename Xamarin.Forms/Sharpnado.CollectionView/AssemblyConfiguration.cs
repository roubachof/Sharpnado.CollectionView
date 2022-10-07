using Xamarin.Forms;
using Xamarin.Forms.Internals;

#if NET6_0_OR_GREATER
using XmlnsPrefixAttribute = Microsoft.Maui.Controls.XmlnsPrefixAttribute;
#else
[assembly: Preserve]
#endif

[assembly: XmlnsDefinition("http://sharpnado.com", "Sharpnado.CollectionView")]
[assembly: XmlnsDefinition("http://sharpnado.com", "Sharpnado.CollectionView.Effects")]
[assembly: XmlnsPrefix("http://sharpnado.com", "sho")]