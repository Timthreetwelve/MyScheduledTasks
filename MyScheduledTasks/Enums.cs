// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks;

/// <summary>
/// Navigation Page
/// </summary>
internal enum NavPage
{
    TasksView = 0,
    AddTasks = 1,
    Settings = 3,
    About = 4,
    TaskScheduler = 6,
    Restart = 8,
    Exit = 9
}

/// <summary>
/// Theme type, Light, Dark, or System
/// </summary>
internal enum ThemeType
{
    Light = 0,
    Dark = 1,
    System = 2
}

/// <summary>
/// Size of the UI, Smallest, Smaller, Default, Larger, or Largest
/// </summary>
internal enum MySize
{
    Smallest = 0,
    Smaller = 1,
    Default = 2,
    Larger = 3,
    Largest = 4
}

/// <summary>
/// One of the 19 predefined Material Design in XAML colors
/// </summary>
internal enum AccentColor
{
    Red = 0,
    Pink = 1,
    Purple = 2,
    DeepPurple = 3,
    Indigo = 4,
    Blue = 5,
    LightBlue = 6,
    Cyan = 7,
    Teal = 8,
    Green = 9,
    LightGreen = 10,
    Lime = 11,
    Yellow = 12,
    Amber = 13,
    Orange = 14,
    DeepOrange = 15,
    Brown = 16,
    Grey = 17,
    BlueGray = 18
}

public enum Spacing
{
    Compact = 0,
    Comfortable = 1,
    Wide = 2
}