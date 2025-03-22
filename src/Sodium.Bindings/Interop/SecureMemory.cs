using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sodium.Interop
{
	internal static partial class Libsodium
	{

		[LibraryImport("libsodium")]
		[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
		internal static partial void sodium_memzero(Span<byte> buf, nuint len);

		[LibraryImport("libsodium")]
		[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
		internal static partial int sodium_mlock(Span<byte> buf, nuint len);

		[LibraryImport("libsodium")]
		[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
		internal static partial int sodium_munlock(Span<byte> buf, nuint len);

		[LibraryImport("libsodium")]
		[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
		internal static unsafe partial void * sodium_malloc(nuint size);

		[LibraryImport("libsodium")]
		[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
		internal static partial void sodium_free(Span<byte> buf);

		[LibraryImport("libsodium")]
		[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
		internal static unsafe partial void* sodium_allocarray(nuint count, nuint size);

		[LibraryImport("libsodium")]
		[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
		internal static partial int sodium_mprotect_readonly(Span<byte> buf);

		[LibraryImport("libsodium")]
		[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
		internal static partial int sodium_mprotect_readwrite(Span<byte> buf);
	}
}
