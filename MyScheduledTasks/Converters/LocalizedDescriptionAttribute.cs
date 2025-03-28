﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Converters;

/// <summary>
/// Allows a Description Attribute in an Enum to be localized
/// </summary>
/// <remarks>
/// Based on https://brianlagunas.com/localize-enum-descriptions-in-wpf/
/// </remarks>
/// <seealso cref="System.ComponentModel.DescriptionAttribute" />
internal sealed class LocalizedDescriptionAttribute(string resourceKey) : DescriptionAttribute
{
    public override string Description
    {
        get
        {
            object description;
            try
            {
                description = Application.Current.TryFindResource(resourceKey);
            }
            catch (Exception)
            {
                return $"{resourceKey} value is null";
            }

            return description is null ? $"{resourceKey} resource not found" : description.ToString()!;
        }
    }
}
