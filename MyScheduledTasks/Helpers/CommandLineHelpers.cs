// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Helpers;

internal static class CommandLineHelpers
{
    #region Properties
    public static string? CommandLineParserError { get; private set; }
    public static bool Administrator { get; private set; }
    public static bool Hide { get; private set; }
    #endregion Properties

    #region Command line parameters
    public static void ProcessCommandLine()
    {
        using CommandLineParser.CommandLineParser parser = new();
        SwitchArgument hideArgument = new('h',
                                          "hide",
                                          string.Empty,
                                          false);
        SwitchArgument adminArgument = new('a',
                                           "administrator",
                                           string.Empty,
                                           false);
        parser.Arguments.Add(hideArgument);
        parser.Arguments.Add(adminArgument);
        parser.AcceptSlash = true;
        parser.IgnoreCase = true;
        try
        {
            parser.ParseCommandLine(App.Args);
            Hide = hideArgument.Value;
            Administrator = adminArgument.Value;
        }
        catch (UnknownArgumentException e)
        {
            CommandLineParserError = "Command line argument: " + e.Message;
        }
        catch (Exception e)
        {
            CommandLineParserError = "Command line argument: " + e.Message + e.StackTrace;
        }
    }
    #endregion Command line parameters
}
