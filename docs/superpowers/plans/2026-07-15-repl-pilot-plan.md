# Additive Repl Pilot Implementation Plan

**Goal:** Add a separate `Guppi.Repl` pilot that maps selected capabilities once and exposes them as CLI, interactive REPL, and MCP.

**Architecture:** Keep existing heads intact. Add one Repl composition root and one constructor-injected `IReplModule` per pilot skill, typed records, and `Repl.Testing` graph tests.

**Tech Stack:** .NET 10, Repl, Repl.Mcp, Repl.Spectre, and Repl.Testing `0.11.0-dev.181`, NUnit 4, FluentAssertions 8.

**Design:** [2026-07-15-repl-pilot-design.md](../specs/2026-07-15-repl-pilot-design.md)

## Task 1: Test and project infrastructure

- [x] Write the first graph test against the wished-for `GuppiReplApp.Create()` API.
- [x] Verify RED because the app does not exist.
- [x] Add minimal `Guppi.Repl`, `Guppi.Repl.Tests`, packages, and solution entries.
- [x] Verify the test now fails because `utilities date` is unmapped.

## Task 2: `utilities date`

- [x] Test a fixed UTC date through `ReplTestHost` and typed `DateResult`; verify RED.
- [x] Register `TimeProvider`, mount `UtilitiesModule`, map typed `--utc`; verify GREEN.

## Task 3: `utilities guid`

- [x] Test a typed non-empty `GuidResult`; verify RED.
- [x] Map the read-only command; verify GREEN.

## Task 4: `ip local`

- [x] Test with a deterministic fake `ILocalIpAddressProvider`; verify RED.
- [x] Map active IPv4 interfaces to `LocalIpAddressResult`; annotate read-only; verify GREEN.

## Task 5: Hue discovery

- [x] Test typed `hue bridges`; RED, implement, GREEN.
- [x] Test typed `hue lights --ip ...`; RED, implement, GREEN.
- [x] Map domain entities to stable records and add read-only, open-world, and long-running metadata.

## Task 6: Natural Hue controls

- [x] Test `hue kitchen on` name-to-ID resolution; RED, implement, GREEN.
- [x] Test numeric IDs, case-insensitive names, and `hue kitchen off` in separate slices.
- [x] Test semantic unknown-light failure without sending a command.
- [x] Add typed IP, brightness, and color options plus mutation annotations.

## Task 7: Hue completion

- [x] Test fake-backed name and ID completion; verify RED.
- [x] Attach interactive-only `WithCompletion("light", ...)`; verify GREEN.

## Task 8: Persistent REPL

- [x] Confirm `ReplTestHost` hosted invocations do not preserve interactive context scope.
- [x] Run one real redirected interactive loop: enter `hue`, list lights, turn on `kitchen`, navigate back, then run `utilities date`.
- [x] Assert context behavior and fake-backed device calls with no real device access.

## Task 9: MCP

- [x] Enable `UseMcpServer()` and keep CLI and REPL tests green.
- [x] Launch STDIO MCP, send initialize plus `tools/list`, and verify pilot tools and schemas.

## Task 10: Packaging

- [x] Pack Release, inspect the package, and install into a temporary tool path.
- [x] Smoke-test installed CLI JSON and MCP discovery.

## Task 11: Documentation

- [x] Update `README.md` and `AGENTS.md` for additive and experimental scope, commands, conventions, and non-publication.

## Task 12: Full verification and PR

- [x] Release build, full tests, CLI, REPL, MCP, completion, and package smoke tests.
- [x] Format and analyzers, `git diff --check`, security and scope review.
- [x] Push `agent/repl-pilot` and open a verified draft PR to `rprouse/guppi:main` with exact evidence.

## Task 13: Post-review hardening

- [x] Replace context-prefix MCP exposure with an exact seven-command allow-list.
- [x] Resolve a fresh Hue service/provider per invocation and cover the lifetime with a regression test.
- [x] Prioritize numeric Hue IDs and reject duplicate case-insensitive names with candidate IDs.
- [x] Model brightness as an integer percentage, validate 0–100, and assert its live MCP schema.
- [x] Mark Hue mutations explicitly destructive and assert their MCP annotations.
- [x] Automate MCP STDIO initialize, exact tool listing, schema validation, tool call, and JSON-only stdout.
- [x] Propagate cancellation through Core and Hue discovery without `WaitAsync` around non-cancellable Q42 mutations.
