using Sodium.Interop;
using System.Runtime.InteropServices;

namespace Sodium
{
	public static class SodiumRandom
	{
		public static uint GetUInt32()
		{
			SodiumBindings.EnsureInitialized();
			return Libsodium.randombytes_random();
		}

		public static uint GetUInt32(uint upperBound)
		{
			SodiumBindings.EnsureInitialized();
			return Libsodium.randombytes_uniform(upperBound);
		}

		public static void Fill(Span<byte> buffer)
		{
			SodiumBindings.EnsureInitialized();
			Libsodium.randombytes_buf(buffer, (nuint)buffer.Length);
		}

		public static void Fill(Span<byte> buffer, ReadOnlySpan<byte> seed)
		{
			SodiumBindings.EnsureInitialized();
			if (seed.Length != Libsodium.randombytes_SEEDBYTES)
			{
				throw new ArgumentException($"seed must be {Libsodium.randombytes_SEEDBYTES} bytes in length", nameof(seed));
			}
			Libsodium.randombytes_buf_deterministic(buffer, (nuint)buffer.Length, seed);
		}

		public static void Close()
		{
			SodiumBindings.EnsureInitialized();
			if (Libsodium.randombytes_close() != 0)
			{
				throw new SodiumException("randombytes_close() failed");
			}
		}

		public static void Stir()
		{
			SodiumBindings.EnsureInitialized();
			Libsodium.randombytes_stir();
		}
	}
}
