using System.Collections.Generic;
using System.IO;
using System.Threading;
using Guppi.Core.Entities.Hue;
using Guppi.Core.Interfaces.Services;
using Guppi.Core.Services.Hue;
using Guppi.Repl.Results;

namespace Guppi.Repl.Tests.Skills
{

    [TestFixture]
    public sealed class HueModuleTests
    {
        [Test]
        public async Task BridgesReturnsTypedDiscoveryResults()
        {
            var fake = new FakeHueLightService
            {
                Bridges =
                [
                    new HueBridge { BridgeId = "bridge-1", IpAddress = "192.0.2.10" },
            ],
            };
            await using var host = ReplTestHost.Create(() =>
                GuppiReplApp.Create(services => services.AddSingleton<IHueLightService>(fake)));
            await using var session = await host.OpenSessionAsync();

            var execution = await session.RunCommandAsync("hue bridges --json --no-logo");

            execution.ExitCode.Should().Be(0, execution.OutputText);
            execution.GetResult<HueBridgeResult[]>().Should().Equal(
                new HueBridgeResult("bridge-1", "192.0.2.10"));
            fake.ListBridgesCallCount.Should().Be(1);
        }

        [Test]
        public async Task LightsReturnsTypedResultsForSelectedBridge()
        {
            var fake = new FakeHueLightService
            {
                Lights =
                [
                    new HueLight
                {
                    Id = "7",
                    Name = "Kitchen",
                    On = true,
                    Brightness = 200,
                    Color = "#ffaa00",
                    Type = "Extended color light",
                },
            ],
            };
            await using var host = ReplTestHost.Create(() =>
                GuppiReplApp.Create(services => services.AddSingleton<IHueLightService>(fake)));
            await using var session = await host.OpenSessionAsync();

            var execution = await session.RunCommandAsync("hue lights --ip 192.0.2.10 --json --no-logo");

            execution.ExitCode.Should().Be(0, execution.OutputText);
            execution.GetResult<HueLightResult[]>().Should().Equal(
                new HueLightResult("7", "Kitchen", true, 200, "#ffaa00", "Extended color light"));
            fake.ListLightsCallCount.Should().Be(1);
            fake.LastIpAddress.Should().Be("192.0.2.10");
        }

        [Test]
        public async Task OnResolvesLightNameAndSendsTypedCommand()
        {
            var fake = new FakeHueLightService
            {
                Lights =
                [
                    new HueLight { Id = "7", Name = "Kitchen" },
            ],
            };
            await using var host = ReplTestHost.Create(() =>
                GuppiReplApp.Create(services => services.AddSingleton<IHueLightService>(fake)));
            await using var session = await host.OpenSessionAsync();

            var execution = await session.RunCommandAsync(
                "hue kItChEn on --ip 192.0.2.10 --brightness 80 --color red --json --no-logo");

            execution.ExitCode.Should().Be(0, execution.OutputText);
            execution.GetResult<HueActionResult>().Should().Be(
                new HueActionResult("7", "Kitchen", true, 80, "red", "192.0.2.10"));
            fake.SetLightCallCount.Should().Be(1);
            fake.LastSetLight.Should().BeEquivalentTo(new SetLightCommand
            {
                IpAddress = "192.0.2.10",
                Light = 7,
                On = true,
                Off = false,
                Brightness = 80,
                Color = "red",
            }, options => options.Excluding(command => command.WaitForUserInput));
        }

        [Test]
        public async Task OffResolvesNumericIdAndSendsTypedCommand()
        {
            var fake = new FakeHueLightService
            {
                Lights =
                [
                    new HueLight { Id = "7", Name = "Kitchen" },
            ],
            };
            await using var host = ReplTestHost.Create(() =>
                GuppiReplApp.Create(services => services.AddSingleton<IHueLightService>(fake)));
            await using var session = await host.OpenSessionAsync();

            var execution = await session.RunCommandAsync(
                "hue 7 off --ip 192.0.2.10 --json --no-logo");

            execution.ExitCode.Should().Be(0, execution.OutputText);
            execution.GetResult<HueActionResult>().Should().Be(
                new HueActionResult("7", "Kitchen", false, null, null, "192.0.2.10"));
            fake.SetLightCallCount.Should().Be(1);
            fake.LastSetLight.Should().BeEquivalentTo(new SetLightCommand
            {
                IpAddress = "192.0.2.10",
                Light = 7,
                On = false,
                Off = true,
            }, options => options.Excluding(command => command.WaitForUserInput));
        }

        [Test]
        public async Task BrightnessOutsidePercentageRangeReturnsValidationWithoutMutation()
        {
            var fake = new FakeHueLightService
            {
                Lights = [new HueLight { Id = "7", Name = "Kitchen" }],
            };
            await using var host = ReplTestHost.Create(() =>
                GuppiReplApp.Create(services => services.AddSingleton<IHueLightService>(fake)));
            await using var session = await host.OpenSessionAsync();

            var execution = await session.RunCommandAsync(
                "hue Kitchen on --brightness 101 --json --no-logo");

            execution.ExitCode.Should().Be(1);
            execution.OutputText.Should().Contain("Brightness must be between 0 and 100 percent.");
            fake.SetLightCallCount.Should().Be(0);
        }

        [Test]
        public async Task NumericInputPrioritizesAnExactIdOverTheSameName()
        {
            var fake = new FakeHueLightService
            {
                Lights =
                [
                    new HueLight { Id = "8", Name = "7" },
                    new HueLight { Id = "7", Name = "Kitchen" },
                ],
            };
            await using var host = ReplTestHost.Create(() =>
                GuppiReplApp.Create(services => services.AddSingleton<IHueLightService>(fake)));
            await using var session = await host.OpenSessionAsync();

            var execution = await session.RunCommandAsync("hue 7 off --json --no-logo");

            execution.ExitCode.Should().Be(0, execution.OutputText);
            fake.LastSetLight.Light.Should().Be(7);
            execution.GetResult<HueActionResult>().LightName.Should().Be("Kitchen");
        }

        [Test]
        public async Task DuplicateLightNameReturnsValidationWithCandidateIdsWithoutMutation()
        {
            var fake = new FakeHueLightService
            {
                Lights =
                [
                    new HueLight { Id = "7", Name = "Kitchen" },
                    new HueLight { Id = "8", Name = "kitchen" },
                ],
            };
            await using var host = ReplTestHost.Create(() =>
                GuppiReplApp.Create(services => services.AddSingleton<IHueLightService>(fake)));
            await using var session = await host.OpenSessionAsync();

            var execution = await session.RunCommandAsync("hue Kitchen off --json --no-logo");

            execution.ExitCode.Should().Be(1);
            execution.OutputText.Should().Contain("Hue light name Kitchen is ambiguous. Use one of these IDs: 7, 8.");
            fake.SetLightCallCount.Should().Be(0);
        }

        [Test]
        public async Task UnknownLightReturnsValidationWithoutMutation()
        {
            var fake = new FakeHueLightService
            {
                Lights =
                [
                    new HueLight { Id = "7", Name = "Kitchen" },
            ],
            };
            await using var host = ReplTestHost.Create(() =>
                GuppiReplApp.Create(services => services.AddSingleton<IHueLightService>(fake)));
            await using var session = await host.OpenSessionAsync();

            var execution = await session.RunCommandAsync("hue unknown on --json --no-logo");

            execution.ExitCode.Should().Be(1);
            execution.OutputText.Should().Contain("\"kind\": \"validation\"");
            execution.OutputText.Should().Contain("Hue light unknown was not found.");
            fake.SetLightCallCount.Should().Be(0);
        }

        [Test]
        public async Task LightCompletionReturnsMatchingNamesAndIds()
        {
            var fake = new FakeHueLightService
            {
                Lights =
                [
                    new HueLight { Id = "7", Name = "Kitchen" },
                new HueLight { Id = "8", Name = "Bedroom" },
            ],
            };
            await using var host = ReplTestHost.Create(() =>
                GuppiReplApp.Create(services => services.AddSingleton<IHueLightService>(fake)));
            await using var session = await host.OpenSessionAsync();

            var execution = await session.RunCommandAsync(
                "complete hue placeholder on --target light --input Kit --no-logo");

            execution.ExitCode.Should().Be(0, execution.OutputText);
            execution.OutputText.Should().Contain("Kitchen");
            execution.OutputText.Should().NotContain("Bedroom");
            fake.ListLightsCallCount.Should().Be(1);
        }

        [Test]
        public async Task HueInvocationPassesCancellationTokenToDiscoveryAndMutation()
        {
            var fake = new FakeHueLightService
            {
                Lights = [new HueLight { Id = "7", Name = "Kitchen" }],
            };
            await using var host = ReplTestHost.Create(() =>
                GuppiReplApp.Create(services => services.AddSingleton<IHueLightService>(fake)));
            await using var session = await host.OpenSessionAsync();
            using var cancellation = new CancellationTokenSource();

            var execution = await session.RunCommandAsync(
                "hue Kitchen on --json --no-logo",
                cancellation.Token);

            execution.ExitCode.Should().Be(0, execution.OutputText);
            fake.ListLightsCancellationToken.Should().NotBeNull();
            fake.ListLightsCancellationToken.Value.CanBeCanceled.Should().BeTrue();
            fake.SetLightCancellationToken.Should().Be(fake.ListLightsCancellationToken);
        }

        [Test]
        public async Task HueServiceIsResolvedPerCommandInvocation()
        {
            var createdServices = 0;
            await using var host = ReplTestHost.Create(() =>
                GuppiReplApp.Create(services => services.AddTransient<IHueLightService>(_ =>
                {
                    createdServices++;
                    return new FakeHueLightService
                    {
                        Lights = [new HueLight { Id = "7", Name = "Kitchen" }],
                    };
                })));
            await using var session = await host.OpenSessionAsync();

            var first = await session.RunCommandAsync("hue lights --json --no-logo");
            var second = await session.RunCommandAsync("hue lights --json --no-logo");

            first.ExitCode.Should().Be(0, first.OutputText);
            second.ExitCode.Should().Be(0, second.OutputText);
            createdServices.Should().Be(2);
        }

        [Test]
        [NonParallelizable]
        public void InteractiveLoopNavigatesHueAndBackToUtilities()
        {
            var fake = new FakeHueLightService
            {
                Lights =
                [
                    new HueLight { Id = "7", Name = "Kitchen" },
            ],
            };
            var originalInput = Console.In;
            var originalOutput = Console.Out;
            var originalError = Console.Error;
            using var input = new StringReader("lights --json\nKitchen on --json\n..\nutilities date --json\nexit\n");
            using var output = new StringWriter();

            int exitCode;
            try
            {
                Console.SetIn(input);
                Console.SetOut(output);
                Console.SetError(output);
                var app = GuppiReplApp.Create(services => services.AddSingleton<IHueLightService>(fake));
                exitCode = app.Run(["hue", "--no-logo"]);
            }
            finally
            {
                Console.SetIn(originalInput);
                Console.SetOut(originalOutput);
                Console.SetError(originalError);
            }

            exitCode.Should().Be(0, output.ToString());
            output.ToString().Should().Contain("Kitchen");
            output.ToString().Should().Contain("value");
            fake.SetLightCallCount.Should().Be(1);
        }

        private sealed class FakeHueLightService : IHueLightService
        {
            public HueBridge[] Bridges { get; init; } = [];

            public HueLight[] Lights { get; init; } = [];

            public int ListLightsCallCount { get; private set; }

            public CancellationToken? ListLightsCancellationToken { get; private set; }

            public CancellationToken? SetLightCancellationToken { get; private set; }

            public string LastIpAddress { get; private set; }

            public SetLightCommand LastSetLight { get; private set; }

            public int SetLightCallCount { get; private set; }

            public int ListBridgesCallCount { get; private set; }

            public void Configure() => throw new NotSupportedException();

            public Task<bool> Register(
                string ipAddress,
                Action<string> waitForUserInput,
                CancellationToken cancellationToken = default) =>
                throw new NotSupportedException();

            public uint GetDefaultLight() => throw new NotSupportedException();

            public Task<IEnumerable<HueLight>> ListLights(
                string ipAddress,
                Action<string> waitForUserInput,
                CancellationToken cancellationToken)
            {
                ListLightsCancellationToken = cancellationToken;
                return ListLightsCore(ipAddress);
            }

            private Task<IEnumerable<HueLight>> ListLightsCore(string ipAddress)
            {
                ListLightsCallCount++;
                LastIpAddress = ipAddress;
                return Task.FromResult<IEnumerable<HueLight>>(Lights);
            }

            public Task<IEnumerable<HueBridge>> ListBridges(CancellationToken cancellationToken = default)
            {
                ListBridgesCallCount++;
                return Task.FromResult<IEnumerable<HueBridge>>(Bridges);
            }

            public Task SetLight(SetLightCommand request, CancellationToken cancellationToken)
            {
                SetLightCancellationToken = cancellationToken;
                return SetLightCore(request);
            }

            private Task SetLightCore(SetLightCommand request)
            {
                SetLightCallCount++;
                LastSetLight = request;
                return Task.CompletedTask;
            }
        }
    }
}
