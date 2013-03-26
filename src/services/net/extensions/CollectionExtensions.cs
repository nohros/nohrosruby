using System;
using System.Collections.Generic;

namespace Nohros.Ruby
{
  public static class CollectionExtensions
  {
    /// <summary>
    /// Determines whether the <paramref name="collection"/> contains a
    /// specific value.
    /// </summary>
    /// <param name="collection">
    /// An aray of strings to serach for <paramref name="value"/>.
    /// </param>
    /// <param name="value">
    /// The string to locate in the <paramref name="collection"/>
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is found in the
    /// <paramref name="collection"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool Contains(this IEnumerable<string> collection,
      string value, StringComparison comparison_type) {
      foreach (var s in collection) {
        if (string.Compare(s, value, comparison_type) == 0) {
          return true;
        }
      }
      return false;
    }
  }
}
