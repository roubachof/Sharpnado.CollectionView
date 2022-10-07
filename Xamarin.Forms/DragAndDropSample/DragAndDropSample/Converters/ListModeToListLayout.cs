using System;
using System.Globalization;

using DragAndDropSample.ViewModels;

using Sharpnado.CollectionView.RenderedViews;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DragAndDropSample.Converters
{
    public class ListModeToListLayout: IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mode = (ListMode)value;

            switch (mode)
            {
                case ListMode.Vertical:
                    return CollectionViewLayout.Vertical; 
                case ListMode.Grid:
                    return CollectionViewLayout.Grid;
                default:
                    return CollectionViewLayout.Horizontal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
