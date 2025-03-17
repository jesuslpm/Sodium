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
			SodiumRandom.Fill(b2, b1);
			SodiumRandom.Close();
			SodiumRandom.Stir();

			var hex = SodiumHelpers.BinToHex(b1);
			Console.WriteLine($"Hex: {hex}");
			Span<byte> bin = stackalloc byte[32];
			bin = SodiumHelpers.HexToBin(hex, bin);
			var areEquals = SodiumHelpers.Equals(b1, bin);
			Console.WriteLine($"Are equals: {areEquals}");
			

			var len = SodiumHelpers.GetBase64EncodedLen(1, Base64Variant.UrlSafeNoPadding, includeNullTerminator: false);
			Console.WriteLine($"Base64 encoded length: {len}");

			var b64 = SodiumHelpers.BinToBase64(b1, Base64Variant.UrlSafeNoPadding);
			Console.WriteLine($"Base64: {b64}");
			Span<byte> b64Bin = stackalloc byte[32];
			b64Bin = SodiumHelpers.Base64ToBin(b64, b64Bin, Base64Variant.UrlSafeNoPadding);
			areEquals = SodiumHelpers.Equals(b1, b64Bin);
			Console.WriteLine($"Are equals: {areEquals}");

			var b64EncodedLen = SodiumHelpers.GetBase64EncodedLen(32, Base64Variant.UrlSafeNoPadding, includeNullTerminator: false);
			Span<char> b64Chars = stackalloc char[b64EncodedLen];
			b64Chars = SodiumHelpers.BinToBase64(b1, b64Chars, Base64Variant.UrlSafeNoPadding);

			Span<byte> numberBytes = stackalloc byte[16];
			Unsafe.WriteUnaligned(ref numberBytes[0], ulong.MaxValue);
			SodiumHelpers.Increment(numberBytes, 1);
			var incrementedLow = Unsafe.ReadUnaligned<ulong>(ref numberBytes[0]);
			var incrementedHigh = Unsafe.ReadUnaligned<ulong>(ref numberBytes[8]);
			Debug.Assert(incrementedLow == 0ul && incrementedHigh == 1ul);


			Span<byte> a = stackalloc byte[32];
			Span<byte> b = stackalloc byte[32];
			Unsafe.WriteUnaligned(ref a[0], 65535);
			Unsafe.WriteUnaligned(ref b[0], 1);
			SodiumHelpers.Add(a, b);
			var added = Unsafe.ReadUnaligned<ulong>(ref a[0]);
			Debug.Assert(added == 65536ul);

			Console.WriteLine("Completed successfully.");
		}
	}
}
