// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Converters;
/// <summary>
/// This converter is used to enable/disable menu items
/// depending on the number of items selected in the DataGrid
/// </summary>
internal class MenuItemEnabledConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int selected && parameter is string desired)
        {
            switch (desired.ToLowerInvariant())
            {
                case "one":
                    return selected == 1;
                case "oneormore":
                    return selected >= 1;
                default:
                    return false;
            }
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
