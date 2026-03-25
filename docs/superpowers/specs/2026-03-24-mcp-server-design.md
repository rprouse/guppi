# Guppi MCP Server & Multi-Tool Packaging

**Date:** 2026-03-24
**Version:** 9.0.0 (major bump from 8.0.0)

## Overview

Add a STDIO MCP server (`guppi.mcp`) to the Guppi project, sharing Guppi.Core's services. Switch from single-project `PackAsTool` packaging to a `.nuspec`-based multi-tool NuGet package so both `guppi` and `guppi.mcp` are distributed in the existing `dotnet-guppi` package.

## Goals

- Ship an MCP server alongside the existing CLI in a single NuGet tool package
- Establish the pattern for progressively exposing Guppi skills as MCP tools
- Start with a stub (Utilities: date/time, GUID) to validate the architecture
- Keep the build/CI pipeline simple using `dotnet pack` with a `.nuspec` hybrid

## Non-Goals

- HTTP/SSE transport (future — SDK supports it, no code changes needed)
- Exposing all skills as MCP tools (incremental, one service at a time)
- Refactoring existing services to remove Spectre.Console output (will happen incrementally as tools are added)

## Solution Structure

```
Directory.Build.props     # NEW — shared version (9.0.0) and metadata
Guppi.Console/            # Existing CLI (packaging metadata removed from csproj)
Guppi.Core/               # Existing shared business logic (unchanged)
Guppi.Tests/              # Existing tests (unchanged)
Guppi.MCP/                # NEW — STDIO MCP server
  Tools/                  # MCP tool classes (attribute-based)
  Program.cs              # Host builder with DI + STDIO transport
  Guppi.MCP.csproj        # Project file
Guppi.Package/            # NEW — Packaging-only project
  Guppi.Package.csproj    # References Console + MCP, uses NuspecFile
  dotnet-guppi.nuspec     # Manifest defining both tool commands
  DotnetToolSettings.xml  # Registers both tool commands
dotnet-todo/              # Existing submodule (unchanged)
```

**Layer flow:**
- `Skill` (Console) -> `IService` -> `IProvider` (Core)
- `Tool` (MCP) -> `IService` -> `IProvider` (Core)

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
  </PropertyGroup>
</Project>
```

All projects inherit version and metadata. Each executable can access its version at runtime via `Assembly.GetEntryAssembly().GetName().Version`.

## Guppi.MCP Project

**Target framework:** net10.0

**Dependencies:**
- `ModelContextProtocol` 1.1.0 (confirmed published 2026-03-06, targets net10.0)
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
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="ModelContextProtocol" Version="1.1.0" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="10.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Guppi.Core\Guppi.Core.csproj" />
  </ItemGroup>
</Project>
```

`AssemblyName` must be `guppi.mcp` to match the `EntryPoint` in `DotnetToolSettings.xml`. `CopyLocalLockFileAssemblies` ensures all transitive NuGet dependencies are copied to the build output directory, which is required because the `.nuspec` packages from `bin/` output rather than `publish/` output.

**Program.cs:**
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

The stub tools use `DateTime` and `Guid` directly — no service injection needed. The existing `IUtilitiesService` only has `RestartExplorer()`, and the date/time/GUID logic in `UtilitiesSkill` is already inline. As more tools are added that need real services, the MCP SDK resolves DI parameters automatically from the container.

**Note on Spectre.Console:** Existing services may write directly to the console. For the initial stub this is not an issue since the tools are self-contained. As more tools are added, services will be refactored to return data rather than writing output — moving Spectre.Console usage to the Console application layer where it belongs.

**Testing:** No new tests for the initial stub. Test coverage for MCP tools will be added as real service-backed tools are introduced. The stub is simple enough (two static methods with no dependencies) that it can be validated by running `guppi.mcp` and connecting an MCP client.

## Packaging (Approach C: .nuspec + dotnet pack Hybrid)

### Guppi.Package/Guppi.Package.csproj

This is a packaging-only project — it produces no assembly of its own. Its sole purpose is to drive `dotnet pack` with the `.nuspec` file. The project references ensure both Console and MCP are built before packing.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsPackable>true</IsPackable>
    <NuspecFile>dotnet-guppi.nuspec</NuspecFile>
    <NuspecProperties>
      version=$(Version);
      configuration=$(Configuration);
      consoleDir=$(MSBuildThisFileDirectory)..\Guppi.Console\bin\$(Configuration)\net10.0;
      mcpDir=$(MSBuildThisFileDirectory)..\Guppi.MCP\bin\$(Configuration)\net10.0
    </NuspecProperties>
  </PropertyGroup>

  <PropertyGroup>
    <PackageOutputPath>./nupkg</PackageOutputPath>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\Guppi.Console\Guppi.Console.csproj" />
    <ProjectReference Include="..\Guppi.MCP\Guppi.MCP.csproj" />
  </ItemGroup>
</Project>
```

Note: CI runs `dotnet build --configuration Release` on the full solution before `dotnet pack --no-build`, so both Console and MCP will already be built when packing occurs.

Configuration is parameterized — `dotnet pack` packs Debug, `dotnet pack --configuration Release` packs Release.

### Guppi.Package/dotnet-guppi.nuspec

```xml
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
  <metadata>
    <id>dotnet-guppi</id>
    <version>$version$</version>
    <authors>Rob Prouse</authors>
    <description>Guppi command line utility and MCP server</description>
    <license type="file">LICENSE</license>
    <icon>ackbar.png</icon>
    <projectUrl>https://github.com/rprouse/guppi</projectUrl>
    <repository type="git" url="https://github.com/rprouse/guppi" />
    <packageTypes>
      <packageType name="DotnetTool" />
    </packageTypes>
  </metadata>
  <files>
    <file src="$consoleDir$\**\*" target="tools/net10.0/any/guppi" />
    <file src="$mcpDir$\**\*" target="tools/net10.0/any/guppi.mcp" />
    <file src="DotnetToolSettings.xml" target="tools/net10.0/any" />
    <file src="..\LICENSE" target="" />
    <file src="..\img\ackbar.png" target="" />
  </files>
</package>
```

### Guppi.Package/DotnetToolSettings.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<DotNetCliTool Version="1">
  <Commands>
    <Command Name="guppi" EntryPoint="guppi/guppi.dll" Runner="dotnet" />
    <Command Name="guppi.mcp" EntryPoint="guppi.mcp/guppi.mcp.dll" Runner="dotnet" />
  </Commands>
</DotNetCliTool>
```

### Changes to Guppi.Console.csproj

Remove the following properties (now handled by Directory.Build.props and Guppi.Package):
- `PackAsTool`
- `ToolCommandName`
- `PackageId`
- `PackageOutputPath`
- `Version`
- `Authors`, `Company`, `Copyright`
- `PackageIcon`, `PackageLicenseFile`, `PackageProjectUrl`
- `RepositoryUrl`, `RepositoryType`
- `NeutralLanguage` (en-CA — move to Directory.Build.props if desired)
- NuGet `<None Include>` items for LICENSE and ackbar.png

Add:
- `<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>` — ensures all transitive NuGet dependencies are copied to the build output directory for packaging

Keep: `OutputType`, `TargetFramework`, `AssemblyName`, `ApplicationIcon`, `Product`, `Description`, project references, and package references.

**Note on package description:** The existing Guppi.Console.csproj has a longer Bobverse-themed description. The `.nuspec` uses a shorter description: "Guppi command line utility and MCP server". This is intentional — the NuGet package description should be functional. The Bobverse lore stays in the README.

## CI/CD Changes

File: `.github/workflows/continuous_integration.yml`

**Pack step:** Target Guppi.Package instead of the default project:
```yaml
- name: Package NuGet
  run: dotnet pack Guppi.Package/Guppi.Package.csproj --no-build --configuration Release
```

**Artifact upload:** Update path to `Guppi.Package/nupkg`:
```yaml
- name: Upload Artifacts
  uses: actions/upload-artifact@v4
  with:
    name: nupkg
    path: Guppi.Package/nupkg
```

**Publish step:** No change — `**/dotnet-guppi.*.nupkg` glob still matches.

## Files Changed Summary

### New Files
| File | Purpose |
|---|---|
| `Directory.Build.props` | Shared version (9.0.0) and metadata |
| `Guppi.MCP/Guppi.MCP.csproj` | MCP server project |
| `Guppi.MCP/Program.cs` | Host builder with STDIO transport + DI |
| `Guppi.MCP/Tools/UtilitiesTools.cs` | Stub tool: date/time, GUID |
| `Guppi.Package/Guppi.Package.csproj` | Packaging project with NuspecFile |
| `Guppi.Package/dotnet-guppi.nuspec` | Multi-tool package manifest |
| `Guppi.Package/DotnetToolSettings.xml` | Registers both tool commands |

### Modified Files
| File | Change |
|---|---|
| `Guppi.slnx` | Add Guppi.MCP and Guppi.Package projects |
| `Guppi.Console/Guppi.Console.csproj` | Remove PackAsTool, NuGet metadata, version |
| `.github/workflows/continuous_integration.yml` | Pack targets Guppi.Package, update artifact path |
| `AGENTS.md` | Architecture, commands, dependencies |
| `README.md` | MCP server section, updated install commands |

### Unchanged
- Guppi.Core
- Guppi.Tests
- dotnet-todo submodule
