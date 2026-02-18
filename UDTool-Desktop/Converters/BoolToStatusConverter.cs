using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace UDTool_Desktop.Converters;

public class BoolToStatusConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isValid)
        {
            return isValid ? "Connected" : "Not Connected";
        }
        
        return "Unknown";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

