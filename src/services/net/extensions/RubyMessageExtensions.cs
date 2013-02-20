using System;
using System.Text;

namespace Nohros.Ruby.Extensions
{
  public static class RubyMessageExtension
  {
    /// <summary>
    /// Returns the specified 64-bit signed integer valus as an array of bytes.
    /// </summary>
    /// <param name="value">
    /// The number to convert.
    /// </param>
    /// <returns>
    /// The specified 64-bit signed integer valus as an array of bytes.
    /// </returns>
    /// <remarks>
    /// The order of bytes in the array returned by <see cref="AsBytes(long)"/>
    /// method depends whether the computer architecture is little-endian
    /// or big-endian.
    /// <para>
    /// This method is just a shortcut for:
    /// <code>
    /// BitConverter.GetBytes();
    /// </code>
    /// </para>
    /// </remarks>
    public static byte[] AsBytes(this long value) {
      return BitConverter.GetBytes(value);
    }

    /// <summary>
    /// Returns the specified 32-bit signed integer valus as an array of bytes.
    /// </summary>
    /// <param name="value">
    /// The number to convert.
    /// </param>
    /// <returns>
    /// The specified 32-bit signed integer valus as an array of bytes.
    /// </returns>
    /// <remarks>
    /// The order of bytes in the array returned by <see cref="AsBytes(int)"/>
    /// method depends whether the computer architecture is little-endian
    /// or big-endian.
    /// <para>
    /// This method is just a shortcut for:
    /// <code>
    /// BitConverter.GetBytes();
    /// </code>
    /// </para>
    /// </remarks>
    public static byte[] AsBytes(this int value) {
      return BitConverter.GetBytes(value);
    }

    /// <summary>
    /// Returns the specified 16-bit signed integer valus as an array of bytes.
    /// </summary>
    /// <param name="value">
    /// The number to convert.
    /// </param>
    /// <returns>
    /// The specified 16-bit signed integer valus as an array of bytes.
    /// </returns>
    /// <remarks>
    /// The order of bytes in the array returned by <see cref="AsBytes(short)"/>
    /// method depends whether the computer architecture is little-endian
    /// or big-endian.
    /// <para>
    /// This method is just a shortcut for:
    /// <code>
    /// BitConverter.GetBytes();
    /// </code>
    /// </para>
    /// </remarks>
    public static byte[] AsBytes(this short value) {
      return BitConverter.GetBytes(value);
    }

    /// <summary>
    /// Encodes the specified string using the <see cref="Encoding.Unicode"/>
    /// encoding.
    /// </summary>
    /// <param name="str">
    /// The string to convert.
    /// </param>
    /// <returns>
    /// A byte array containing the results of encoding the specified set of
    /// characters using the <see cref="Encoding.Unicode"/>.
    /// </returns>
    /// <seealso cref="Convert.FromBase64String"/>
    public static byte[] AsBytes(this string str) {
      return Encoding.Unicode.GetBytes(str);
    }

    /// <summary>
    /// Returns the specified string, encoded as a sequence of bytes.
    /// </summary>
    /// <param name="str">
    /// The string containing the characters to encode.
    /// </param>
    /// <param name="encoding">
    /// A <see cref="Encoding"/> object that can be used to encode a set of
    /// characters.
    /// </param>
    /// <returns>
    /// A byte array containing the results of encoding the specified set of
    /// characters.
    /// </returns>
    public static byte[] AsBytes(this string str, Encoding encoding) {
      return encoding.GetBytes(str);
    }

    /// <summary>
    /// Decodes all the bytes in the specified byte array into a string.
    /// </summary>
    /// <param name="value">
    /// The byte array containing the sequence of bytes to decode.
    /// </param>
    /// <param name="encoding">
    /// A <see cref="Encoding"/> object that can be used to decode
    /// <paramref name="value"/>.
    /// </param>
    /// <returns>
    /// A string containing the results of decoding the specified sequence of
    /// bytes.
    /// </returns>
    public static string AsString(this byte[] value, Encoding encoding) {
      return encoding.GetString(value);
    }

    /// <summary>
    /// Decodes all the bytes in the specified byte array into a string
    /// using the <see cref="Encoding.Unicode"/> encoding.
    /// </summary>
    /// <param name="value">
    /// The byte array containing the sequence of bytes to decode.
    /// </param>
    /// <returns>
    /// A string containing the results of decoding the specified sequence of
    /// bytes.
    /// </returns>
    public static string AsString(this byte[] value) {
      return Encoding.Unicode.GetString(value);
    }

    /// <summary>
    /// Gets a 32-bit integer converted from four bytes at a specified position
    /// in a byte array.
    /// </summary>
    /// <param name="value">
    /// An array of bytes.
    /// </param>
    /// <returns>
    /// A 32-bit signed integer formed by four bytes beginning at position
    /// zero.
    /// </returns>
    public static int AsInt(this byte[] value) {
      return value.AsInt(0);
    }

    /// <summary>
    /// Gets a 32-bit integer converted from four bytes at a specified position
    /// in a byte array.
    /// </summary>
    /// <param name="value">
    /// An array of bytes.
    /// </param>
    /// <param name="index">
    /// The starting position within <paramref name="value"/>
    /// </param>
    /// <returns>
    /// A 32-bit signed integer formed by four bytes beginning at
    /// <paramref name="index"/>.
    /// </returns>
    public static int AsInt(this byte[] value, int index) {
      return BitConverter.ToInt32(value, index);
    }

    /// <summary>
    /// Gets a 16-bit integer converted from four bytes at a specified position
    /// in a byte array.
    /// </summary>
    /// <param name="value">
    /// An array of bytes.
    /// </param>
    /// <returns>
    /// A 16-bit signed integer formed by four bytes beginning at position
    /// zero.
    /// </returns>
    public static short AsShort(this byte[] value) {
      return value.AsShort(0);
    }

    /// <summary>
    /// Gets a 16-bit integer converted from four bytes at a specified position
    /// in a byte array.
    /// </summary>
    /// <param name="value">
    /// An array of bytes.
    /// </param>
    /// <param name="index">
    /// The starting position within <paramref name="value"/>
    /// </param>
    /// <returns>
    /// A 16-bit signed integer formed by four bytes beginning at
    /// <paramref name="index"/>.
    /// </returns>
    public static short AsShort(this byte[] value, int index) {
      return BitConverter.ToInt16(value, 0);
    }

    /// <summary>
    /// Gets a 64-bit integer converted from four bytes at a specified position
    /// in a byte array.
    /// </summary>
    /// <param name="value">
    /// An array of bytes.
    /// </param>
    /// <returns>
    /// A 64-bit signed integer formed by four bytes beginning at position
    /// zero.
    /// </returns>
    public static long AsLong(this byte[] value) {
      return value.AsLong(0);
    }

    /// <summary>
    /// Gets a 64-bit integer converted from four bytes at a specified position
    /// in a byte array.
    /// </summary>
    /// <param name="value">
    /// An array of bytes.
    /// </param>
    /// <param name="index">
    /// The starting position within <paramref name="value"/>
    /// </param>
    /// <returns>
    /// A 64-bit signed integer formed by four bytes beginning at
    /// <paramref name="index"/>.
    /// </returns>
    public static long AsLong(this byte[] value, int index) {
      return BitConverter.ToInt64(value, index);
    }
  }
}
