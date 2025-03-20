﻿namespace Sodium
{
	/// <summary>
	/// Represents errors that occur during Sodium operations.
	/// </summary>
	[Serializable]
	public class SodiumException : InvalidOperationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SodiumException"/> class.
		/// </summary>
		public SodiumException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SodiumException"/> class
		/// with a specified error message.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		public SodiumException(string? message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SodiumException"/> class
		/// with a specified error message and a reference to the inner exception
		/// that is the cause of this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
		public SodiumException(string? message, Exception? innerException) : base(message, innerException)
		{
		}
	}
}
