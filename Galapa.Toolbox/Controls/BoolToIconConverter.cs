using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Galapa.Toolbox.Controls;

/// <summary>
///     Converts a boolean value to an icon path geometry.
/// </summary>
public class BoolToIconConverter : IValueConverter
{
    /// <summary>
    ///     The path data to use when the value is true.
    /// </summary>
    public string? TrueValue { get; set; }

    /// <summary>
    ///     The path data to use when the value is false.
    /// </summary>
    public string? FalseValue { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            var pathData = boolValue ? this.TrueValue : this.FalseValue;
            if (!string.IsNullOrEmpty(pathData)) return Geometry.Parse(pathData);
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}