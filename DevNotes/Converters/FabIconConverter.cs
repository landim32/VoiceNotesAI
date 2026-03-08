using System.Globalization;

namespace DevNotes.Converters;

public class FabIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isOpen)
            return isOpen ? "\u2715" : "\u002B";
        return "\u002B";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
