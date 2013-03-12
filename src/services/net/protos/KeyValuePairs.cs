﻿using System;
using System.Collections.Generic;

namespace Nohros.Ruby
{
  /// <summary>
  /// Factory and utility methods for <see cref="KeyValuePair"/>.
  /// </summary>
  public sealed class KeyValuePairs
  {
    /// <summary>
    /// Creates an <see cref="KeyValuePair"/> object using the specified key
    /// and value strings.
    /// </summary>
    /// <param name="key">
    /// The key of the <see cref="KeyValuePair"/> object.
    /// </param>
    /// <param name="value">
    /// The value associated with the <paramref name="key"/>.
    /// </param>
    /// <returns>
    /// A <see cref="KeyValuePair"/> whose key is <paramref name="key"/> and
    /// value is <paramref name="value"/>.
    /// </returns>
    public static KeyValuePair FromKeyValuePair(string key, string value) {
      return new KeyValuePair.Builder()
        .SetKey(key)
        .SetValue(value)
        .Build();
    }

    /// <summary>
    /// Creates an <see cref="KeyValuePair"/> object using the specified
    /// <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <param name="key_value_pair">
    /// A <see cref="KeyValuePair{TKey,TValue}"/> object containing the key and
    /// value for the <see cref="KeyValuePair"/>.
    /// </param>
    /// <returns>
    /// A <see cref="KeyValuePair"/> whose key is
    /// <see cref="KeyValuePair{TKey,TValue}.Key"/> and value is
    /// <see cref="KeyValuePair{TKey,TValue}.Value"/>.
    /// </returns>
    public static KeyValuePair FromKeyValuePair(
      KeyValuePair<string, string> key_value_pair) {
      return FromKeyValuePair(key_value_pair.Key, key_value_pair.Value);
    }

    /// <summary>
    /// Creates an list of <see cref="KeyValuePair"/> objects using the
    /// specified collection of <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    public static IList<KeyValuePair> FromKeyValuePairs(
      IEnumerable<KeyValuePair<string, string>> key_value_pairs) {
      IList<KeyValuePair> pairs = new List<KeyValuePair>();
      foreach (KeyValuePair<string, string> key_value_pair in key_value_pairs) {
        pairs.Add(FromKeyValuePair(key_value_pair));
      }
      return pairs;
    }
  }
}