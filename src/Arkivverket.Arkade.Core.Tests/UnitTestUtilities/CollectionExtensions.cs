using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Tests.UnitTestUtilities;

public static class CollectionExtensions
{
    /// <summary>
    /// Returns a collection of items from the source collection with the specified items excluded exactly once.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collections.</typeparam>
    /// <param name="sourceItems">The collection of source items.</param>
    /// <param name="itemsToExcludeOnce">The collection of items to exclude exactly once.</param>
    /// <returns>A collection with the items excluded exactly once from the source items.</returns>
    public static IEnumerable<T> ExceptOnce<T>(this IEnumerable<T> sourceItems, IEnumerable<T> itemsToExcludeOnce)
    {
        var remainingItems = new List<T>(sourceItems);

        foreach (T itemToExclude in itemsToExcludeOnce)
            remainingItems.Remove(itemToExclude);

        return remainingItems;
    }
}
