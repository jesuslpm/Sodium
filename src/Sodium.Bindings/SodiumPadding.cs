using Sodium.Interop;

namespace Sodium
{
	public static class SodiumPadding
	{
		public static Span<byte> Pad(Span<byte> buffer, int unpaddedLen, int blockSize)
		{
			SodiumBindings.EnsureInitialized();
			if (blockSize <= 0)
			{
				throw new ArgumentException("block_size must be greater than 0");
			}
			if (unpaddedLen > buffer.Length)
			{
				throw new ArgumentException("unpadded_len must be less than or equal to buffer.Length");
			}
			if (Libsodium.sodium_pad(out nuint padded_len, buffer, (nuint) unpaddedLen, (nuint)blockSize, (nuint) buffer.Length) != 0)
			{
				throw new ArgumentException("Padding failed because the buffer is too short");
			}
			return buffer.Slice(0, (int)padded_len);
		}

		public static Span<byte> Unpad(Span<byte> buffer, int blockSize)
		{
			SodiumBindings.EnsureInitialized();
			if (blockSize <= 0)
			{
				throw new ArgumentException("block_size must be greater than 0");
			}
			if (Libsodium.sodium_unpad(out nint unpadded_len, buffer, (nuint)buffer.Length, (nuint)blockSize) != 0)
			{
				throw new SodiumException("Unpadding failed");
			}
			return buffer.Slice(0, (int)unpadded_len);
		}
	}
}
