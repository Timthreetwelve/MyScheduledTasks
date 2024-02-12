// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Models;

/// <summary>
/// Class for properties used by the Import Task
/// </summary>
public class ImportTask
{
    public static ImportTask Import { get; set; } = new();

    public ImportTask()
    {
    }

    public string XmlFile { get; set; } = string.Empty;

    public string TaskName { get; set; } = string.Empty;

    public bool Overwrite { get; set; }

    public bool RunOnlyLoggedOn { get; set; } = true;
}
