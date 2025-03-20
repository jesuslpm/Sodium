using Sodium.Interop;

namespace Sodium
{
	/// <summary>
	/// Static class for random number generation.
	/// </summary>
	public static class SodiumRandom
	{
		/// <summary>
		/// Gets a random unsigned 32-bit integer.
		/// </summary>
		/// <returns>A random unsigned 32-bit integer.</returns>
		public static uint GetUInt32()
		{
			SodiumBindings.EnsureInitialized();
			return Libsodium.randombytes_random();
		}

		/// <summary>
		/// Gets a random unsigned 32-bit integer less than the specified upper bound.
		/// </summary>
		/// <param name="upperBound">The upper bound (exclusive) for the random number.</param>
		/// <returns>A random unsigned 32-bit integer less than upperBound.</returns>
		public static uint GetUInt32(uint upperBound)
		{
			SodiumBindings.EnsureInitialized();
			return Libsodium.randombytes_uniform(upperBound);
		}

		/// <summary>
		/// Fills the specified buffer with random bytes.
		/// </summary>
		/// <param name="buffer">The buffer to fill with random bytes.</param>
		public static void Fill(Span<byte> buffer)
		{
			SodiumBindings.EnsureInitialized();
			Libsodium.randombytes_buf(buffer, (nuint)buffer.Length);
		}

		/// <summary>
		/// Fills the specified buffer with deterministic random bytes based on the provided seed.
		/// </summary>
		/// <param name="buffer">The buffer to fill with deterministic random bytes.</param>
		/// <param name="seed">The seed used for deterministic random byte generation.</param>
		/// <exception cref="ArgumentException">Thrown when the seed length is not equal to Libsodium.randombytes_SEEDBYTES.</exception>
		public static void Fill(Span<byte> buffer, ReadOnlySpan<byte> seed)
		{
			SodiumBindings.EnsureInitialized();
			if (seed.Length != Libsodium.randombytes_SEEDBYTES)
			{
				throw new ArgumentException($"seed must be {Libsodium.randombytes_SEEDBYTES} bytes in length", nameof(seed));
			}
			Libsodium.randombytes_buf_deterministic(buffer, (nuint)buffer.Length, seed);
		}

		/// <summary>
		/// Closes the random number generator.
		/// </summary>
		/// <exception cref="SodiumException">Thrown when randombytes_close() fails.</exception>
		public static void Close()
		{
			SodiumBindings.EnsureInitialized();
			if (Libsodium.randombytes_close() != 0)
			{
				throw new SodiumException("randombytes_close() failed");
			}
		}

		/// <summary>
		/// Stirs the random number generator to ensure randomness.
		/// </summary>
		public static void Stir()
		{
			SodiumBindings.EnsureInitialized();
			Libsodium.randombytes_stir();
		}
	}
}
