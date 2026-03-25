# Guppi MCP Server & Two-Package Distribution

**Date:** 2026-03-24
**Version:** 9.0.0 (major bump from 8.0.0)
**Status:** Implemented

## Overview

Add a STDIO MCP server (`guppi.mcp`) to the Guppi project, sharing Guppi.Core's services. Distributed as a second dotnet tool package (`dotnet-guppi-mcp`) alongside the existing CLI package (`dotnet-guppi`). Version and shared metadata centralized in `Directory.Build.props`.

## Design History

The original design proposed a single NuGet package containing both tools via a `.nuspec` manifest and `DotnetToolSettings.xml` with multiple `<Command>` entries. During implementation, we discovered that `dotnet tool install` does not support multiple commands per package ("More than one command is defined for the tool"). The design was revised to use two separate packages, each with `PackAsTool`.

## Goals

- Ship an MCP server alongside the existing CLI as separate dotnet tool packages
- Establish the pattern for progressively exposing Guppi skills as MCP tools
- Start with a stub (Utilities: date/time, GUID) to validate the architecture
- Centralize version and metadata across projects

## Non-Goals

- HTTP/SSE transport (future — SDK supports it via `ModelContextProtocol.AspNetCore`)
- Exposing all skills as MCP tools (incremental, one service at a time)
- Refactoring existing services to remove Spectre.Console output (will happen incrementally as tools are added)

## Solution Structure

```
Directory.Build.props     # Shared version (9.0.0) and metadata
Guppi.Console/            # CLI entry point (PackAsTool → dotnet-guppi)
Guppi.Core/               # Shared business logic (unchanged)
Guppi.Tests/              # NUnit tests (unchanged)
Guppi.MCP/                # STDIO MCP server (PackAsTool → dotnet-guppi-mcp)
  Tools/                  # MCP tool classes (attribute-based)
  Program.cs              # Host builder with DI + STDIO transport
dotnet-todo/              # Git submodule (unchanged)
```

**Layer flow:**
- `Skill` (Console) → `IService` → `IProvider` (Core)
- `Tool` (MCP) → `IService` → `IProvider` (Core)

Both Console and MCP are "heads" on top of Guppi.Core.

## Directory.Build.props

Placed at solution root. MSBuild automatically imports it into every project.

```xml
<Project>
  <PropertyGroup>
    <Version>9.0.0</Version>
    <Authors>Rob Prouse</Authors>
    <Company>Alteridem Consulting</Company>
    <Copyright>Copyright (c) 2025-2026 Rob Prouse</Copyright>
    <NeutralLanguage>en-CA</NeutralLanguage>
  </PropertyGroup>
</Project>
```

## Guppi.MCP Project

**Target framework:** net10.0

**Dependencies:**
- `ModelContextProtocol` 1.1.0 (official C# MCP SDK, Apache-2.0, targets net10.0)
- `Microsoft.Extensions.Hosting` 10.0.0
- Project reference to Guppi.Core

**Guppi.MCP/Guppi.MCP.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <AssemblyName>guppi.mcp</AssemblyName>
    <RootNamespace>Guppi.MCP</RootNamespace>
    <Product>Guppi MCP Server</Product>
    <Description>GUPPI MCP Server - exposes Guppi skills as Model Context Protocol tools for AI assistants.</Description>
    <PackageId>dotnet-guppi-mcp</PackageId>
    <PackAsTool>true</PackAsTool>
    <ToolCommandName>guppi.mcp</ToolCommandName>
    <PackageOutputPath>./nupkg</PackageOutputPath>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <PackageProjectUrl>https://github.com/rprouse/guppi</PackageProjectUrl>
    <RepositoryUrl>https://github.com/rprouse/guppi</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
    <PackageIcon>ackbar.png</PackageIcon>
  </PropertyGroup>
  ...
</Project>
```

**Program.cs** (top-level statements):
```csharp
var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddCore()
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<UtilitiesTools>();

await builder.Build().RunAsync();
```

**Tools/UtilitiesTools.cs:**
```csharp
[McpServerToolType]
public class UtilitiesTools
{
    [McpServerTool, Description("Gets the current date and time")]
    public static string GetDateTime()
    {
        return DateTime.Now.ToString("F");
    }

    [McpServerTool, Description("Generates a new GUID")]
    public static string GetGuid()
    {
        return Guid.NewGuid().ToString();
    }
}
```

The stub tools use `DateTime` and `Guid` directly — no service injection needed. As more tools are added, the MCP SDK resolves DI parameters automatically from the container.

## Packaging

Each project uses `PackAsTool` independently:

| Package | Project | Tool Command | Package ID |
|---|---|---|---|
| CLI | Guppi.Console | `guppi` | `dotnet-guppi` |
| MCP Server | Guppi.MCP | `guppi.mcp` | `dotnet-guppi-mcp` |

Both projects use `PackageLicenseExpression` (MIT) instead of file-based license. Version, authors, company, and copyright come from `Directory.Build.props`.

### Changes to Guppi.Console.csproj

- Version, Authors, Company, Copyright, NeutralLanguage → moved to Directory.Build.props
- PackageLicenseFile → replaced with PackageLicenseExpression (MIT)
- All other PackAsTool and NuGet metadata retained

## CI/CD Changes

File: `.github/workflows/continuous_integration.yml`

**Pack step:** Explicitly pack both projects:
```yaml
- name: Package NuGet
  run: |
    dotnet pack Guppi.Console/Guppi.Console.csproj --no-build --configuration Release
    dotnet pack Guppi.MCP/Guppi.MCP.csproj --no-build --configuration Release
```

**Artifact upload:** Both nupkg directories:
```yaml
- name: Upload Artifacts
  uses: actions/upload-artifact@v4
  with:
    name: nupkg
    path: |
      Guppi.Console/nupkg/*.nupkg
      Guppi.MCP/nupkg/*.nupkg
```

**Publish step:** Glob matches both packages:
```yaml
- name: Publish NuGet to GitHub Packages
  run: dotnet nuget push "**/*.nupkg" ...
```

## Files Changed Summary

### New Files
| File | Purpose |
|---|---|
| `Directory.Build.props` | Shared version (9.0.0) and metadata |
| `Guppi.MCP/Guppi.MCP.csproj` | MCP server project (PackAsTool) |
| `Guppi.MCP/Program.cs` | Host builder with STDIO transport + DI |
| `Guppi.MCP/Tools/UtilitiesTools.cs` | Stub tool: date/time, GUID |

### Modified Files
| File | Change |
|---|---|
| `Guppi.slnx` | Add Guppi.MCP project |
| `Guppi.Console/Guppi.Console.csproj` | Move shared metadata to Directory.Build.props, switch to MIT expression |
| `.github/workflows/continuous_integration.yml` | Pack and publish both packages |
| `AGENTS.md` | Architecture, commands, dependencies |
| `README.md` | MCP server section, updated install commands |

### Unchanged
- Guppi.Core
- Guppi.Tests
- dotnet-todo submodule
