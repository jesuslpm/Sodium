using Sodium.Interop;
using System.Runtime.InteropServices;

namespace Sodium
{
	public static partial class SodiumVersion
	{
		public static int GetMajor()
		{
			return Libsodium.sodium_library_version_major();
		}

		public static int GetMinor()
		{
			return Libsodium.sodium_library_version_minor();
		}

		public static string? GetString()
		{
			var ptr = Libsodium.sodium_version_string();
			return Marshal.PtrToStringAnsi(ptr);
		}
	}
}
