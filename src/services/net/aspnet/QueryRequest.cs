using System;
using Google.ProtocolBuffers;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  public class QueryRequest
  {
    readonly byte[] id_;
    readonly ByteString message_;
    readonly string message_token_;
    readonly int message_type_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryRequest"/> class by
    /// using the specified message id, message and message type.
    /// </summary>
    /// <param name="id">
    /// A sequence of bytes that could be used to identify the message.
    /// </param>
    /// <param name="message">
    /// A <see cref="ByteString"/> containing the message contents.
    /// </param>
    /// <param name="message_type">
    /// A number that identifies the message's type.
    /// </param>
    public QueryRequest(byte[] id, ByteString message, int message_type)
      : this(id, message, message_type, string.Empty) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryRequest"/> class by
    /// using the specified message id, message and message type.
    /// </summary>
    /// <param name="id">
    /// A sequence of bytes that could be used to identify the message.
    /// </param>
    /// <param name="message">
    /// A <see cref="ByteString"/> containing the message contents.
    /// </param>
    /// <param name="message_type">
    /// A number that identifies the message's type.
    /// </param>
    /// <param name="message_token">
    /// A string that identifies the message's type.
    /// </param>
    public QueryRequest(byte[] id, ByteString message, int message_type,
      string message_token) {
      id_ = id;
      message_ = message;
      message_type_ = message_type;
      message_token_ = message_token;
      AckType = RubyMessage.Types.AckType.kRubyNoAck;
    }
    #endregion

    /// <summary>
    /// Gets a sequence of bytes that identifies the message.
    /// </summary>
    public byte[] ID {
      get { return id_; }
    }

    /// <summary>
    /// Gets a <see cref="ByteString"/> representing the message to be sent.
    /// </summary>
    public ByteString Message {
      get { return message_; }
    }

    /// <summary>
    /// Gets a string that identifies the type of message that should be sent.
    /// </summary>
    public string MessageToken {
      get { return message_token_; }
    }

    /// <summary>
    /// Gets an integer taht identifies the type of message that should be sent.
    /// </summary>
    public int MessageType {
      get { return message_type_; }
    }

    /// <summary>
    /// Gets pr sets the type of acknowledge wants to receive.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="RubyMessage.Types.AckType.kRubyNoAck"/>
    /// </remarks>
    public RubyMessage.Types.AckType AckType { get; set; }
  }
}
