using System;

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
    /// Returns the specified string, which encodes binary data as base-64
    /// digits, to an equivalent 8-bit unsigned integer array.
    /// </summary>
    /// <param name="base64_string">
    /// The string to convert.
    /// </param>
    /// <returns>
    /// An array of 8-bit unsigned integers that is equivalent to
    /// <paramref name="base64_string"/>
    /// </returns>
    /// <remarks>
    /// The order of bytes in the array returned by <see cref="AsBytes(long)"/>
    /// method depends whether the computer architecture is little-endian
    /// or big-endian.
    /// <para>
    /// This method is just a shortcut for:
    /// <code>
    /// Convert.FromBase64String();
    /// </code>
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="base64_string"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="FormatException">
    /// The length of <paramref name="base64_string"/>, ignoring white-space
    /// characters, is not zero or a multiple of 4
    /// <para>-or-</para>
    /// <para>
    /// The format of <paramref name="base64_string"/> is invalid.
    /// <paramref name="base64_string"/> contains a non base-64 character, more
    /// than two padding characters, or a non-white-space-character among the
    /// padding characters.
    /// </para>
    /// </exception>
    /// <seealso cref="Convert.FromBase64String"/>
    public static byte[] AsBytes(this string base64_string) {
      return Convert.FromBase64String(base64_string);
    }

    /// <summary>
    /// Converts an array of 8-bit unsigned integers to its equivalent string
    /// representation that is encoded with base-64 digits.
    /// </summary>
    /// <param name="value">
    /// An array of 8-bit unsigned integers.
    /// </param>
    /// <returns>
    /// The string representation, in base 64, of the contents of
    /// <paramref name="value"/>.
    /// </returns>
    /// <remarks>
    /// The order of bytes in the array returned by <see cref="AsBytes(long)"/>
    /// method depends whether the computer architecture is little-endian
    /// or big-endian.
    /// <para>
    /// This method is just a shor-cut for:
    /// <code>
    /// BitConverter.ToBase64String();
    /// </code>
    /// </para>
    /// </remarks>
    /// <seealso cref="Convert.ToBase64String(byte[])"/>
    public static string AsBase64String(this byte[] value) {
      return Convert.ToBase64String(value);
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
