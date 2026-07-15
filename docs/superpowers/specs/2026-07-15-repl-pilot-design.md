# Additive Repl Pilot for Guppi

**Date:** 2026-07-15
**Version:** 9.2.0 (unchanged)
**Status:** Approved for implementation

## Overview

Add a new `Guppi.Repl` executable as a vertical pilot of Repl Toolkit without changing or removing the existing `Guppi.Console` and `Guppi.MCP` applications. The pilot maps a deliberately small set of operations once and exposes the same command graph as a one-shot CLI, an interactive REPL, and an MCP server.

The pilot uses the latest published Repl prerelease verified on GitHub Releases and NuGet on 2026-07-15: `0.11.0-dev.181`.

## Problem

```text
Business services
   ├── System.CommandLine skill wiring
   └── separate MCP tool wiring
```

The pilot tests the intended Repl model:

```text
Business services
   └── one Repl command graph
         ├── CLI
         ├── interactive REPL
         └── MCP
```

## Goals

- Prove one typed command graph can serve CLI, REPL, and MCP.
- Keep existing executables and non-pilot skills unchanged.
- Preserve skill organization with one `IReplModule` per pilot skill.
- Return typed, JSON-friendly records instead of surface-specific strings.
- Support natural scoped use:

  ```text
  guppi.repl
  > utilities date
  > hue
  [hue]> lights
  [hue]> kitchen on
  ```

- Demonstrate DI, cancellation, behavioral annotations, dynamic completion, structured output, and `Repl.Testing`.
- Keep device/network tests deterministic through injected fakes.

## Non-Goals

- Migrating all skills.
- Removing or changing `Guppi.Console`, `Guppi.MCP`, or their packages.
- Publishing the pilot package from CI.
- Changing the `dotnet-todo` submodule.
- Adding Telnet or WebSocket transports.

## Pilot Command Surface

| Route | Typed result | MCP behavior |
|---|---|---|
| `utilities date [--utc|-u]` | `DateResult` | `.ReadOnly()` |
| `utilities guid` | `GuidResult` | `.ReadOnly()` |
| `ip local` | list of `LocalIpAddressResult` | `.ReadOnly()` |
| `hue bridges` | list of `HueBridgeResult` | `.ReadOnly().OpenWorld().LongRunning()` |
| `hue lights [--ip|-i]` | list of `HueLightResult` | `.ReadOnly().OpenWorld().LongRunning()` |
| `hue {light} on [--ip|-i] [--brightness|-b] [--color|-c]` | `HueActionResult` | `.OpenWorld().Idempotent().LongRunning()` |
| `hue {light} off [--ip|-i]` | `HueActionResult` | `.OpenWorld().Idempotent().LongRunning()` |

`{light}` accepts a numeric ID or case-insensitive name. Resolution lists lights through `IHueLightService`, then calls its existing `SetLight` operation.

## Architecture

`GuppiReplApp.Create(...)` creates one `ReplApp`, registers `Guppi.Core`, `TimeProvider.System`, and Repl.Spectre through DI, permits tests to append replacement registrations, mounts named module contexts, enables terminal, interactive, and CLI profiles, and calls `UseMcpServer()`.

```text
Guppi.Repl/
  GuppiReplApp.cs
  Program.cs
  Skills/
    UtilitiesModule.cs
    IpModule.cs
    HueModule.cs
  Results/
    UtilityResults.cs
    NetworkResults.cs
    HueResults.cs
```

Modules use static lambdas and handler-parameter DI. Handlers return serializable records. Errors use semantic Repl results instead of successful strings beginning with `Error:`.

The pilot does not add Hue registration. It avoids synchronous `Console.ReadLine()` in MCP or test sessions and points users to the existing `guppi hue register` flow when registration is required.

`WithCompletion("light", ...)` suggests Hue names and IDs in the interactive REPL. It keeps the default interactive-only scope because Hue discovery is too expensive and stateful to execute on every shell Tab.

## Testing

A new `Guppi.Repl.Tests` project uses NUnit, FluentAssertions, and `Repl.Testing`. Tests invoke the real graph and injected fakes for typed utility, IP, and Hue results, Hue name resolution, semantic errors, completion, and persistent REPL context navigation. Final smoke validation launches `mcp serve` over STDIO and verifies `tools/list`.

## Packaging and CI

`Guppi.Repl` is locally packable as `dotnet-guppi-repl` with tool command `guppi.repl`. It is added to `Guppi.slnx` for build and test coverage, but existing CI publication remains unchanged.

## Acceptance Criteria

- Original 128 tests remain green and pilot tests pass.
- `Guppi.Repl` builds on .NET 10 with Repl `0.11.0-dev.181`.
- Selected operations run from CLI and a persistent REPL session.
- `mcp serve` exposes those operations from the same graph.
- Successes return typed results; failures are semantic errors.
- `hue kitchen on` resolves a light by name.
- Existing executables receive no runtime behavior changes.
- The pilot packs and installs locally as `dotnet-guppi-repl` and `guppi.repl`.
