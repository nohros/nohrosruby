using System;
using Google.ProtocolBuffers;

namespace Nohros.Ruby.Extensions
{
  public static class ByteStringExtensions
  {
    /// <summary>
    /// Gets a 32-bit integer converted from four bytes at a specified position
    /// in a ByteString
    /// </summary>
    /// <param name="value">
    /// A <see cref="ByteString"/> object to convert.
    /// </param>
    /// <returns>
    /// A 32-bit signed integer formed by four bytes.
    /// </returns>
    public static int AsInt(this ByteString value) {
      return value.ToByteArray().AsInt();
    }

    /// <summary>
    /// Gets a 16-bit integer converted from four bytes at a specified position
    /// in a ByteString
    /// </summary>
    /// <param name="value">
    /// A <see cref="ByteString"/> object to convert.
    /// </param>
    /// <returns>
    /// A 16-bit signed integer formed by four bytes.
    /// </returns>
    public static short AsShort(this ByteString value) {
      return value.ToByteArray().AsShort();
    }

    /// <summary>
    /// Gets a 64-bit integer converted from four bytes at a specified position
    /// in a ByteString
    /// </summary>
    /// <param name="value">
    /// A <see cref="ByteString"/> object to convert.
    /// </param>
    /// <returns>
    /// A 64-bit signed integer.
    /// </returns>
    public static long AsLong(this ByteString value) {
      return value.ToByteArray().AsInt();
    }
  }
}
