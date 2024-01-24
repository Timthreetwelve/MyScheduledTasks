// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Models;

internal class CmdLineOptions
{
    [Option('h', "hide", Required = false)]
    public bool Hide { get; set; }

    [Option('a', "administrator", Required = false)]
    public bool Administrator { get; set; }
}
