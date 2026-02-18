using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace UDTool_Desktop.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isValid)
        {
            return isValid 
                ? new SolidColorBrush(Color.Parse("#4ADE80")) // Green
                : new SolidColorBrush(Color.Parse("#F87171")); // Red
        }
        
        return new SolidColorBrush(Color.Parse("#94A3B8")); // Gray
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

