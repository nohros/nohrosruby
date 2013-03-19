using System;
using System.Collections.Generic;

namespace Nohros.Ruby.Extensions
{
  public static class KeyValuePairsExtension
  {
    public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(
      this IEnumerable<KeyValuePair> pairs) {
      return KeyValuePairs.ToKeyValuePairs(pairs);
    }

    public static IEnumerable<KeyValuePair> FromKeyValuePairs(
      this IEnumerable<KeyValuePair<string, string>> pairs) {
      return KeyValuePairs.FromKeyValuePairs(pairs);
    }
  }
}
