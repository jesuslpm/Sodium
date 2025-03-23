using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sodium.Sample
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine($"Major: {SodiumVersion.GetMajor()}, Minor: {SodiumVersion.GetMinor()}, Version string: {SodiumVersion.GetString()}");

			var r1 = SodiumRandom.GetUInt32();
			var r2 = SodiumRandom.GetUInt32(100);
			Span<byte> b1 = stackalloc byte[32];
			SodiumRandom.Fill(b1);
			Span<byte> b2 = stackalloc byte[32];
			SodiumRandom.FillDeterministic(b2, b1);
			SodiumRandom.Close();
			SodiumRandom.Stir();

			var hex = SodiumHexEncoding.BinToHex(b1);
			Console.WriteLine($"Hex: {hex}");
			Span<byte> bin = stackalloc byte[32];
			bin = SodiumHexEncoding.HexToBin(hex, bin);
			var areEquals = SodiumBigUInt.Equals(b1, bin);
			Console.WriteLine($"Are equals: {areEquals}");
			

			var len = SodiumBase64Encoding.GetBase64EncodedLen(1, Base64Variant.UrlSafeNoPadding, includeNullTerminator: false);
			Console.WriteLine($"Base64 encoded length: {len}");

			var b64 = SodiumBase64Encoding.BinToBase64(b1, Base64Variant.UrlSafeNoPadding);
			Console.WriteLine($"Base64: {b64}");
			Span<byte> b64Bin = stackalloc byte[32];
			b64Bin = SodiumBase64Encoding.Base64ToBin(b64, b64Bin, Base64Variant.UrlSafeNoPadding);
			areEquals = SodiumBigUInt.Equals(b1, b64Bin);
			Console.WriteLine($"Are equals: {areEquals}");

			var b64EncodedLen = SodiumBase64Encoding.GetBase64EncodedLen(32, Base64Variant.UrlSafeNoPadding, includeNullTerminator: false);
			Span<char> b64Chars = stackalloc char[b64EncodedLen];
			b64Chars = SodiumBase64Encoding.BinToBase64(b1, b64Chars, Base64Variant.UrlSafeNoPadding);

			Span<byte> numberBytes = stackalloc byte[16];
			Unsafe.WriteUnaligned(ref numberBytes[0], ulong.MaxValue);
			SodiumBigUInt.Increment(numberBytes, 1);
			var incrementedLow = Unsafe.ReadUnaligned<ulong>(ref numberBytes[0]);
			var incrementedHigh = Unsafe.ReadUnaligned<ulong>(ref numberBytes[8]);
			Debug.Assert(incrementedLow == 0ul && incrementedHigh == 1ul);


			Span<byte> a = stackalloc byte[32];
			Span<byte> b = stackalloc byte[32];
			Unsafe.WriteUnaligned(ref a[0], 65535);
			Unsafe.WriteUnaligned(ref b[0], 1);
			SodiumBigUInt.Add(a, b);
			var added = Unsafe.ReadUnaligned<ulong>(ref a[0]);
			Debug.Assert(added == 65536ul);

			Console.WriteLine("Completed successfully.");
		}
	}
}
