using System;
using System.Collections.Generic;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// A <see cref="IRubyMessageSender"/> is an object that sends messages
  /// through an communication channel.
  /// </summary>
  public interface IRubyMessageSender
  {
    /// <summary>
    /// Sends a ruby message to the ruby service node.
    /// </summary>
    /// <param name="message">
    /// The message to send.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method is used to send a message to the service node that is
    /// hosting a service. It is usually used to send a reply to a messages
    /// that was successfully received and processed.
    /// <para>Ruby does not impose a
    /// strict request/reply message pattern. A received message could have
    /// or not a reply, the service that is receiving the message and the
    /// sender should establish a message pattern that is most suitable for its
    /// operation.
    /// </para>
    /// </remarks>
    bool Send(IRubyMessage message);

    /// <summary>
    /// Sends a ruby message to the ruby service node located at.
    /// </summary>
    /// <param name="message">
    /// The message to send.
    /// </param>
    /// <param name="facts">
    /// A collection of key value pairs containing the message facts.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method is used to send a message to the service node that is
    /// hosting a service. It is usually used to send a reply to a messages
    /// that was successfully received and processed.
    /// <para>Ruby does not impose a
    /// strict request/reply message pattern. A received message could have
    /// or not a reply, the service that is receiving the message and the
    /// sender should establish a message pattern that is most suitable for its
    /// operation.
    /// </para>
    /// </remarks>
    bool Send(IRubyMessage message,
      IEnumerable<KeyValuePair<string, string>> facts);

    /// <summary>
    /// Sends a ruby message to the ruby service.
    /// </summary>
    /// <param name="message">
    /// The encoded message to be sent.
    /// </param>
    /// <param name="type">
    /// A integer that identifies the type of the message.
    /// </param>
    /// <param name="message_id">
    /// The ID associated with the message.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method is used to send a message to the service node that is
    /// hosting a service. It is usually used to send a reply to a messages
    /// that was successfully received and processed.
    /// <para>
    /// Ruby does not impose a strict request/reply message pattern. A received
    /// message could have or not a reply, the service that is receiving the
    /// message and the sender should establish a message pattern that is most
    /// suitable for its operation.
    /// </para>
    /// <para>
    /// When sending a reply the destination address should be
    /// </para>
    /// </remarks>
    bool Send(byte[] message_id, int type, byte[] message);

    /// <summary>
    /// Sends a ruby message to the ruby service.
    /// </summary>
    /// <param name="message">
    /// The encoded message to be sent.
    /// </param>
    /// <param name="type">
    /// A integer that identifies the type of the message.
    /// </param>
    /// <param name="message_id">
    /// The ID associated with the message.
    /// </param>
    /// <param name="destination">
    /// The address of the message receiver or zero(0) if the message is
    /// destinated to the ruby service host.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method is used to send a message to the service node that is
    /// hosting a service. It is usually used to send a reply to a messages
    /// that was successfully received and processed.
    /// <para>
    /// Ruby does not impose a strict request/reply message pattern. A received
    /// message could have or not a reply, the service that is receiving the
    /// message and the sender should establish a message pattern that is most
    /// suitable for its operation.
    /// </para>
    /// <para>
    /// When sending a reply the destination address should be
    /// </para>
    /// </remarks>
    bool Send(byte[] message_id, int type, byte[] message, byte[] destination);

    /// <summary>
    /// Sends a ruby message to the ruby service.
    /// </summary>
    /// <param name="message">
    /// The encoded message to be sent.
    /// </param>
    /// <param name="type">
    /// A integer that identifies the type of the message.
    /// </param>
    /// <param name="message_id">
    /// The ID associated with the message.
    /// </param>
    /// <param name="facts">
    /// A collection of key value pairs containing the message facts.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method is used to send a message to the service node that is
    /// hosting a service. It is usually used to send a reply to a messages
    /// that was successfully received and processed.
    /// <para>
    /// Ruby does not impose a strict request/reply message pattern. A received
    /// message could have or not a reply, the service that is receiving the
    /// message and the sender should establish a message pattern that is most
    /// suitable for its operation.
    /// </para>
    /// <para>
    /// When sending a reply the destination address should be
    /// </para>
    /// </remarks>
    bool Send(byte[] message_id, int type, byte[] message,
      IEnumerable<KeyValuePair<string, string>> facts);

    /// <summary>
    /// Sends a ruby message to the ruby service.
    /// </summary>
    /// <param name="message">
    /// The encoded message to be sent.
    /// </param>
    /// <param name="type">
    /// A integer that identifies the type of the message.
    /// </param>
    /// <param name="message_id">
    /// The ID associated with the message.
    /// </param>
    /// <param name="destination">
    /// The address of the message receiver or zero(0) if the message is
    /// destinated to the ruby service host.
    /// </param>
    /// <param name="facts">
    /// A collection of key value pairs containing the message facts.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method is used to send a message to the service node that is
    /// hosting a service. It is usually used to send a reply to a messages
    /// that was successfully received and processed.
    /// <para>
    /// Ruby does not impose a strict request/reply message pattern. A received
    /// message could have or not a reply, the service that is receiving the
    /// message and the sender should establish a message pattern that is most
    /// suitable for its operation.
    /// </para>
    /// <para>
    /// When sending a reply the destination address should be
    /// </para>
    /// </remarks>
    bool Send(byte[] message_id, int type, byte[] message, byte[] destination,
      IEnumerable<KeyValuePair<string, string>> facts);

    /// <summary>
    /// Sends a ruby message to the ruby service.
    /// </summary>
    /// <param name="message">
    /// The encoded message to be sent.
    /// </param>
    /// <param name="type">
    /// A integer that identifies the type of the message.
    /// </param>
    /// <param name="message_id">
    /// The ID associated with the message.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <param name="token">
    /// A string that identifies the message type.
    /// </param>
    /// <remarks>
    /// This method is used to send a message to the service node that is
    /// hosting a service. It is usually used to send a reply to a messages
    /// that was successfully received and processed.
    /// <para>
    /// Ruby does not impose a strict request/reply message pattern. A received
    /// message could have or not a reply, the service that is receiving the
    /// message and the sender should establish a message pattern that is most
    /// suitable for its operation.
    /// </para>
    /// </remarks>
    bool Send(byte[] message_id, int type, byte[] message, string token);

    /// <summary>
    /// Sends a ruby message to the ruby service.
    /// </summary>
    /// <param name="message">
    /// The encoded message to be sent.
    /// </param>
    /// <param name="type">
    /// A integer that identifies the type of the message.
    /// </param>
    /// <param name="message_id">
    /// The ID associated with the message.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <param name="token">
    /// A string that identifies the message type.
    /// </param>
    /// <param name="destination">
    /// The address of the message receiver or zero(0) if the message is
    /// destinated to the ruby service host.
    /// </param>
    /// <remarks>
    /// This method is used to send a message to the service node that is
    /// hosting a service. It is usually used to send a reply to a messages
    /// that was successfully received and processed.
    /// <para>
    /// Ruby does not impose a strict request/reply message pattern. A received
    /// message could have or not a reply, the service that is receiving the
    /// message and the sender should establish a message pattern that is most
    /// suitable for its operation.
    /// </para>
    /// </remarks>
    bool Send(byte[] message_id, int type, byte[] message, string token,
      byte[] destination);

    /// <summary>
    /// Sends a ruby message to the ruby service.
    /// </summary>
    /// <param name="message">
    /// The encoded message to be sent.
    /// </param>
    /// <param name="type">
    /// A integer that identifies the type of the message.
    /// </param>
    /// <param name="message_id">
    /// The ID associated with the message.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <param name="token">
    /// A string that identifies the message type.
    /// </param>
    /// <param name="facts">
    /// A collection of key value pairs containing the message facts.
    /// </param>
    /// <remarks>
    /// This method is used to send a message to the service node that is
    /// hosting a service. It is usually used to send a reply to a messages
    /// that was successfully received and processed.
    /// <para>
    /// Ruby does not impose a strict request/reply message pattern. A received
    /// message could have or not a reply, the service that is receiving the
    /// message and the sender should establish a message pattern that is most
    /// suitable for its operation.
    /// </para>
    /// </remarks>
    bool Send(byte[] message_id, int type, byte[] message, string token,
      IEnumerable<KeyValuePair<string, string>> facts);

    /// <summary>
    /// Sends a ruby message to the ruby service.
    /// </summary>
    /// <param name="message">
    /// The encoded message to be sent.
    /// </param>
    /// <param name="type">
    /// A integer that identifies the type of the message.
    /// </param>
    /// <param name="message_id">
    /// The ID associated with the message.
    /// </param>
    /// <returns>
    /// <c>true</c> is the message was succesfully sent; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <param name="token">
    /// A string that identifies the message type.
    /// </param>
    /// <param name="destination">
    /// The address of the message receiver or zero(0) if the message is
    /// destinated to the ruby service host.
    /// </param>
    /// <param name="facts">
    /// A collection of key value pairs containing the message facts.
    /// </param>
    /// <remarks>
    /// This method is used to send a message to the service node that is
    /// hosting a service. It is usually used to send a reply to a messages
    /// that was successfully received and processed.
    /// <para>
    /// Ruby does not impose a strict request/reply message pattern. A received
    /// message could have or not a reply, the service that is receiving the
    /// message and the sender should establish a message pattern that is most
    /// suitable for its operation.
    /// </para>
    /// </remarks>
    bool Send(byte[] message_id, int type, byte[] message, string token,
      byte[] destination, IEnumerable<KeyValuePair<string, string>> facts);
  }
}
