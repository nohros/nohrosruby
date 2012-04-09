using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nohros.Ruby.Service.Net
{
  /// <summary>
  /// A basic implementation of the <see cref="IRubyMessage"/> interface.
  /// </summary>
  public class RubyMessage : IRubyMessage
  {
    IRubyMessageHeader header_;
    string message_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="RubyMessage"/> by using
    /// the specified message header and service specific message.
    /// </summary>
    /// <param name="header">The message header.</param>
    /// <param name="message">The service specific message.</param>
    public RubyMessage(IRubyMessageHeader header, string message) {
    }
    #endregion

    public IRubyMessageHeader Header {
      get { return header_; }
    }

    public string Message {
      get { return message_; }
    }
  }
}
