using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodium
{
	[Serializable]
	public class SodiumException : InvalidOperationException
	{
		public SodiumException()
		{
		}

		public SodiumException(string? message) : base(message)
		{
		}

		public SodiumException(string? message, Exception? innerException) : base(message, innerException)
		{
		}
	}
}
