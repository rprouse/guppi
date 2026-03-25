# Guppi MCP Server Implementation Plan

> **HISTORICAL DOCUMENT** — This plan was executed on 2026-03-24 but the packaging
> approach changed during implementation. **Do not follow these steps.** The final
> implementation is documented in the
> [design spec](../specs/2026-03-24-mcp-server-design.md) and the
> `feature/mcp-server` branch commits.

**Status:** Completed (2026-03-24) — with significant deviation from original packaging approach.

**Goal:** Add a STDIO MCP server to the Guppi project, distributed as a separate dotnet tool package (`dotnet-guppi-mcp`).

**Tech Stack:** .NET 10.0, ModelContextProtocol 1.1.0, Microsoft.Extensions.Hosting 10.0.0

## What Changed From This Plan

Tasks 1-2 were executed as written. Tasks 3-11 below describe a `.nuspec`-based
`Guppi.Package` approach that **failed** during Task 7 — `dotnet tool install`
does not support multiple commands per package. The final implementation:

- Uses `PackAsTool` on each project instead of a shared `.nuspec` manifest
- Ships two packages: `dotnet-guppi` (CLI) and `dotnet-guppi-mcp` (MCP server)
- Has no `Guppi.Package` project, no `.nuspec`, no `DotnetToolSettings.xml`
- Does not use `CopyLocalLockFileAssemblies` (not needed with `PackAsTool`)

See the [updated spec](../specs/2026-03-24-mcp-server-design.md) for the final architecture.

---

## Original Plan (for historical reference)

The tasks below are preserved as-is to document what was originally planned.
Checkboxes are left unchecked intentionally — they do not reflect the actual work done.

---

### Task 1: Create feature branch

**Files:** None

- [ ] **Step 1: Create and switch to feature branch**

```bash
git checkout -b feature/mcp-server
```

- [ ] **Step 2: Verify branch**

```bash
git branch --show-current
```
Expected: `feature/mcp-server`

---

### Task 2: Add Directory.Build.props (centralized version)

**Files:**
- Create: `Directory.Build.props`

- [ ] **Step 1: Create Directory.Build.props at solution root**

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

- [ ] **Step 2: Verify it builds**

```bash
dotnet build
```
Expected: Build succeeds. The version in Directory.Build.props will be inherited by all projects.

- [ ] **Step 3: Commit**

```bash
git add Directory.Build.props
git commit -m "feat: add Directory.Build.props with centralized version 9.0.0"
```

---

### Task 3: Strip packaging metadata from Guppi.Console.csproj

**Files:**
- Modify: `Guppi.Console/Guppi.Console.csproj`

This task removes properties that are now handled by Directory.Build.props (version, authors, company, copyright) and Guppi.Package (PackAsTool, NuGet metadata). It also adds `CopyLocalLockFileAssemblies` for the new packaging approach.

- [ ] **Step 1: Remove packaging and metadata properties from Guppi.Console.csproj**

Remove these lines/properties from the first `<PropertyGroup>` (lines 8-24):
- `Authors` (line 8)
- `Company` (line 9)
- `Copyright` (line 11)
- `PackageLicenseFile` (line 12)
- `PackageProjectUrl` (line 13)
- `RepositoryUrl` (line 14)
- `PackageId` (line 15)
- `Version` (line 16)
- `PackAsTool` (line 17)
- `ToolCommandName` (line 18)
- `PackageOutputPath` (line 19)
- `NeutralLanguage` (line 20)
- `RepositoryType` (line 21)
- `PackageIcon` (line 24)

Add `CopyLocalLockFileAssemblies`:
```xml
<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
```

Remove the `<None Include>` ItemGroups for ackbar.png (lines 27-36, the entire ItemGroup) and LICENSE (lines 38-43, the entire ItemGroup).

Note: `NeutralLanguage` (en-CA) is intentionally dropped — it was only relevant for NuGet package metadata which now lives in the `.nuspec`.

Keep: `OutputType`, `TargetFramework`, `RootNamespace`, `AssemblyName`, `Product`, `Description`, `ApplicationIcon`, the `<Compile Remove="nupkg\**" />` items, package references, and project reference.

The resulting file should look like:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>Guppi.Console</RootNamespace>
    <AssemblyName>guppi</AssemblyName>
    <Product>Guppi command line utility</Product>
    <Description>GUPPI (or General Unit Primary Peripheral Interface) Is a semi-sentient software being that helps Replicants interact with the many systems they have at their disposal. This is an early implementation of the interface and as this is not the year 2133 in the fictional Bobverse, GUPPI is not actually semi-sentient and is only a command line utility to provide me with the information I need.</Description>
    <ApplicationIcon>guppi.ico</ApplicationIcon>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
  </PropertyGroup>

  <ItemGroup>
    <Compile Remove="nupkg\**" />
    <EmbeddedResource Remove="nupkg\**" />
    <None Remove="nupkg\**" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
    <PackageReference Include="Spectre.Console" Version="0.50.0" />
    <PackageReference Include="System.CommandLine" Version="2.0.0-beta4.22272.1" />
    <PackageReference Include="System.CommandLine.NamingConventionBinder" Version="2.0.0-beta4.22272.1" />
    <PackageReference Include="TextCopy" Version="6.2.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Guppi.Core\Guppi.Core.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 2: Verify it builds and tests pass**

```bash
dotnet build && dotnet test
```
Expected: Build and tests succeed. The project no longer produces a `.nupkg` on its own.

- [ ] **Step 3: Commit**

```bash
git add Guppi.Console/Guppi.Console.csproj
git commit -m "refactor: strip packaging metadata from Guppi.Console.csproj

Version, authors, copyright now come from Directory.Build.props.
PackAsTool and NuGet metadata move to Guppi.Package.
Added CopyLocalLockFileAssemblies for nuspec-based packaging."
```

---

### Task 4: Create Guppi.MCP project with stub tools

**Files:**
- Create: `Guppi.MCP/Guppi.MCP.csproj`
- Create: `Guppi.MCP/Program.cs`
- Create: `Guppi.MCP/Tools/UtilitiesTools.cs`

- [ ] **Step 1: Create project directory**

```bash
mkdir -p Guppi.MCP/Tools
```

- [ ] **Step 2: Create Guppi.MCP.csproj**

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

- [ ] **Step 3: Create Program.cs**

Follow the project's code style: block-scoped namespaces, Allman braces, 4-space indentation. However, since Program.cs in Guppi.Console uses top-level statements, follow that same pattern here.

```csharp
using Guppi.Core;
using Guppi.MCP.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddCore()
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<UtilitiesTools>();

await builder.Build().RunAsync();
```

- [ ] **Step 4: Create Tools/UtilitiesTools.cs**

Follow the project's code style: block-scoped namespaces, Allman braces, 4-space indentation.

```csharp
using System;
using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Guppi.MCP.Tools
{
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
}
```

- [ ] **Step 5: Verify it builds**

```bash
dotnet build Guppi.MCP/Guppi.MCP.csproj
```
Expected: Build succeeds. Output includes `guppi.mcp.dll` in the bin directory.

- [ ] **Step 6: Commit**

```bash
git add Guppi.MCP/
git commit -m "feat: add Guppi.MCP STDIO MCP server with stub utilities tools

New project sharing Guppi.Core services. Exposes GetDateTime and GetGuid
as MCP tools via ModelContextProtocol 1.1.0 SDK."
```

---

### Task 5: Create Guppi.Package packaging project

**Files:**
- Create: `Guppi.Package/Guppi.Package.csproj`
- Create: `Guppi.Package/dotnet-guppi.nuspec`
- Create: `Guppi.Package/DotnetToolSettings.xml`

- [ ] **Step 1: Create project directory**

```bash
mkdir -p Guppi.Package
```

- [ ] **Step 2: Create Guppi.Package.csproj**

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
    <PackageOutputPath>./nupkg</PackageOutputPath>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\Guppi.Console\Guppi.Console.csproj" />
    <ProjectReference Include="..\Guppi.MCP\Guppi.MCP.csproj" />
  </ItemGroup>

</Project>
```

This is a packaging-only project — `EnableDefaultCompileItems` is false because there is no source code to compile.

- [ ] **Step 3: Create dotnet-guppi.nuspec**

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

- [ ] **Step 4: Create DotnetToolSettings.xml**

```xml
<?xml version="1.0" encoding="utf-8"?>
<DotNetCliTool Version="1">
  <Commands>
    <Command Name="guppi" EntryPoint="guppi/guppi.dll" Runner="dotnet" />
    <Command Name="guppi.mcp" EntryPoint="guppi.mcp/guppi.mcp.dll" Runner="dotnet" />
  </Commands>
</DotNetCliTool>
```

- [ ] **Step 5: Build and pack**

```bash
dotnet build --configuration Release && dotnet pack Guppi.Package/Guppi.Package.csproj --no-build --configuration Release
```
Expected: Build succeeds and produces `Guppi.Package/nupkg/dotnet-guppi.9.0.0.nupkg`.

- [ ] **Step 6: Verify package contents**

```bash
dotnet nuget locals all -l
```
Then inspect the package:
```bash
unzip -l Guppi.Package/nupkg/dotnet-guppi.9.0.0.nupkg | head -50
```
Expected: Package contains `tools/net10.0/any/guppi/guppi.dll`, `tools/net10.0/any/guppi.mcp/guppi.mcp.dll`, `tools/net10.0/any/DotnetToolSettings.xml`, `LICENSE`, and `ackbar.png`.

- [ ] **Step 7: Commit**

```bash
git add Guppi.Package/
git commit -m "feat: add Guppi.Package for multi-tool NuGet packaging

Uses .nuspec manifest with dotnet pack to ship both guppi and guppi.mcp
as tool commands in a single dotnet-guppi package."
```

---

### Task 6: Update solution file

**Files:**
- Modify: `Guppi.slnx`

- [ ] **Step 1: Add both new projects to the solution**

Add to `Guppi.slnx` after the last `<Project>` element (the `Guppi.Tests` entry):

```xml
  <Project Path="Guppi.MCP/Guppi.MCP.csproj" />
  <Project Path="Guppi.Package/Guppi.Package.csproj" />
```

- [ ] **Step 2: Verify full solution builds**

```bash
dotnet build
```
Expected: All projects build successfully including Guppi.MCP and Guppi.Package.

- [ ] **Step 3: Run tests**

```bash
dotnet test
```
Expected: All existing tests pass.

- [ ] **Step 4: Commit**

```bash
git add Guppi.slnx
git commit -m "feat: add Guppi.MCP and Guppi.Package to solution"
```

---

### Task 7: Test local tool installation

**Files:** None (validation only)

- [ ] **Step 1: Pack the tool**

```bash
dotnet build --configuration Release && dotnet pack Guppi.Package/Guppi.Package.csproj --no-build --configuration Release
```

- [ ] **Step 2: Install locally**

```bash
dotnet tool install -g --add-source ./Guppi.Package/nupkg dotnet-guppi
```
If already installed, use update:
```bash
dotnet tool update -g --add-source ./Guppi.Package/nupkg dotnet-guppi
```
Expected: Tool installs successfully. Should report installing version 9.0.0.

- [ ] **Step 3: Verify both commands exist**

```bash
guppi --help
```
Expected: Shows guppi help output.

```bash
guppi.mcp --help
```
Expected: MCP server starts (it uses STDIO, so `--help` may not work the same way). At minimum, verify the command is found and executes without crashing. Use Ctrl+C to stop if it starts waiting for STDIO input.

- [ ] **Step 4: Verify guppi commands still work**

```bash
guppi time
guppi guid
```
Expected: Both commands produce output as before.

---

### Task 8: Update CI/CD workflow

**Files:**
- Modify: `.github/workflows/continuous_integration.yml`

- [ ] **Step 1: Update the pack step (line 51)**

Change:
```yaml
    - name: 📦 Package NuGet
      run: dotnet pack --no-build --configuration Release
```

To:
```yaml
    - name: 📦 Package NuGet
      run: dotnet pack Guppi.Package/Guppi.Package.csproj --no-build --configuration Release
```

- [ ] **Step 2: Update the artifact upload path (line 57)**

Change:
```yaml
        path: Guppi.Console/nupkg/*.nupkg
```

To:
```yaml
        path: Guppi.Package/nupkg/*.nupkg
```

- [ ] **Step 3: Verify publish step glob still works (line 77)**

The existing glob `**/dotnet-guppi.*.nupkg` will match from the new location. No change needed.

- [ ] **Step 4: Commit**

```bash
git add .github/workflows/continuous_integration.yml
git commit -m "ci: update workflow to pack from Guppi.Package

Pack step now targets Guppi.Package/Guppi.Package.csproj.
Artifact upload path updated to Guppi.Package/nupkg."
```

---

### Task 9: Update AGENTS.md

**Files:**
- Modify: `AGENTS.md`

- [ ] **Step 1: Update Commands section**

Replace the pack and install commands (lines 15-21):

```markdown
# Pack as dotnet tool
dotnet pack Guppi.Package/Guppi.Package.csproj --configuration Release

# Install locally (from solution root)
dotnet tool install -g --add-source ./Guppi.Package/nupkg dotnet-guppi

# Update local install
dotnet tool update -g --add-source ./Guppi.Package/nupkg dotnet-guppi

# Run
guppi <skill> <command> [options]
guppi.mcp  # Starts the MCP server (STDIO)
```

- [ ] **Step 2: Update Architecture section**

Replace the architecture tree (lines 29-42) with:

```markdown
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
Guppi.MCP/              # MCP server entry point (STDIO transport)
  Tools/                # MCP tool classes (attribute-based)
  Program.cs            # DI composition root
Guppi.Package/          # NuGet packaging (no code, .nuspec manifest)
Guppi.Tests/            # NUnit tests
dotnet-todo/            # Git submodule - todo.txt library
```

- [ ] **Step 3: Update Layer flow (line 44)**

Replace with:

```markdown
**Layer flow:**
- `Skill` (Console) -> `IService` -> `IProvider` (Core)
- `Tool` (MCP) -> `IService` -> `IProvider` (Core)
```

- [ ] **Step 4: Add ModelContextProtocol to Key Dependencies (after line 63)**

Add to the Key Dependencies list:

```markdown
- **ModelContextProtocol** (1.1.0) — MCP server SDK (STDIO + HTTP transports)
```

- [ ] **Step 5: Update Gotchas section**

Update the NuGet packages note (line 92) to:

```markdown
- NuGet packages are published to **GitHub Packages** on merge to main — packaging is driven by `Guppi.Package/` using a `.nuspec` manifest
```

- [ ] **Step 6: Commit**

```bash
git add AGENTS.md
git commit -m "docs: update AGENTS.md for MCP server and new packaging"
```

---

### Task 10: Update README.md

**Files:**
- Modify: `README.md`

- [ ] **Step 1: Update .NET SDK requirement (line 143)**

Change:
```markdown
the [.NET SDK](https://dotnet.microsoft.com/download) to be installed. .NET 9.0 or newer is recommended.
```

To:
```markdown
the [.NET SDK](https://dotnet.microsoft.com/download) to be installed. .NET 10.0 or newer is required.
```

- [ ] **Step 2: Add MCP Server section before Installation (before line 140)**

Insert a new section:

## MCP Server

Guppi includes an MCP (Model Context Protocol) server that exposes Guppi skills as tools
for AI assistants like Claude. The MCP server uses STDIO transport.

### Configuring in Claude Code

Add the following to your Claude Code MCP settings:

```json
{
  "mcpServers": {
    "guppi": {
      "command": "guppi.mcp"
    }
  }
}
```

The MCP server currently exposes:
- **GetDateTime** — Returns the current date and time
- **GetGuid** — Generates a new GUID

More tools will be added progressively.

- [ ] **Step 3: Update local testing install commands (lines 152, 158)**

Change both occurrences of `./Guppi.Console/nupkg` to `./Guppi.Package/nupkg`:

```sh
dotnet tool install -g --add-source ./Guppi.Package/nupkg dotnet-guppi
```

```sh
dotnet tool update -g --add-source ./Guppi.Package/nupkg dotnet-guppi
```

- [ ] **Step 4: Update GitHub Packages section (line 163)**

Change:
```markdown
Whenever the version is updated in `Guppi/Guppi.csproj`, a merge to master will publish the NuGet package
```

To:
```markdown
Whenever the version is updated in `Directory.Build.props`, a merge to main will publish the NuGet package
```

- [ ] **Step 5: Commit**

```bash
git add README.md
git commit -m "docs: update README for MCP server and new packaging paths"
```

---

### Task 11: Final verification

**Files:** None (validation only)

- [ ] **Step 1: Clean build from scratch**

```bash
dotnet clean && dotnet build --configuration Release
```
Expected: Clean build succeeds for all projects.

- [ ] **Step 2: Run all tests**

```bash
dotnet test --no-restore --verbosity normal
```
Expected: All tests pass.

- [ ] **Step 3: Pack and verify**

```bash
dotnet pack Guppi.Package/Guppi.Package.csproj --no-build --configuration Release
```
Expected: Produces `Guppi.Package/nupkg/dotnet-guppi.9.0.0.nupkg`.

- [ ] **Step 4: Verify package contents**

```bash
unzip -l Guppi.Package/nupkg/dotnet-guppi.9.0.0.nupkg | grep -E "(guppi\.dll|guppi\.mcp\.dll|DotnetToolSettings|LICENSE|ackbar)"
```
Expected: All five key files present in the package.

- [ ] **Step 5: Install and smoke test**

```bash
dotnet tool update -g --add-source ./Guppi.Package/nupkg dotnet-guppi
guppi time
guppi guid
```
Expected: Both commands work. Version should report 9.0.0.
