using TUnit.Assertions;
using TUnit.Core;
using Sodium;
using System;
using TUnit.Assertions.AssertConditions.Throws;

namespace Sodium.Tests
{
	public class SodiumSecretBoxTests
	{
		private static byte[] GenerateRandomPlainText()
		{
			var plaintextLen = 32 + SodiumRandom.GetUInt32(upperBound: 16);
			var plaintext = new byte[plaintextLen];
			SodiumRandom.Fill(plaintext);
			return plaintext;
		}

		[Test]
		public async Task EncryptCombined_DecryptCombined_Success()
		{
			Span<byte> key = stackalloc byte[SodiumSecretBox.KeyLen];
			Span<byte> nonce = stackalloc byte[SodiumSecretBox.NonceLen];
			SodiumRandom.Fill(key);
			SodiumRandom.Fill(nonce);

			var plaintext = GenerateRandomPlainText();
			Span<byte> ciphertextBuffer = stackalloc byte[plaintext.Length + SodiumSecretBox.MacLen];

			var ciphertext = SodiumSecretBox.EncryptCombined(ciphertextBuffer, plaintext, key, nonce);
			var ciphertextLen = ciphertext.Length;
			Span<byte> decryptedBuffer = stackalloc byte[plaintext.Length];
			var decrypted = SodiumSecretBox.DecryptCombined(decryptedBuffer, ciphertext, key, nonce).ToArray();

			await Assert.That(ciphertextLen).IsEqualTo(plaintext.Length + SodiumSecretBox.MacLen);
			await Assert.That(decrypted).IsSequenceEqualTo(plaintext);
		}

		[Test]
		public async Task EncryptCombined_AutoNonce_DecryptCombined_AutoNonce_Success()
		{
			Span<byte> key = stackalloc byte[SodiumSecretBox.KeyLen];
			SodiumRandom.Fill(key);

			var plaintext = GenerateRandomPlainText();
			Span<byte> ciphertextBuffer = stackalloc byte[plaintext.Length + SodiumSecretBox.MacLen + SodiumSecretBox.NonceLen];

			var ciphertext = SodiumSecretBox.EncryptCombined(ciphertextBuffer, plaintext, key);
			Span<byte> decryptedBuffer = stackalloc byte[plaintext.Length];
			var decrypted = SodiumSecretBox.DecryptCombined(decryptedBuffer, ciphertext, key).ToArray();

			await Assert.That(decrypted).IsSequenceEqualTo(plaintext);
		}

		[Test]
		public async Task EncryptDetached_DecryptDetached_Success()
		{
			Span<byte> key = stackalloc byte[SodiumSecretBox.KeyLen];
			Span<byte> nonce = stackalloc byte[SodiumSecretBox.NonceLen];
			SodiumRandom.Fill(key);
			SodiumRandom.Fill(nonce);

			var plaintext = GenerateRandomPlainText();
			Span<byte> ciphertextBuffer = stackalloc byte[plaintext.Length];
			Span<byte> macBuffer = stackalloc byte[SodiumSecretBox.MacLen];

			var ciphertext = SodiumSecretBox.EncryptDetached(ciphertextBuffer, macBuffer, plaintext, key, nonce);
			Span<byte> decryptedBuffer = stackalloc byte[plaintext.Length];
			var decrypted = SodiumSecretBox.DecryptDetached(decryptedBuffer, ciphertext, key, macBuffer, nonce).ToArray();

			await Assert.That(decrypted).IsSequenceEqualTo(plaintext);
		}

		[Test]
		public async Task EncryptDetached_AutoNonce_DecryptDetached_AutoNonce_Success()
		{
			Span<byte> key = stackalloc byte[SodiumSecretBox.KeyLen];
			SodiumRandom.Fill(key);

			var plaintext = GenerateRandomPlainText();
			Span<byte> ciphertextBuffer = stackalloc byte[plaintext.Length + SodiumSecretBox.NonceLen];
			Span<byte> macBuffer = stackalloc byte[SodiumSecretBox.MacLen];

			var ciphertext = SodiumSecretBox.EncryptDetached(ciphertextBuffer, macBuffer, plaintext, key);
			Span<byte> decryptedBuffer = stackalloc byte[plaintext.Length];
			var decrypted = SodiumSecretBox.DecryptDetached(decryptedBuffer, ciphertext, key, macBuffer).ToArray();

			await Assert.That(decrypted).IsSequenceEqualTo(plaintext);
		}

		[Test]
		public async Task EncryptCombined_InvalidCiphertextBuffer_ThrowsArgumentException()
		{
			byte[] key = new byte[SodiumSecretBox.KeyLen];
			byte[] nonce = new byte[SodiumSecretBox.NonceLen];
			SodiumRandom.Fill(key);
			SodiumRandom.Fill(nonce);

			var plaintext = GenerateRandomPlainText();
			byte[] ciphertextBuffer = new byte[plaintext.Length + SodiumSecretBox.MacLen - 1]; // Buffer too small

			await Assert.That(() => SodiumSecretBox.EncryptCombined(ciphertextBuffer, plaintext, key, nonce)).Throws<ArgumentException>();
		}

		[Test]
		public async Task EncryptCombined_InvalidKeyLength_ThrowsArgumentException()
		{
			byte[] key = new byte[SodiumSecretBox.KeyLen - 1];
			byte[] nonce = new byte[SodiumSecretBox.NonceLen];
			SodiumRandom.Fill(nonce);

			var plaintext = GenerateRandomPlainText();
			byte[] ciphertextBuffer = new byte[plaintext.Length + SodiumSecretBox.MacLen];

			await Assert.That(() => SodiumSecretBox.EncryptCombined(ciphertextBuffer, plaintext, key, nonce)).Throws<ArgumentException>();
		}

		[Test]
		public async Task EncryptCombined_InvalidNonceLength_ThrowsArgumentException()
		{
			byte[] key = new byte[SodiumSecretBox.KeyLen];
			byte[] nonce = new byte[SodiumSecretBox.NonceLen - 1];
			SodiumRandom.Fill(key);

			var plaintext = GenerateRandomPlainText();
			byte[] ciphertextBuffer = new byte[plaintext.Length + SodiumSecretBox.MacLen];

			await Assert.That(() => SodiumSecretBox.EncryptCombined(ciphertextBuffer, plaintext, key, nonce)).Throws<ArgumentException>();
		}

		[Test]
		public async Task DecryptCombined_InvalidCiphertextLength_ThrowsArgumentException()
		{
			byte[] key = new byte[SodiumSecretBox.KeyLen];
			byte[] nonce = new byte[SodiumSecretBox.NonceLen];
			SodiumRandom.Fill(key);
			SodiumRandom.Fill(nonce);

			byte[] ciphertextBuffer = new byte[SodiumSecretBox.MacLen - 1]; // Buffer too small
			byte[] plaintextBuffer = new byte[10];

			await Assert.That(() => SodiumSecretBox.DecryptCombined(plaintextBuffer, ciphertextBuffer, key, nonce)).Throws<ArgumentException>();
		}

		[Test]
		public async Task DecryptDetached_InvalidMacLength_ThrowsArgumentException()
		{
			byte[] key = new byte[SodiumSecretBox.KeyLen];
			byte[] nonce = new byte[SodiumSecretBox.NonceLen];
			SodiumRandom.Fill(key);
			SodiumRandom.Fill(nonce);

			byte[] ciphertextBuffer = new byte[10];
			byte[] macBuffer = new byte[SodiumSecretBox.MacLen - 1]; // mac too short
			byte[] plaintextBuffer = new byte[10];

			await Assert.That(() => SodiumSecretBox.DecryptDetached(plaintextBuffer, ciphertextBuffer, key, macBuffer, nonce)).Throws<ArgumentException>();
		}

		[Test]
		public async Task DecryptCombined_TamperedCiphertext_ThrowsSodiumException()
		{
			byte[] key = new byte[SodiumSecretBox.KeyLen];
			byte[] nonce = new byte[SodiumSecretBox.NonceLen];
			SodiumRandom.Fill(key);
			SodiumRandom.Fill(nonce);

			var plaintext = GenerateRandomPlainText();
			byte[] ciphertextBuffer = new byte[plaintext.Length + SodiumSecretBox.MacLen];

			var ciphertext = SodiumSecretBox.EncryptCombined(ciphertextBuffer, plaintext, key, nonce).ToArray(); // Convert to Array to be safe.

			// Tamper with the ciphertext by flipping a bit
			ciphertext[5] ^= 0b00000001; // Flip the 1st bit of the 6th byte

			byte[] decryptedBuffer = new byte[plaintext.Length];

			await Assert.That(() => SodiumSecretBox.DecryptCombined(decryptedBuffer, ciphertext, key, nonce)).Throws<SodiumException>();
		}

		[Test]
		public async Task DecryptCombined_AutoNonce_TamperedCiphertext_ThrowsSodiumException()
		{
			byte[] key = new byte[SodiumSecretBox.KeyLen];
			SodiumRandom.Fill(key);

			var plaintext = GenerateRandomPlainText();
			byte[] ciphertextBuffer = new byte[plaintext.Length + SodiumSecretBox.MacLen + SodiumSecretBox.NonceLen];

			var ciphertext = SodiumSecretBox.EncryptCombined(ciphertextBuffer, plaintext, key).ToArray(); // Convert to Array to be safe.

			// Tamper with the ciphertext by changing a byte
			ciphertext[SodiumSecretBox.NonceLen + 10] ^= 0xFF; // Change the 11th byte after nonce

			byte[] decryptedBuffer = new byte[plaintext.Length];

			await Assert.That(() => SodiumSecretBox.DecryptCombined(decryptedBuffer, ciphertext, key)).Throws<SodiumException>();
		}

		[Test]
		public async Task DecryptDetached_TamperedCiphertext_ThrowsSodiumException()
		{
			byte[] key = new byte[SodiumSecretBox.KeyLen];
			byte[] nonce = new byte[SodiumSecretBox.NonceLen];
			SodiumRandom.Fill(key);
			SodiumRandom.Fill(nonce);

			var plaintext = GenerateRandomPlainText();
			byte[] ciphertextBuffer = new byte[plaintext.Length];
			byte[] macBuffer = new byte[SodiumSecretBox.MacLen];

			var ciphertext = SodiumSecretBox.EncryptDetached(ciphertextBuffer, macBuffer, plaintext, key, nonce).ToArray(); // Convert to array.

			// Tamper with the ciphertext by flipping a bit
			ciphertext[15] ^= 0b00000001; // Flip the 1st bit of the 16th byte

			byte[] decryptedBuffer = new byte[plaintext.Length];

			await Assert.That(() => SodiumSecretBox.DecryptDetached(decryptedBuffer, ciphertext, key, macBuffer, nonce)).Throws<SodiumException>();
		}

		[Test]
		public async Task DecryptDetached_AutoNonce_TamperedCiphertext_ThrowsSodiumException()
		{
			byte[] key = new byte[SodiumSecretBox.KeyLen];
			SodiumRandom.Fill(key);

			var plaintext = GenerateRandomPlainText();
			byte[] ciphertextBuffer = new byte[plaintext.Length + SodiumSecretBox.NonceLen];
			byte[] macBuffer = new byte[SodiumSecretBox.MacLen];

			var ciphertext = SodiumSecretBox.EncryptDetached(ciphertextBuffer, macBuffer, plaintext, key).ToArray(); // Convert to array.

			// Tamper with the ciphertext by changing a byte
			ciphertext[SodiumSecretBox.NonceLen + 20] ^= 0xFF; // Change the 21th byte after nonce

			byte[] decryptedBuffer = new byte[plaintext.Length];

			await Assert.That(() => SodiumSecretBox.DecryptDetached(decryptedBuffer, ciphertext, key, macBuffer)).Throws<SodiumException>();
		}
	}
}