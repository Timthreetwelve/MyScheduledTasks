// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Converters;

/// <summary>
/// Converter to annotate root folder
/// </summary>
internal sealed class RootConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string root = GetStringResource("SettingsItem_Root");
        return value?.ToString() == "\\" && UserSettings.Setting!.AnnotateRoot ? $"\\  [{root}]" : value!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
