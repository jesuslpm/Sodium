using TUnit.Assertions.AssertConditions.Throws;
using System;

namespace Sodium.Tests
{
	public class SodiumBigUIntTests
	{
		[Test]
		public async Task Equals_EqualBuffers_ReturnsTrue()
		{
			byte[] b1 = { 1, 2, 3, 4 };
			byte[] b2 = { 1, 2, 3, 4 };

			await Assert.That(SodiumBigUInt.Equals(b1, b2)).IsTrue();
		}

		[Test]
		public async Task Equals_DifferentBuffers_ReturnsFalse()
		{
			byte[] b1 = { 1, 2, 3, 4 };
			byte[] b2 = { 4, 3, 2, 1 };

			await Assert.That(SodiumBigUInt.Equals(b1, b2)).IsFalse();
		}

		[Test]
		public async Task Equals_DifferentLengths_ReturnsFalse()
		{
			byte[] b1 = { 1, 2, 3, 4 };
			byte[] b2 = { 1, 2, 3 };

			await Assert.That(SodiumBigUInt.Equals(b1, b2)).IsFalse();
		}

		[Test]
		public async Task Increment_IncrementsByOne()
		{
			byte[] number = { 0, 0, 0, 0 };
			byte[] expected = { 1, 0, 0, 0 };

			SodiumBigUInt.Increment(number);

			await Assert.That(number).IsSequenceEqualTo(expected);
		}

		[Test]
		public async Task Increment_IncrementsByValue()
		{
			byte[] number = { 0, 0, 0, 0, 0, 0, 0, 0 };
			byte[] expected = { 1, 0, 0, 0, 0, 0, 0, 0 };

			SodiumBigUInt.Increment(number, 1);

			await Assert.That(number).IsSequenceEqualTo(expected);
		}

		[Test]
		public async Task Increment_IncrementsByLargeValue()
		{
			byte[] number = { 0, 0, 0, 0, 0, 0, 0, 0 };
			byte[] expected = { 255, 255, 255, 255, 255, 255, 255, 127 };

			SodiumBigUInt.Increment(number, long.MaxValue);

			await Assert.That(number).IsSequenceEqualTo(expected);
		}

		[Test]
		public async Task Add_AddsTwoBuffers()
		{
			byte[] a = { 1, 0, 0, 0 };
			byte[] b = { 1, 0, 0, 0 };
			byte[] expected = { 2, 0, 0, 0 };

			SodiumBigUInt.Add(a, b);

			await Assert.That(a).IsSequenceEqualTo(expected);
		}

		[Test]
		public async Task Add_DifferentLengths_ThrowsArgumentException()
		{
			byte[] a = { 1, 0, 0, 0 };
			byte[] b = { 1, 0, 0 };

			await Assert.That(() => SodiumBigUInt.Add(a, b)).Throws<ArgumentException>();
		}

		[Test]
		public async Task Subtract_SubtractsTwoBuffers()
		{
			byte[] subtrahend = { 2, 0, 0, 0 };
			byte[] minuend = { 1, 0, 0, 0 };
			byte[] expected = { 1, 0, 0, 0 };

			SodiumBigUInt.Subtract(subtrahend, minuend);

			await Assert.That(subtrahend).IsSequenceEqualTo(expected);
		}

		[Test]
		public async Task Subtract_DifferentLengths_ThrowsArgumentException()
		{
			byte[] subtrahend = { 2, 0, 0, 0 };
			byte[] minuend = { 1, 0, 0 };

			await Assert.That(() => SodiumBigUInt.Subtract(subtrahend, minuend)).Throws<ArgumentException>();
		}

		[Test]
		public async Task Compare_EqualBuffers_ReturnsZero()
		{
			byte[] b1 = { 1, 2, 3, 4 };
			byte[] b2 = { 1, 2, 3, 4 };

			await Assert.That(SodiumBigUInt.Compare(b1, b2)).IsEqualTo(0);
		}

		[Test]
		public async Task Compare_FirstBufferLess_ReturnsMinusOne()
		{
			byte[] b1 = { 4, 3, 2, 1 };
			byte[] b2 = { 1, 2, 3, 4 };

			await Assert.That(SodiumBigUInt.Compare(b1, b2)).IsEqualTo(-1);
		}

		[Test]
		public async Task Compare_FirstBufferGreater_ReturnsOne()
		{
			byte[] b1 = { 1, 2, 3, 4 };
			byte[] b2 = { 4, 3, 2, 1 };

			await Assert.That(SodiumBigUInt.Compare(b1, b2)).IsEqualTo(1);
		}

		[Test]
		public async Task Compare_DifferentLengths_ThrowsArgumentException()
		{
			byte[] b1 = { 1, 2, 3, 4 };
			byte[] b2 = { 1, 2, 3 };

			await Assert.That(() => SodiumBigUInt.Compare(b1, b2)).Throws<ArgumentException>();
		}

		[Test]
		public async Task IsZero_ZeroBuffer_ReturnsTrue()
		{
			byte[] b = { 0, 0, 0, 0 };

			await Assert.That(SodiumBigUInt.IsZero(b)).IsTrue();
		}

		[Test]
		public async Task IsZero_NonZeroBuffer_ReturnsFalse()
		{
			byte[] b = { 1, 0, 0, 0 };

			await Assert.That(SodiumBigUInt.IsZero(b)).IsFalse();
		}
	}
}