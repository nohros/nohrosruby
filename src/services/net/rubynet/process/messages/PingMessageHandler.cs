using System;
using System.Collections.Generic;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;

namespace Nohros.Ruby
{
  internal class PingMessageHandler
  {
    const string kPongMessageToken = Strings.kPongMessageToken;

    readonly IRubyMessageChannel ruby_message_channel_;

    #region .ctor
    public PingMessageHandler(IRubyMessageChannel ruby_message_channel) {
      ruby_message_channel_ = ruby_message_channel;
    }
    #endregion

    public void Handle(RubyMessagePacket packet) {
      RubyMessage message = new RubyMessage.Builder()
        .SetToken(kPongMessageToken)
        .SetType((int) NodeMessageType.kNodePong)
        .SetId(packet.Message.Id)
        .SetSender(packet.Message.Sender)
        .Build();
      ruby_message_channel_.Send(message, new[] {
        new KeyValuePair<string, string>(RubyStrings.kServiceNameFact,
          Strings.kNodeServiceName)
      });
    }
  }
}
