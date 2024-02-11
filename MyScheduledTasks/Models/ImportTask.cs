// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Models;

/// <summary>
/// Class for properties used by the Import Task
/// </summary>
public static class ImportTask
{
    public static string ImportTaskXML { get; set; } = string.Empty;

    public static string ImportTaskName { get; set; } = string.Empty;

    public static bool ImportOverwrite { get; set; }

    public static bool ImportRunOnlyLoggedOn { get; set; } = true;

    public static bool ImportRunHighest { get; set; }
}
