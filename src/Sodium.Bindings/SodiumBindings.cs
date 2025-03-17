using Sodium.Interop;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sodium
{
	public static partial class SodiumBindings
	{
		private static volatile bool isInitialized;
		private static readonly object initLock = new object();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void EnsureInitialized()
		{
			if (isInitialized) return;
			lock (initLock)
			{
				if (isInitialized) return;
				InitializeBindings();
			}
		}

		private static void SodiumInit()
		{
			// sodium_init() returns 0 on success, -1 on failure, and 1 if the library had already been initialized.
			if (Libsodium.sodium_init() < 0)
			{
				throw new SodiumException("Failed to initialize libsodium.");
			};
		}

		private static void SetMisuseHandler(Action handler)
		{
			if (Libsodium.sodium_set_misuse_handler(handler) != 0)
			{
				throw new SodiumException("Failed to set misuse handler.");
			};
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void InitializeBindings()
		{
			try
			{
				if (SodiumVersion.GetMajor() != Libsodium.LIBSODIUM_VERSION_MAJOR ||
					SodiumVersion.GetMinor() != Libsodium.LIBSODIUM_VERSION_MINOR)
				{
					string? version = SodiumVersion.GetString();
					throw version != null && version != Libsodium.SODIUM_VERSION_STRING
						? new SodiumException($"An error occurred while initializing cryptographic primitives. (Expected libsodium {Libsodium.SODIUM_VERSION_STRING} but found {version}.)")
						: new SodiumException("An error occurred while initializing cryptographic primitives: version mismatch");
				}
				SetMisuseHandler(MisuseHandler);
				SodiumInit();
				isInitialized = true;
			}
			catch (DllNotFoundException e)
			{
				throw new PlatformNotSupportedException("Could not initialize platform-specific components. libsodium may not be supported on this platform. See https://github.com/ektrah/libsodium-core/blob/master/INSTALL.md for more information.", e);
			}
			catch (BadImageFormatException e)
			{
				throw new PlatformNotSupportedException("Could not initialize platform-specific components. libsodium may not be supported on this platform. See https://github.com/ektrah/libsodium-core/blob/master/INSTALL.md for more information.", e);
			}
		}

		private static void MisuseHandler()
		{
			throw new SodiumException("Misuse detected");
		}
	}
}