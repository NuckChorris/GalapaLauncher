using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Galapa.Launcher.Converters;

public sealed class ShowObject
{
    public static readonly ShowObject Instance = new();
}

public sealed class BoolToObjectOrNullConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? ShowObject.Instance : null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}