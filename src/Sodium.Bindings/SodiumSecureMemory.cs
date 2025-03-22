using Sodium.Interop;
using System.Runtime.InteropServices;

namespace Sodium
{
	/// <summary>
	/// Provides methods for secure memory management using libsodium.
	/// These methods help protect sensitive data from being swapped to disk or accessed by other processes.
	/// </summary>
	public static partial class SodiumSecureMemory
	{
		/// <summary>
		/// Fills a buffer with zeros, effectively erasing its contents.
		/// </summary>
		/// <param name="buf">The span of bytes to zero out.</param>
		public static void MemZero(Span<byte> buf)
		{
			SodiumBindings.EnsureInitialized();
			Libsodium.sodium_memzero(buf, (nuint)buf.Length);
		}

		/// <summary>
		/// Locks a buffer in memory, preventing it from being swapped to disk.
		/// </summary>
		/// <param name="buf">The span of bytes to lock.</param>
		/// <exception cref="SodiumException">Thrown if locking the memory fails.</exception>
		public static void MemLock(Span<byte> buf)
		{
			SodiumBindings.EnsureInitialized();
			if (Libsodium.sodium_mlock(buf, (nuint)buf.Length) != 0)
			{
				throw new SodiumException("Failed to lock memory");
			}
		}

		/// <summary>
		/// Unlocks a buffer in memory, allowing it to be swapped to disk if necessary.
		/// </summary>
		/// <param name="buf">The span of bytes to unlock.</param>
		/// <exception cref="SodiumException">Thrown if unlocking the memory fails.</exception>

		public static void MemUnlock(Span<byte> buf)
		{
			SodiumBindings.EnsureInitialized();
			if (Libsodium.sodium_munlock(buf, (nuint)buf.Length) != 0)
			{
				throw new SodiumException("Failed to unlock memory");
			}
		}

		/// <summary>
		/// Allocates a buffer of the specified size in secure memory.
		/// </summary>
		/// <param name="size">The size of the buffer to allocate in bytes.</param>
		/// <returns>A span of bytes representing the allocated memory.</returns>
		/// <exception cref="SodiumException">Thrown if memory allocation fails.</exception>
		public static Span<byte> Malloc(int size)
		{
			SodiumBindings.EnsureInitialized();
			unsafe
			{
				void* ptr = Libsodium.sodium_malloc((nuint)size);
				if (ptr == null)
				{
					throw new SodiumException("Failed to allocate memory");
				}
				return new Span<byte>(ptr, size);
			}
		}

		/// <summary>
		/// Frees a buffer allocated with <see cref="Malloc"/>.
		/// </summary>
		/// <param name="buf">The span of bytes representing the memory to free.</param>
		public static void Free(Span<byte> buf)
		{
			SodiumBindings.EnsureInitialized();
			Libsodium.sodium_free(buf);
		}

		/// <summary>
		/// Allocates an array of the specified type and length in secure memory.
		/// </summary>
		/// <typeparam name="T">The type of elements in the array. Must be an unmanaged type.</typeparam>
		/// <param name="length">The number of elements to allocate.</param>
		/// <returns>A span of the specified type representing the allocated memory.</returns>
		/// <exception cref="SodiumException">Thrown if memory allocation fails.</exception>

		public static Span<T> AllocArray<T>(int length) where T : unmanaged
		{
			SodiumBindings.EnsureInitialized();
			unsafe
			{
				void* ptr = Libsodium.sodium_allocarray((nuint)length, (nuint)sizeof(T));
				if (ptr == null)
				{
					throw new SodiumException("Failed to allocate memory");
				}
				return new Span<T>(ptr, length);
			}
		}

		/// <summary>
		/// Frees an array allocated with <see cref="AllocArray"/>.
		/// </summary>
		/// <typeparam name="T">The type of elements in the array. Must be an unmanaged type.</typeparam>
		/// <param name="buf">The span of the array to free.</param>

		public static void FreeArray<T>(Span<T> buf) where T : unmanaged
		{
			Free(MemoryMarshal.AsBytes(buf));
		}

		/// <summary>
		/// Sets the memory protection of a buffer to read-only.
		/// </summary>
		/// <param name="buf">The span of bytes to protect.</param>
		/// <exception cref="SodiumException">Thrown if setting the memory protection fails.</exception>
		public static void ProtectReadOnly(Span<byte> buf)
		{
			SodiumBindings.EnsureInitialized();
			if (Libsodium.sodium_mprotect_readonly(buf) != 0)
			{
				throw new SodiumException("Failed to set memory to read-only");
			}
		}

		/// <summary>
		/// Sets the memory protection of an array buffer to read-only.
		/// </summary>
		/// <typeparam name="T">The type of elements in the array. Must be an unmanaged type.</typeparam>
		/// <param name="arrayBuffer">The span of the array to protect.</param>
		/// <exception cref="SodiumException">Thrown if setting the memory protection fails.</exception>

		public static void ProtectReadOnly<T>(Span<T> arrayBuffer) where T : unmanaged
		{
			ProtectReadOnly(MemoryMarshal.AsBytes(arrayBuffer));
		}

		/// <summary>
		/// Sets the memory protection of a buffer to read-write.
		/// </summary>
		/// <param name="buf">The span of bytes to protect.</param>
		/// <exception cref="SodiumException">Thrown if setting the memory protection fails.</exception>

		public static void ProtectReadWrite(Span<byte> buf)
		{
			SodiumBindings.EnsureInitialized();
			if (Libsodium.sodium_mprotect_readwrite(buf) != 0)
			{
				throw new SodiumException("Failed to set memory to read-write");
			}
		}

		/// <summary>
		/// Sets the memory protection of an array buffer to read-write.
		/// </summary>
		/// <typeparam name="T">The type of elements in the array. Must be an unmanaged type.</typeparam>
		/// <param name="arrayBuffer">The span of the array to protect.</param>
		/// <exception cref="SodiumException">Thrown if setting the memory protection fails.</exception>
		public static void ProtectReadWrite<T>(Span<T> arrayBuffer) where T : unmanaged
		{
			ProtectReadWrite(MemoryMarshal.AsBytes(arrayBuffer));
		}
	}
}
