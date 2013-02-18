using System;
using System.Collections.Generic;

namespace Nohros.Ruby
{
  public class FactSet
  {
    readonly Dictionary<string, Fact> facts_;

    #region .ctor
    public FactSet() {
      facts_ = new Dictionary<string, Fact>();
    }
    #endregion

    public void Add(Fact fact) {
      Fact f;
      if (!facts_.TryGetValue(fact.Name, out f)) {
        facts_.Add(fact.Name, fact);
      }
    }
  }
}
