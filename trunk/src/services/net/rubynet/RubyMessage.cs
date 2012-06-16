using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// An implementation of the <see cref="IRubyMessage"/> interface.
  /// </summary>
  public partial class RubyMessage : IRubyMessage
  {
    byte[] IRubyMessage.Message { get { return Message.ToByteArray(); } }

    IFact[] IRubyMessage.Facts {
      get {
        IFact[] facts = new IFact[FactsCount];
        for (int i = 0; i < FactsCount; i++) {
          facts[i] = GetFacts(i);
        }
        return facts;
      }
    }
  }
}
