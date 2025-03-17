using Sodium.Interop;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sodium
{
	public static class SodiumBigUInt
	{
		/// <summary>
		/// Compares two byte buffers for equality in constant time.
		/// </summary>
		/// <param name="b1">first buffer to compare</param>
		/// <param name="b2">second buffer to compare</param>
		/// <returns>True if they are equals, false otherwise</returns>
		public static bool Equals(ReadOnlySpan<byte> b1, ReadOnlySpan<byte> b2)
		{
			SodiumBindings.EnsureInitialized();
			if (b1.Length != b2.Length)
			{
				return false;
			}
			return Libsodium.sodium_memcmp(b1, b2, (nuint)b1.Length) == 0;
		}

		public static void Increment(Span<byte> number, ulong increment)
		{
			// sodium_increment doesn't work
			//if (increment > (ulong)nuint.MaxValue)
			//{
			//	throw new SodiumException($"increment must be less than or equal to {nuint.MaxValue}. But it is {increment}");
			//}
			//Libsodium.sodium_increment(number, (nuint)increment);

			//So Let's do it using Add
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

		public static void Add(Span<byte> a, ReadOnlySpan<byte> b)
		{
			SodiumBindings.EnsureInitialized();
			if (a.Length != b.Length)
			{
				throw new ArgumentException("a and b must have the same length");
			}
			Libsodium.sodium_add(a, b, (nuint)a.Length);
		}

		public static void Subtract(Span<byte> subtrahend, ReadOnlySpan<byte> minuend)
		{
			SodiumBindings.EnsureInitialized();
			if (subtrahend.Length != minuend.Length)
			{
				throw new ArgumentException("subtrahend and minuend must have the same length");
			}
			Libsodium.sodium_sub(subtrahend, minuend, (nuint)subtrahend.Length);
		}

		public static int Compare(ReadOnlySpan<byte> b1, ReadOnlySpan<byte> b2)
		{
			SodiumBindings.EnsureInitialized();
			if (b1.Length != b2.Length)
			{
				throw new ArgumentException("b1 and b2 must have the same length");
			}
			return Libsodium.sodium_compare(b1, b2, (nuint)b1.Length);
		}

		public static bool IsZero(ReadOnlySpan<byte> b)
		{
			SodiumBindings.EnsureInitialized();
			return Libsodium.sodium_is_zero(b, (nuint)b.Length) == 1;
		}
	}
}
