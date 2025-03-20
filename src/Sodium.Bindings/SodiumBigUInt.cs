﻿using Sodium.Interop;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Sodium
{
	/// <summary>
	/// Provides methods for working with big unsigned integers.
	/// </summary>
	public static class SodiumBigUInt
	{
		/// <summary>
		/// Compares two byte buffers for equality in constant time.
		/// </summary>
		/// <param name="b1">First buffer to compare.</param>
		/// <param name="b2">Second buffer to compare.</param>
		/// <returns>True if the buffers are equal, false otherwise.</returns>
		public static bool Equals(ReadOnlySpan<byte> b1, ReadOnlySpan<byte> b2)
		{
			SodiumBindings.EnsureInitialized();
			if (b1.Length != b2.Length)
			{
				return false;
			}
			return Libsodium.sodium_memcmp(b1, b2, (nuint)b1.Length) == 0;
		}

		/// <summary>
		/// Increments the given byte buffer representing a big unsigned integer by 1.
		/// </summary>
		/// <param name="number">The byte buffer to increment.</param>
		public static void Increment(Span<byte> number)
		{
			SodiumBindings.EnsureInitialized();
			Libsodium.sodium_increment(number, (nuint)number.Length);
		}

		/// <summary>
		/// Increments the given byte buffer representing a big unsigned integer by a specified value.
		/// </summary>
		/// <param name="number">The byte buffer representing a big unsigned integer to increment.</param>
		/// <param name="increment">The value to increment by.</param>
		public static void Increment(Span<byte> number, ulong increment)
		{
			SodiumBindings.EnsureInitialized();
			Span<byte> b = stackalloc byte[number.Length];
			if (BitConverter.IsLittleEndian)
			{
				Unsafe.WriteUnaligned(ref b[0], increment);
			}
			else
			{
				Unsafe.WriteUnaligned(ref b[0], BinaryPrimitives.ReverseEndianness(increment));
			}
			Add(number, b);
		}

		/// <summary>
		/// Adds two byte buffers representing big unsigned integers.
		/// </summary>
		/// <param name="a">The first byte buffer representing a big unsigned integer.</param>
		/// <param name="b">The second byte buffer representing a big unsigned integer.</param>
		public static void Add(Span<byte> a, ReadOnlySpan<byte> b)
		{
			SodiumBindings.EnsureInitialized();
			if (a.Length != b.Length)
			{
				throw new ArgumentException("a and b must have the same length");
			}
			Libsodium.sodium_add(a, b, (nuint)a.Length);
		}

		/// <summary>
		/// Subtracts one byte buffer from another representing big unsigned integers.
		/// </summary>
		/// <param name="subtrahend">The byte buffer to subtract from.</param>
		/// <param name="minuend">The byte buffer to subtract.</param>
		public static void Subtract(Span<byte> subtrahend, ReadOnlySpan<byte> minuend)
		{
			SodiumBindings.EnsureInitialized();
			if (subtrahend.Length != minuend.Length)
			{
				throw new ArgumentException("subtrahend and minuend must have the same length");
			}
			Libsodium.sodium_sub(subtrahend, minuend, (nuint)subtrahend.Length);
		}

		/// <summary>
		/// Compares two byte buffers representing big unsigned integers.
		/// </summary>
		/// <param name="b1">The first byte buffer.</param>
		/// <param name="b2">The second byte buffer.</param>
		/// <returns>A negative number if b1 is less than b2, zero if they are equal, and a positive number if b1 is greater than b2.</returns>
		public static int Compare(ReadOnlySpan<byte> b1, ReadOnlySpan<byte> b2)
		{
			SodiumBindings.EnsureInitialized();
			if (b1.Length != b2.Length)
			{
				throw new ArgumentException("b1 and b2 must have the same length");
			}
			return Libsodium.sodium_compare(b1, b2, (nuint)b1.Length);
		}

		/// <summary>
		/// Checks if the given byte buffer is zero.
		/// </summary>
		/// <param name="b">The byte buffer to check.</param>
		/// <returns>True if the byte buffer is zero, false otherwise.</returns>
		public static bool IsZero(ReadOnlySpan<byte> b)
		{
			SodiumBindings.EnsureInitialized();
			return Libsodium.sodium_is_zero(b, (nuint)b.Length) == 1;
		}
	}
}
