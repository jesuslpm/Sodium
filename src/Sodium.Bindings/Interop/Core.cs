using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sodium.Interop
{
	internal static partial class Libsodium
	{

		// Corresponding to LIBSODIUM_VERSION_MAJOR
		[LibraryImport("libsodium")]
		[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
		internal static partial int sodium_init();

		// Corresponding to LIBSODIUM_VERSION_MINOR
		[LibraryImport("libsodium")]
		[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
		internal static partial int sodium_set_misuse_handler(Action handler);

	}
}
