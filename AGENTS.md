# AGENTS.md

## Commands

```bash
# Build
dotnet build
dotnet build --configuration Release

# Test
dotnet test
dotnet test --no-restore --verbosity normal

# Pack as dotnet tools
dotnet pack Guppi.Console/Guppi.Console.csproj --configuration Release
dotnet pack Guppi.MCP/Guppi.MCP.csproj --configuration Release
dotnet pack Guppi.Repl/Guppi.Repl.csproj --configuration Release

# Install locally (from solution root)
dotnet tool install -g --add-source ./Guppi.Console/nupkg dotnet-guppi
dotnet tool install -g --add-source ./Guppi.MCP/nupkg dotnet-guppi-mcp
dotnet tool install -g --add-source ./Guppi.Repl/nupkg dotnet-guppi-repl

# Update local install
dotnet tool update -g --add-source ./Guppi.Console/nupkg dotnet-guppi
dotnet tool update -g --add-source ./Guppi.MCP/nupkg dotnet-guppi-mcp
dotnet tool update -g --add-source ./Guppi.Repl/nupkg dotnet-guppi-repl

# Run
guppi <skill> <command> [options]
guppi.mcp  # Starts the existing MCP server (STDIO)
guppi.repl <context> <command> [options]  # Experimental one-shot CLI
guppi.repl                              # Experimental interactive REPL
guppi.repl mcp serve                    # Experimental graph over MCP STDIO
```

## Architecture

```
Guppi.Console/          # CLI entry point, command definitions
  Skills/               # System.CommandLine commands (one per feature)
  Program.cs            # DI composition root
Guppi.Core/             # Business logic, no CLI dependencies
  Services/             # Service implementations (one per feature)
  Interfaces/           # Service and provider interfaces
  Providers/            # External system integrations (Git, Hue, Calendar, HTTP)
  Configurations/       # Per-skill configuration classes
  Entities/             # Domain models
  Extensions/           # Helper extension methods
Guppi.MCP/              # Existing MCP server entry point (STDIO transport)
  Tools/                # MCP tool classes (attribute-based)
  Program.cs            # DI composition root
Guppi.Repl/             # Experimental shared Repl graph (CLI + REPL + MCP)
  Skills/               # IReplModule definitions for pilot contexts
  Results/              # Stable typed result records
  GuppiReplApp.cs       # Shared composition root and MCP allow-list
Guppi.Repl.Tests/       # Repl.Testing graph tests and interactive integration tests
Guppi.Tests/            # Existing NUnit tests
dotnet-todo/            # Git submodule - todo.txt library
```

**Layer flow:**
- `Skill` (Console) -> `IService` -> `IProvider` (Core)
- `Tool` (MCP) -> `IService` -> `IProvider` (Core)
- `IReplModule` (pilot) -> `IService`/injected adapter -> `IProvider` (Core), exposed through one graph

Each feature follows a consistent pattern:
- `Guppi.Console/Skills/{Name}Skill.cs` — defines CLI commands via `ISkill.GetCommands()`
- `Guppi.Core/Services/{Name}Service.cs` — implements `I{Name}Service`
- `Guppi.Core/Interfaces/Services/I{Name}Service.cs` — service contract
- Optional: `Guppi.Core/Providers/` for external integrations
- Optional: `Guppi.Core/Configurations/` for per-skill settings

Skills are registered in `Program.cs` via DI as `ISkill` implementations.
MCP tools are registered in `Guppi.MCP/Program.cs` via `.WithTools<T>()` on the MCP server builder.
Services and providers are registered in `Guppi.Core/DependencyInjection.cs`.

## Key Dependencies

- **.NET 10.0 (net10.0)** — target framework
- **System.CommandLine** (2.0.0-beta4) — CLI framework (beta, some APIs may be unstable)
- **Spectre.Console** — rich terminal output
- **Microsoft.Extensions.DependencyInjection** — DI container
- **NUnit 4** + **FluentAssertions 8** — testing
- **dotnet-todo** — git submodule, must be checked out (`git submodule update --init`)
- **ModelContextProtocol** (1.1.0) — existing MCP server SDK (STDIO + HTTP transports)
- **Repl / Repl.Mcp / Repl.Spectre / Repl.Testing** (0.11.0-dev.181, pinned prerelease) — experimental shared graph
- **Notable Core dependencies:** LibGit2Sharp (Git operations), Google.Apis.Calendar/Tasks (Google integration), Q42.HueApi (Philips Hue), OpenAI (AI features), Microsoft.Playwright (web scraping), ClosedXML (Excel), System.IO.Ports (serial)

## MCP Tool Return Types

The `[McpServerTool]` method return type determines how the MCP SDK serializes the response:

- `string` → returned as a `TextContentBlock`
- Any other object → auto-serialized to JSON as text content
- `IEnumerable<ContentBlock>` → returned directly as content blocks
- `CallToolResult` → returned as-is for full control over the response

For tools that can return either structured data (success) or an error message (failure), use `Task<object>` as the return type. Return domain entities on success (auto-serialized to JSON) and a descriptive `string` on error.

## Code Style

Conventions from `.editorconfig` (follows NUnit coding standards):

- **Indentation:** 4 spaces, CRLF line endings
- **Private fields:** `_camelCase` (underscore prefix)
- **Public members:** `PascalCase`
- **Parameters/locals:** `camelCase`
- **Namespaces:** block-scoped (not file-scoped)
- **Braces:** Allman style (new line before `else`, `catch`, `finally`)
- **var:** use when type is apparent
- **Usings:** outside namespace, `System` directives first

## Configuration

Per-skill configuration files are stored as JSON in:
- Windows: `%LOCALAPPDATA%\Guppi\`
- Pattern: `Configuration.Load<T>(name)` / `Configuration.Save()`

## Adding a New Feature

1. Create `Guppi.Core/Interfaces/Services/I{Name}Service.cs` — service contract
2. Create `Guppi.Core/Services/{Name}Service.cs` — implements the service
3. Register the service in `Guppi.Core/DependencyInjection.cs`
4. **For CLI:** Create `Guppi.Console/Skills/{Name}Skill.cs` implementing `ISkill`, register in `Guppi.Console/Program.cs`
5. **For MCP:** Create `Guppi.MCP/Tools/{Name}Tools.cs` with `[McpServerTool]` attributes, register via `.WithTools<{Name}Tools>()` in `Guppi.MCP/Program.cs`
6. **For the experimental shared graph:** Add an `IReplModule` under `Guppi.Repl/Skills`, mount it in `GuppiReplApp`, return typed records, annotate MCP behavior, and explicitly allow-list its paths before exposing them
7. Optional: Add `Guppi.Core/Providers/` for external integrations, `Guppi.Core/Configurations/` for settings

## Gotchas

- The `dotnet-todo` directory is a **git submodule** — clone with `--recurse-submodules` or run `git submodule update --init`
  - **AGENTS:** Do not make changes to the todo submodule without explicit user approval. It is maintained separately from the main Guppi project.
- System.CommandLine is a **pre-release beta** — avoid using APIs not already in the codebase
- `Guppi.Core` exposes internals to `Guppi.Tests` via `InternalsVisibleTo`
- CI runs on `ubuntu-latest` but the app targets Windows features (System.Speech, System.Management)
- CI publishes two packages on merge to main: `dotnet-guppi` and `dotnet-guppi-mcp`; `dotnet-guppi-repl` is experimental and intentionally not in publication workflows
- The solution uses the new `.slnx` format (not `.sln`)
- **MCP STDIO transport:** Never log to stdout in `Guppi.MCP` or `guppi.repl mcp serve` — it corrupts JSON-RPC; protocol smoke tests must parse every stdout line as JSON
- Repl.Testing `0.11.0-dev.181` executes handlers with a session provider that does not include `app.Services`; inject pilot dependencies into `IReplModule` constructors, and use a real redirected interactive loop to test context navigation
- `dotnet format Guppi.slnx --verify-no-changes` currently reports historical line-ending/import/encoding debt; verify the new projects individually and do not reformat unrelated files

## Workflow

- **Always work on a branch** — never commit directly to `main`. Create a feature branch (e.g., `feature/hue-mcp-tools`) before making changes.
- **Always commit specs and plans** — design specs (`docs/superpowers/specs/`) and implementation plans (`docs/superpowers/plans/`) are project artifacts. Always include them in a commit on the feature branch.
- **Version updates** — update the version in `Directory.Build.props` using semantic versioning:
  - **Major** (X.0.0): breaking changes to existing commands, services, or MCP tool contracts
  - **Minor** (x.Y.0): new features — new skills, MCP tools, services, or commands
  - **Patch** (x.y.Z): bug fixes, documentation updates, refactoring with no behavior change
