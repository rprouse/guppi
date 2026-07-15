using Guppi.Repl.Results;

namespace Guppi.Repl.Tests.Skills
{

    [TestFixture]
    public sealed class UtilitiesModuleTests
    {
        [Test]
        public async Task DateReturnsTypedUtcResult()
        {
            var fixedTime = new DateTimeOffset(2030, 1, 2, 12, 34, 56, TimeSpan.Zero);
            await using var host = ReplTestHost.Create(() =>
                GuppiReplApp.Create(services =>
                    services.AddSingleton<TimeProvider>(new FixedTimeProvider(fixedTime))));
            await using var session = await host.OpenSessionAsync();

            var execution = await session.RunCommandAsync("utilities date --utc --json --no-logo");

            execution.ExitCode.Should().Be(0);
            var result = execution.GetResult<DateResult>();
            result.Value.Should().Be(new DateOnly(2030, 1, 2));
            result.IsUtc.Should().BeTrue();
        }

        [Test]
        public async Task GuidReturnsTypedNonEmptyResult()
        {
            await using var host = ReplTestHost.Create(() => GuppiReplApp.Create());
            await using var session = await host.OpenSessionAsync();

            var execution = await session.RunCommandAsync("utilities guid --json --no-logo");

            execution.ExitCode.Should().Be(0);
            execution.GetResult<GuidResult>().Value.Should().NotBe(Guid.Empty);
        }

        private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => utcNow;

            public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
        }
    }
}
