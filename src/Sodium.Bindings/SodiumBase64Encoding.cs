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


	public static class SodiumBase64Encoding
	{

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
	}
}
