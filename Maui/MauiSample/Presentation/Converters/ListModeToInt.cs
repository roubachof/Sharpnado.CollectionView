using System.Globalization;
using MauiSample.Presentation.ViewModels;

namespace MauiSample.Presentation.Converters
{
    public class ListModeToVisibility: IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ListMode)value) == ListMode.Vertical;
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
