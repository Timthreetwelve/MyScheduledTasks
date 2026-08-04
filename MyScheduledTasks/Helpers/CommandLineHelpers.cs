// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Helpers;

internal static class CommandLineHelpers
{
    #region Properties
    public static bool Administrator { get; private set; }
    public static bool Hide { get; private set; }
    #endregion Properties

    #region Parse command line parameters
    /// <summary>
    /// Parses the command line arguments and sets the corresponding properties.
    /// </summary>
    /// <remarks>
    /// This method does not consider multiple arguments since the administrator and hide arguments are expected to be mutually exclusive.
    /// If the administrator argument is present, the application will restart with elevated privileges, and the hide argument will be ignored.
    /// </remarks>
    public static void ParseCommandLine()
    {
        Administrator = false;
        Hide = false;

        try
        {
            foreach (string rawArgument in App.Args)
            {
                if (string.IsNullOrWhiteSpace(rawArgument))
                {
                    continue;
                }

                string argument = rawArgument.Trim();
                if (IsHideArgument(argument))
                {
                    Hide = true;
                    continue;
                }
                if (IsAdministratorArgument(argument))
                {
                    Administrator = true;
                    continue;
                }

                _log.Warn($"Unknown command line argument: \"{argument}\"");
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"Error processing command line arguments. {ex.Message}");
        }
    }
    #endregion Parse command line parameters

    #region Argument checks
    /// <summary>
    /// Determines whether the specified argument is a hide argument.
    /// </summary>
    /// <returns>true if the argument is a hide argument; otherwise, false.</returns>
    private static bool IsHideArgument(string argument)
    {
        string normalized = NormalizeArgument(argument);
        return normalized.Equals("h", StringComparison.Ordinal) ||
               normalized.Equals("hide", StringComparison.Ordinal);
    }

    /// <summary>
    /// Determines whether the specified argument is an administrator argument.
    /// </summary>
    /// <returns>true if the argument is an administrator argument; otherwise, false.</returns>
    private static bool IsAdministratorArgument(string argument)
    {
        string normalized = NormalizeArgument(argument);
        return normalized.Equals("a", StringComparison.Ordinal) ||
               normalized.Equals("administrator", StringComparison.Ordinal);
    }
    #endregion Argument checks

    /// <summary>
    /// Normalizes the specified argument by trimming whitespace, removing all leading '-'s or '/'s, and converting to lowercase.
    /// </summary>
    /// <remarks>
    /// This method does not consider all possible command line argument formats, but it is sufficient for the expected arguments in this application.
    /// </remarks>
    /// <returns>The normalized argument string.</returns>
    private static string NormalizeArgument(string argument)
    {
         return argument.Trim().TrimStart('-', '/').ToLowerInvariant();
    }
}
