using Guppi.Repl.Results;
using Guppi.Repl.Skills;

namespace Guppi.Repl.Tests.Skills
{

    [TestFixture]
    public sealed class IpModuleTests
    {
        [Test]
        public async Task LocalReturnsTypedIpv4Addresses()
        {
            var addresses = new[]
            {
            new LocalIpAddressResult("Ethernet", "192.0.2.42"),
        };
            var fake = new FakeLocalIpAddressProvider(addresses);
            var app = GuppiReplApp.Create(services =>
                services.AddSingleton<ILocalIpAddressProvider>(fake));
            app.Services.GetRequiredService<ILocalIpAddressProvider>().Should().BeSameAs(fake);
            await using var host = ReplTestHost.Create(() => app);
            await using var session = await host.OpenSessionAsync();

            var execution = await session.RunCommandAsync("ip local --json --no-logo");

            fake.CallCount.Should().Be(1);
            execution.ExitCode.Should().Be(0, execution.OutputText);
            execution.GetResult<LocalIpAddressResult[]>().Should().Equal(addresses);
        }

        private sealed class FakeLocalIpAddressProvider(LocalIpAddressResult[] addresses) : ILocalIpAddressProvider
        {
            public int CallCount { get; private set; }

            public LocalIpAddressResult[] GetLocalAddresses()
            {
                CallCount++;
                return addresses;
            }
        }
    }
}
