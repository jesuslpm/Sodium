using Sodium.Interop;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Sodium
{
	/// <summary>Represents Base64 encoding variants.</summary>
	public enum Base64Variant
	{
		/// <summary>Original Base64 encoding variant.</summary>
		Original = Libsodium.sodium_base64_VARIANT_ORIGINAL,
		/// <summary>Original Base64 encoding variant with no padding.</summary>
		OriginalNoPadding = Libsodium.sodium_base64_VARIANT_ORIGINAL_NO_PADDING,
		/// <summary>URL safe Base64 encoding variant.</summary>
		UrlSafe = Libsodium.sodium_base64_VARIANT_URLSAFE,
		/// <summary>URL safe Base64 encoding variant with no padding.</summary>
		UrlSafeNoPadding = Libsodium.sodium_base64_VARIANT_URLSAFE_NO_PADDING,
	}


	public static class SodiumHelpers
	{
		/// <summary>
		/// Compares two byte buffers for equality in constant time.
		/// </summary>
		/// <param name="b1">first buffer to compare</param>
		/// <param name="b2">second buffer to compare</param>
		/// <returns>True if they are equals, false otherwise</returns>
		public static bool Equals(ReadOnlySpan<byte> b1, ReadOnlySpan<byte> b2)
		{
			SodiumBindings.EnsureInitialized();
			if (b1.Length != b2.Length)
			{
				return false;
			}
			return Libsodium.sodium_memcmp(b1, b2, (nuint)b1.Length) == 0;
		}

		/// <summary>
		/// Converts a byte buffer to a hexadecimal string in constant time for a given size.
		/// </summary>
		/// <param name="bin"></param>
		/// <returns></returns>
		public static string BinToHex(ReadOnlySpan<byte> bin)
		{
			SodiumBindings.EnsureInitialized();
			if (bin.Length == 0)
			{
				return string.Empty;
			}
			int hexAsciiBytesLen = bin.Length * 2 + 1;
			Span<byte> hexAsciiBytes = hexAsciiBytesLen <= Constants.MaxStackAlloc ? stackalloc byte[hexAsciiBytesLen] : new byte[hexAsciiBytesLen];
			var result = Libsodium.sodium_bin2hex(hexAsciiBytes, (nuint)hexAsciiBytes.Length, bin, (nuint)bin.Length);
			if (result == 0)
			{
				throw new SodiumException("sodium_bin2hex failed");
			}
			return Encoding.ASCII.GetString(hexAsciiBytes.Slice(0, hexAsciiBytes.Length - 1));
		}

		public static Span<char> BinToHex(ReadOnlySpan<byte> bin, Span<char> hex)
		{
			SodiumBindings.EnsureInitialized();
			if (hex.Length < bin.Length * 2)
			{
				throw new ArgumentException("hex buffer must be at least twice the size of the bin buffer");
			}
			if (bin.Length == 0)
			{
				return hex.Slice(0, 0);
			}
			int hexAsciiBytesLen = bin.Length * 2 + 1;
			Span<byte> hexAsciiBytes = hexAsciiBytesLen <= Constants.MaxStackAlloc ? stackalloc byte[hexAsciiBytesLen] : new byte[hexAsciiBytesLen];
			var result = Libsodium.sodium_bin2hex(hexAsciiBytes, (nuint)hex.Length, bin, (nuint)bin.Length);
			if (result == 0)
			{
				throw new SodiumException("sodium_bin2hex failed");
			}
			Encoding.ASCII.GetChars(hexAsciiBytes.Slice(0, hexAsciiBytesLen - 1), hex);
			return hex.Slice(0, hexAsciiBytesLen - 1);
		}

		public static Span<byte> HexToBin(string hex, Span<byte> bin, string? ignore = null)
		{
			return HexToBin(hex.AsSpan(), bin, ignore);
		}

		public static Span<byte> HexToBin(ReadOnlySpan<char> hex, Span<byte> bin, string? ignore = null)
		{
			SodiumBindings.EnsureInitialized();
			if (hex.Length == 0)
			{
				return bin.Slice(0, 0);
			}
			ignore ??= string.Empty;
			Span<byte> ignoreBytes = stackalloc byte[ignore.Length + 1];
			Encoding.ASCII.GetBytes(ignore.AsSpan(), ignoreBytes);
			Span<byte> hexBytes = hex.Length <= Constants.MaxStackAlloc ? stackalloc byte[hex.Length] : new byte[hex.Length];
			Encoding.ASCII.GetBytes(hex, hexBytes);

			if (Libsodium.sodium_hex2bin(bin, (nuint)bin.Length, hexBytes, (nuint)hex.Length, ignoreBytes, out nuint bin_len, nint.Zero) != 0)
			{
				throw new SodiumException("sodium_hex2bin failed because hex string contains invalid characters or the destination bin buffer is too short");
			}
			return bin.Slice(0, (int)bin_len);
		}

		public static int GetBase64DecodedMaxLen(int base64Len)
		{
			return base64Len * 3 / 4;
		}

		public static int GetBase64EncodedLen(int binLen, Base64Variant variant, bool includeNullTerminator = true)
		{
			SodiumBindings.EnsureInitialized();
			var len = (int)Libsodium.sodium_base64_encoded_len((nuint)binLen, (int)variant);
			return includeNullTerminator ? len : len - 1;
		}
		public static Span<byte> Base64ToBin(string b64, Span<byte> bin, Base64Variant variant, string? ignore = null)
		{
			return Base64ToBin(b64.AsSpan(), bin, variant, ignore);
		}

		public static Span<byte> Base64ToBin(ReadOnlySpan<char> b64, Span<byte> bin, Base64Variant variant, string? ignore = null)
		{
			SodiumBindings.EnsureInitialized();
			if (b64.Length == 0)
			{
				return bin.Slice(0, 0);
			}
			ignore ??= string.Empty;
			Span<byte> ignoreBytes = stackalloc byte[ignore.Length + 1];
			Encoding.ASCII.GetBytes(ignore.AsSpan(), ignoreBytes);
			Span<byte> b64AsciiBytes = b64.Length <= Constants.MaxStackAlloc ? stackalloc byte[b64.Length] : new byte[b64.Length];
			Encoding.ASCII.GetBytes(b64, b64AsciiBytes);
			if (Libsodium.sodium_base642bin(bin, (nuint)bin.Length, b64AsciiBytes, (nuint)b64.Length, ignoreBytes, out nuint bin_len, nint.Zero, (int)variant) != 0)
			{
				throw new SodiumException("sodium_base642bin failed because Base64 contains invalid characters or the destination bin buffer is too short");
			}
			return bin.Slice(0, (int)bin_len);
		}

		public static string BinToBase64(ReadOnlySpan<byte> bin, Base64Variant variant)
		{
			SodiumBindings.EnsureInitialized();
			if (bin.Length == 0)
			{
				return string.Empty;
			}
			int b64AsciiBytesLen = GetBase64EncodedLen(bin.Length, variant);
			Span<byte> b64AsciiBytes = b64AsciiBytesLen <= Constants.MaxStackAlloc ? stackalloc byte[b64AsciiBytesLen] : new byte[b64AsciiBytesLen];
			var result = Libsodium.sodium_bin2base64(b64AsciiBytes, (nuint)b64AsciiBytes.Length, bin, (nuint)bin.Length, (int)variant);
			if (result == 0)
			{
				throw new SodiumException("sodium_bin2base64 failed");
			}
			return Encoding.ASCII.GetString(b64AsciiBytes.Slice(0, b64AsciiBytesLen - 1));
		}

		public static Span<char> BinToBase64(ReadOnlySpan<byte> bin, Span<char> b64, Base64Variant variant)
		{
			SodiumBindings.EnsureInitialized();
			if (bin.Length == 0)
			{
				return b64.Slice(0, 0);
			}
			int b64AsciiBytesLen = GetBase64EncodedLen(bin.Length, variant);
			if (b64.Length < b64AsciiBytesLen - 1)
			{
				throw new ArgumentException("b64 buffer is too short", nameof(b64));
			}
			Span<byte> b64AsciiBytes = b64AsciiBytesLen <= Constants.MaxStackAlloc ? stackalloc byte[b64AsciiBytesLen] : new byte[b64AsciiBytesLen];
			var result = Libsodium.sodium_bin2base64(b64AsciiBytes, (nuint)b64AsciiBytes.Length, bin, (nuint)bin.Length, (int)variant);
			if (result == 0)
			{
				throw new SodiumException("sodium_bin2base64 failed");
			}
			Encoding.ASCII.GetChars(b64AsciiBytes.Slice(0, b64AsciiBytesLen - 1), b64);
			return b64.Slice(0, b64AsciiBytesLen - 1);
		}

		public static void Increment(Span<byte> number, ulong increment)
		{
			// sodium_increment doesn't work
			//if (increment > (ulong)nuint.MaxValue)
			//{
			//	throw new SodiumException($"increment must be less than or equal to {nuint.MaxValue}. But it is {increment}");
			//}
			//Libsodium.sodium_increment(number, (nuint)increment);

			//So Let's do it using Add
			SodiumBindings.EnsureInitialized();
			Span<byte> b = stackalloc byte[number.Length];
			if (BitConverter.IsLittleEndian)
			{
				Unsafe.WriteUnaligned(ref b[0], increment);
			}
			else
			{
				Unsafe.WriteUnaligned(ref b[0], BinaryPrimitives.ReverseEndianness(increment));
			}
			Add(number, b);
		}

		public static void Add(Span<byte> a, ReadOnlySpan<byte> b)
		{
			SodiumBindings.EnsureInitialized();
			if (a.Length != b.Length)
			{
				throw new ArgumentException("a and b must have the same length");
			}
			Libsodium.sodium_add(a, b, (nuint)a.Length);
		}

		public static void Subtract(Span<byte> subtrahend, ReadOnlySpan<byte> minuend)
		{
			SodiumBindings.EnsureInitialized();
			if (subtrahend.Length != minuend.Length)
			{
				throw new ArgumentException("subtrahend and minuend must have the same length");
			}
			Libsodium.sodium_sub(subtrahend, minuend, (nuint)subtrahend.Length);
		}

		public static int Compare(ReadOnlySpan<byte> b1, ReadOnlySpan<byte> b2)
		{
			SodiumBindings.EnsureInitialized();
			if (b1.Length != b2.Length)
			{
				throw new ArgumentException("b1 and b2 must have the same length");
			}
			return Libsodium.sodium_compare(b1, b2, (nuint)b1.Length);
		}

		public static bool IsZero(ReadOnlySpan<byte> b)
		{
			SodiumBindings.EnsureInitialized();
			return Libsodium.sodium_is_zero(b, (nuint)b.Length) == 1;
		}
	}
}
