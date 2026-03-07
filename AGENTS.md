# AGENTS.md

## Commands

```bash
# Build
dotnet build
dotnet build --configuration Release

# Test
dotnet test
dotnet test --no-restore --verbosity normal

# Pack as dotnet tool
dotnet pack --configuration Release

# Install locally (from solution root)
dotnet tool install -g --add-source ./Guppi.Console/nupkg dotnet-guppi

# Update local install
dotnet tool update -g --add-source ./Guppi.Console/nupkg dotnet-guppi

# Run
guppi <skill> <command> [options]
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
Guppi.Tests/            # NUnit tests
dotnet-todo/            # Git submodule - todo.txt library
```

**Layer flow:** `Skill` (Console) -> `IService` -> `IProvider` (Core)

Each feature follows a consistent pattern:
- `Guppi.Console/Skills/{Name}Skill.cs` — defines CLI commands via `ISkill.GetCommands()`
- `Guppi.Core/Services/{Name}Service.cs` — implements `I{Name}Service`
- `Guppi.Core/Interfaces/Services/I{Name}Service.cs` — service contract
- Optional: `Guppi.Core/Providers/` for external integrations
- Optional: `Guppi.Core/Configurations/` for per-skill settings

Skills are registered in `Program.cs` via DI as `ISkill` implementations.
Services and providers are registered in `Guppi.Core/DependencyInjection.cs`.

## Key Dependencies

- **.NET 9.0** — target framework
- **System.CommandLine** (2.0.0-beta4) — CLI framework (beta, some APIs may be unstable)
- **Spectre.Console** — rich terminal output
- **Microsoft.Extensions.DependencyInjection** — DI container
- **NUnit 4** + **FluentAssertions 8** — testing
- **dotnet-todo** — git submodule, must be checked out (`git submodule update --init`)

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

## Gotchas

- The `dotnet-todo` directory is a **git submodule** — clone with `--recurse-submodules` or run `git submodule update --init`
  - **AGENTS:** Do not make changes to the todo sub-module without explicit user approval. It is maintained separately from the main Guppi project.
- System.CommandLine is a **pre-release beta** — avoid using APIs not already in the codebase
- `Guppi.Core` exposes internals to `Guppi.Tests` via `InternalsVisibleTo`
- CI runs on `ubuntu-latest` but the app targets Windows features (System.Speech, System.Management)
- NuGet packages are published to **GitHub Packages** on merge to main
- The solution uses the new `.slnx` format (not `.sln`)
