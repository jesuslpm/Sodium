using Sodium.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodium
{
	public static class SodiumHexEncoding
	{
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
	}
}
