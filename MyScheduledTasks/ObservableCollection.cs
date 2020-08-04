// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyScheduledTasks
{
    // https://stackoverflow.com/questions/3973137/order-a-observablecollectiont-without-creating-a-new-one
    public static class ObservableCollection
    {
        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> source,
            Func<TSource, TKey> keySelector, bool ascending = true)
        {
            if (ascending)
            {
                List<TSource> sortedList = source.OrderBy(keySelector).ToList();
                source.Clear();
                foreach (var sortedItem in sortedList)
                {
                    source.Add(sortedItem);
                }
            }
            else
            {
                List<TSource> sortedList = source.OrderByDescending(keySelector).ToList();
                source.Clear();
                foreach (var sortedItem in sortedList)
                {
                    source.Add(sortedItem);
                }
            }
        }
    }
}
