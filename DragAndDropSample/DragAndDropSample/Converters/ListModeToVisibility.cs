using System;
using System.Globalization;

using DragAndDropSample.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DragAndDropSample.Converters
{
    public class ListModeToInt: IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ListMode)value;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
