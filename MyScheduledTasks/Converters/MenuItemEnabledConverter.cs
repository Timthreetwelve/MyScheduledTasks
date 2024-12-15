// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Converters;
/// <summary>
/// This converter is used to enable/disable menu items
/// depending on the number of items selected in the DataGrid
/// </summary>
internal sealed class MenuItemEnabledConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int selected && parameter is string desired)
        {
            return desired.ToLowerInvariant() switch
            {
                "one" => selected == 1,
                "oneormore" => selected >= 1,
                _ => (object)false,
            };
        }
        return true;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
