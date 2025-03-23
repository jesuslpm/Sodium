using TUnit.Assertions.AssertConditions.Throws;
using System.Text;

namespace Sodium.Tests
{
	public class SodiumBase64EncodingTests
	{
		const string hex = "01F9F847E44914D9EB3FF2AF42";
		private static byte[] bin = new byte[] { 1, 249, 248, 71, 228, 73, 20, 217, 235, 63, 242, 175, 66 };
		private const string base64Original = "Afn4R+RJFNnrP/KvQg==";
		private const string base64OriginalNoPadding = "Afn4R+RJFNnrP/KvQg";
		private const string base64UrlSafe = "Afn4R-RJFNnrP_KvQg==";
		private const string base64UrlSafeNoPadding = "Afn4R-RJFNnrP_KvQg";

		[Test]
		public async Task BinCorrespondToHex()
		{
			var binToHex = Convert.ToHexString(bin);
			await Assert.That(binToHex).IsEqualTo(hex);
		}

		[Test]
		public async Task GetBase64DecodedMaxLen_ReturnsCorrectLength()
		{
			int maxDecodedLen = SodiumBase64Encoding.GetBase64DecodedMaxLen(base64Original.Length);
			await Assert.That(maxDecodedLen).IsGreaterThanOrEqualTo(bin.Length);

			maxDecodedLen = SodiumBase64Encoding.GetBase64DecodedMaxLen(base64OriginalNoPadding.Length);
			await Assert.That(maxDecodedLen).IsGreaterThanOrEqualTo(bin.Length);
		}

		[Test]
		public async Task GetBase64EncodedLen_Original_ReturnsCorrectLength()
		{
			int encodedLen = SodiumBase64Encoding.GetBase64EncodedLen(bin.Length, Base64Variant.Original, includeNullTerminator: false);
			await Assert.That(encodedLen).IsEqualTo(base64Original.Length);
		}

		[Test]
		public async Task GetBase64EncodedLen_OriginalNoPadding_ReturnsCorrectLength()
		{
			int encodedLen = SodiumBase64Encoding.GetBase64EncodedLen(bin.Length, Base64Variant.OriginalNoPadding, includeNullTerminator: false);
			await Assert.That(encodedLen).IsEqualTo(base64OriginalNoPadding.Length);
		}

		[Test]
		public async Task GetBase64EncodedLen_UrlSafe_ReturnsCorrectLength()
		{
			int encodedLen = SodiumBase64Encoding.GetBase64EncodedLen(bin.Length, Base64Variant.UrlSafe, includeNullTerminator: false);
			await Assert.That(encodedLen).IsEqualTo(base64UrlSafe.Length);
		}

		[Test]
		public async Task GetBase64EncodedLen_UrlSafeNoPadding_ReturnsCorrectLength()
		{
			int encodedLen = SodiumBase64Encoding.GetBase64EncodedLen(bin.Length, Base64Variant.UrlSafeNoPadding, includeNullTerminator: false);
			await Assert.That(encodedLen).IsEqualTo(base64UrlSafeNoPadding.Length);
		}

		[Test]
		public async Task Base64ToBin_Original_DecodesCorrectly()
		{
			Span<byte> binaryBuffer = stackalloc byte[SodiumBase64Encoding.GetBase64DecodedMaxLen(base64Original.Length)];
			var decodedBinary = SodiumBase64Encoding.Base64ToBin(base64Original, binaryBuffer, Base64Variant.Original).ToArray();
			await Assert.That(decodedBinary).IsSequenceEqualTo(bin);
		}

		[Test]
		public async Task Base64ToBin_Original_WithIgnore_DecodesCorrectly()
		{
			string base64String = ":" + base64Original;
			Span<byte> binaryBuffer = stackalloc byte[SodiumBase64Encoding.GetBase64DecodedMaxLen(base64String.Length)];
			var decodedBinary = SodiumBase64Encoding.Base64ToBin(base64String, binaryBuffer, Base64Variant.Original, ":").ToArray();
			await Assert.That(decodedBinary).IsSequenceEqualTo(bin);
		}

		[Test]
		public async Task Base64ToBin_ThrowsSodiumException_OnInvalidBase64()
		{
			string invalidBase64 = "InvalidBase64!";
			byte[] binaryBuffer = new byte[SodiumBase64Encoding.GetBase64DecodedMaxLen(invalidBase64.Length)];
			await Assert.That(() => SodiumBase64Encoding.Base64ToBin(invalidBase64, binaryBuffer, Base64Variant.Original)).Throws<SodiumException>();
		}

		[Test]
		public async Task BinToBase64_Original_EncodesCorrectly()
		{
			string b64 = SodiumBase64Encoding.BinToBase64(bin, Base64Variant.Original);
			await Assert.That(b64).IsEqualTo(base64Original);
		}

		[Test]
		public async Task BinToBase64_Original_Span_EncodesCorrectly()
		{
			int b64Len = SodiumBase64Encoding.GetBase64EncodedLen(bin.Length, Base64Variant.Original);
			Span<char> b64CharBuffer = stackalloc char[b64Len];
			string b64 = SodiumBase64Encoding.BinToBase64(bin, b64CharBuffer, Base64Variant.Original).ToString();
			await Assert.That(b64).IsEqualTo(base64Original);
		}

		[Test]
		public async Task BinToBase64_ThrowsArgumentException_WhenBufferTooSmall()
		{
			byte[] binaryData = Encoding.ASCII.GetBytes("Hello World!");
			char[] b64CharBuffer = new char[1];

			await Assert.That(() => SodiumBase64Encoding.BinToBase64(binaryData, b64CharBuffer, Base64Variant.Original)).Throws<ArgumentException>();
		}

		[Test]
		public async Task BinToBase64_OriginalNoPadding()
		{
			string encodedBase64 = SodiumBase64Encoding.BinToBase64(bin, Base64Variant.OriginalNoPadding);
			await Assert.That(encodedBase64).IsEqualTo(base64OriginalNoPadding);
		}

		[Test]
		public async Task Base64ToBin_OriginalNoPadding()
		{
			byte[] binaryBuffer = new byte[SodiumBase64Encoding.GetBase64DecodedMaxLen(base64OriginalNoPadding.Length)];
			var decodedBinary = SodiumBase64Encoding.Base64ToBin(base64OriginalNoPadding, binaryBuffer, Base64Variant.OriginalNoPadding).ToArray();
			await Assert.That(decodedBinary).IsSequenceEqualTo(bin);
		}


        [Test]
        public async Task BinToBase64_UrlSafe()
        {
            string encodedBase64 = SodiumBase64Encoding.BinToBase64(bin, Base64Variant.UrlSafe);
            await Assert.That(encodedBase64).IsEqualTo(base64UrlSafe);
        }

        [Test]
        public async Task Base64ToBin_UrlSafe()
        {
            Span<byte> binaryBuffer = stackalloc byte[SodiumBase64Encoding.GetBase64DecodedMaxLen(base64UrlSafe.Length)];
            var decodedBinary = SodiumBase64Encoding.Base64ToBin(base64UrlSafe, binaryBuffer, Base64Variant.UrlSafe).ToArray();
            await Assert.That(decodedBinary).IsSequenceEqualTo(bin);
        }

        [Test]
        public async Task BinToBase64_UrlSafeNoPadding()
        {
            string encodedBase64 = SodiumBase64Encoding.BinToBase64(bin, Base64Variant.UrlSafeNoPadding);
            await Assert.That(encodedBase64).IsEqualTo(base64UrlSafeNoPadding);
        }

        [Test]
        public async Task Base64ToBin_UrlSafeNoPadding()
        {
            Span<byte> binaryBuffer = stackalloc byte[SodiumBase64Encoding.GetBase64DecodedMaxLen(base64UrlSafeNoPadding.Length)];
            var decodedBinary = SodiumBase64Encoding.Base64ToBin(base64UrlSafeNoPadding, binaryBuffer, Base64Variant.UrlSafeNoPadding).ToArray();
            await Assert.That(decodedBinary).IsSequenceEqualTo(bin);
        }

		[Test]
		public async Task BinToBase64_EmptyBinary_ReturnsEmptyString()
		{
			byte[] emptyBin = Array.Empty<byte>();
			string emptyBase64 = SodiumBase64Encoding.BinToBase64(emptyBin, Base64Variant.Original);
			await Assert.That(emptyBase64).IsEqualTo(string.Empty);
		}
	}
}