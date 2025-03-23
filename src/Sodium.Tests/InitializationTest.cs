using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodium.Tests
{
	public class InitializationTest
	{
		[Test]
		public async Task EnsureInitializedTest()
		{
			SodiumBindings.EnsureInitialized();
			
			await Assert.That(SodiumBindings.IsInitialized).IsTrue();
			await Assert.That(Interop.Libsodium.sodium_init()).IsEqualTo(1);
		}
	}
}
