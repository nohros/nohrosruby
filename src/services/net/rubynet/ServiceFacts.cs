using System;
using System.Collections;
using System.Collections.Generic;

namespace Nohros.Ruby
{
  public class ServiceFacts : IEnumerable<KeyValuePair<string, string>>
  {
    readonly Dictionary<string, string> facts_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceFacts"/> class.
    /// </summary>
    public ServiceFacts() {
      facts_ = new Dictionary<string, string>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceFacts"/> class
    /// that contains the facts copied from the specified collection.
    /// </summary>
    /// <param name="facts"></param>
    public ServiceFacts(IEnumerable<KeyValuePair<string, string>> facts) {
      facts_ = new Dictionary<string, string>();
      foreach (var fact in facts) {
        facts_.Add(fact.Key, fact.Value);
      }
    }
    #endregion

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
      foreach (var fact in facts_) {
        yield return new KeyValuePair<string, string>(fact.Key, fact.Value);
      }
    }
  }
}
