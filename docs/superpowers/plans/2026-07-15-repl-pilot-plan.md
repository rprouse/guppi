# Additive Repl Pilot Implementation Plan

**Goal:** Add a separate `Guppi.Repl` pilot that maps selected capabilities once and exposes them as CLI, interactive REPL, and MCP.

**Architecture:** Keep existing heads intact. Add one Repl composition root and one `IReplModule` per pilot skill, typed records, handler-parameter DI, and `Repl.Testing` graph tests.

**Tech Stack:** .NET 10, Repl, Repl.Mcp, Repl.Spectre, and Repl.Testing `0.11.0-dev.181`, NUnit 4, FluentAssertions 8.

**Design:** [2026-07-15-repl-pilot-design.md](../specs/2026-07-15-repl-pilot-design.md)

## Task 1: Test and project infrastructure

- [ ] Write the first graph test against the wished-for `GuppiReplApp.Create()` API.
- [ ] Verify RED because the app does not exist.
- [ ] Add minimal `Guppi.Repl`, `Guppi.Repl.Tests`, packages, and solution entries.
- [ ] Verify the test now fails because `utilities date` is unmapped.

## Task 2: `utilities date`

- [ ] Test a fixed UTC date through `ReplTestHost` and typed `DateResult`; verify RED.
- [ ] Register `TimeProvider`, mount `UtilitiesModule`, map typed `--utc`; verify GREEN.

## Task 3: `utilities guid`

- [ ] Test a typed non-empty `GuidResult`; verify RED.
- [ ] Map the read-only command; verify GREEN.

## Task 4: `ip local`

- [ ] Test with a deterministic fake `IIPService`; verify RED.
- [ ] Map active IPv4 interfaces to `LocalIpAddressResult`; annotate read-only; verify GREEN.

## Task 5: Hue discovery

- [ ] Test typed `hue bridges`; RED, implement, GREEN.
- [ ] Test typed `hue lights --ip ...`; RED, implement, GREEN.
- [ ] Map domain entities to stable records and add read-only, open-world, and long-running metadata.

## Task 6: Natural Hue controls

- [ ] Test `hue kitchen on` name-to-ID resolution; RED, implement, GREEN.
- [ ] Test numeric IDs, case-insensitive names, and `hue kitchen off` in separate slices.
- [ ] Test semantic unknown-light failure without sending a command.
- [ ] Add typed IP, brightness, and color options plus mutation annotations.

## Task 7: Hue completion

- [ ] Test fake-backed name and ID completion; verify RED.
- [ ] Attach interactive-only `WithCompletion("light", ...)`; verify GREEN.

## Task 8: Persistent REPL

- [ ] Open a `ReplTestHost` session.
- [ ] Enter `hue`, run `lights`, run `kitchen on`, navigate back, then run `utilities date`.
- [ ] Assert typed results and context behavior with no real device access.

## Task 9: MCP

- [ ] Enable `UseMcpServer()` and keep CLI and REPL tests green.
- [ ] Launch STDIO MCP, send initialize plus `tools/list`, and verify pilot tools and schemas.

## Task 10: Packaging

- [ ] Pack Release, inspect the package, and install into a temporary tool path.
- [ ] Smoke-test installed CLI JSON and MCP discovery.

## Task 11: Documentation

- [ ] Update `README.md` and `AGENTS.md` for additive and experimental scope, commands, conventions, and non-publication.

## Task 12: Full verification and PR

- [ ] Release build, full tests, CLI, REPL, MCP, completion, and package smoke tests.
- [ ] Format and analyzers, `git diff --check`, security and scope review.
- [ ] Commit logical increments, push `agent/repl-pilot`, and open a verified draft PR to `rprouse/guppi:main` with exact evidence.
