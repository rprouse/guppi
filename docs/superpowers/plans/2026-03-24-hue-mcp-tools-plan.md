# Hue MCP Tools Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add six Philips Hue light control tools to the Guppi MCP server, reusing the existing `IHueLightService`.

**Architecture:** A single `HueLightsTools` class with non-static instance methods receives `IHueLightService` via constructor injection. Each tool method wraps a service call with error handling, returning domain entities on success or error strings on failure.

**Tech Stack:** .NET 10, ModelContextProtocol SDK 1.1.0, NUnit 4, FluentAssertions 8

**Spec:** `docs/superpowers/specs/2026-03-24-hue-mcp-tools-design.md`

---

## File Map

| File | Action | Responsibility |
|------|--------|---------------|
| `Guppi.MCP/Tools/HueLightsTools.cs` | Create | Six MCP tool methods for Hue light control |
| `Guppi.MCP/Program.cs` | Modify | Append `.WithTools<HueLightsTools>()` to the builder chain |
| `AGENTS.md` | Modify | Add MCP SDK return types section after `## Key Dependencies`, before `## Code Style` |

---

### Task 1: Create HueLightsTools with ListHueBridges

**Files:**
- Create: `Guppi.MCP/Tools/HueLightsTools.cs`

- [ ] **Step 1: Create HueLightsTools class with constructor and ListHueBridges**

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Guppi.Core.Entities.Hue;
using Guppi.Core.Interfaces.Services;
using ModelContextProtocol.Server;

namespace Guppi.MCP.Tools
{
    [McpServerToolType]
    public class HueLightsTools
    {
        private readonly IHueLightService _service;

        public HueLightsTools(IHueLightService service)
        {
            _service = service;
        }

        [McpServerTool, Description("Lists Philips Hue bridges found on the local network")]
        public async Task<object> ListHueBridges()
        {
            try
            {
                return await _service.ListBridges();
            }
            catch (ArgumentException)
            {
                return "Error: Hue Bridge not found";
            }
            catch (InvalidOperationException)
            {
                return "Error: Not registered with bridge. Run 'guppi hue register' from the CLI first.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
```

- [ ] **Step 2: Build to verify it compiles**

Run: `dotnet build Guppi.MCP/Guppi.MCP.csproj`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
git add Guppi.MCP/Tools/HueLightsTools.cs
git commit -m "feat: add HueLightsTools with ListHueBridges MCP tool"
```

---

### Task 2: Add ListHueLights tool

**Files:**
- Modify: `Guppi.MCP/Tools/HueLightsTools.cs`

- [ ] **Step 1: Add ListHueLights method**

Add this method to the `HueLightsTools` class. Note the `light == 0` default resolution and the no-op `WaitForUserInput` lambda:

```csharp
[McpServerTool, Description("Lists all Philips Hue lights and their current state (on/off, brightness, color)")]
public async Task<object> ListHueLights(
    [Description("IP address of the Hue Bridge. Leave empty to auto-discover.")] string ip = null)
{
    try
    {
        return await _service.ListLights(ip, _ => { });
    }
    catch (ArgumentException)
    {
        return "Error: Hue Bridge not found";
    }
    catch (InvalidOperationException)
    {
        return "Error: Not registered with bridge. Run 'guppi hue register' from the CLI first.";
    }
    catch (Exception ex)
    {
        return $"Error: {ex.Message}";
    }
}
```

- [ ] **Step 2: Build to verify it compiles**

Run: `dotnet build Guppi.MCP/Guppi.MCP.csproj`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
git add Guppi.MCP/Tools/HueLightsTools.cs
git commit -m "feat: add ListHueLights MCP tool"
```

---

### Task 3: Add TurnOnHueLight tool

**Files:**
- Modify: `Guppi.MCP/Tools/HueLightsTools.cs`

- [ ] **Step 1: Add the using directive and TurnOnHueLight method**

Add `using Guppi.Core.Services.Hue;` to the top of the file.

Add this method to the class:

```csharp
[McpServerTool, Description("Turns on a Philips Hue light with optional brightness and color")]
public async Task<object> TurnOnHueLight(
    [Description("IP address of the Hue Bridge. Leave empty to auto-discover.")] string ip = null,
    [Description("Light ID to target. 0 for all lights. Leave empty for default light.")] uint light = 0,
    [Description("Brightness percentage, 0-100.")] byte? brightness = null,
    [Description("Color as hex (FF0000 or #FF0000) or named color (red, blue).")] string color = null)
{
    try
    {
        uint targetLight = light == 0 ? _service.GetDefaultLight() : light;
        var command = new SetLightCommand
        {
            IpAddress = ip,
            On = true,
            Off = false,
            Alert = false,
            Brightness = brightness,
            Color = color,
            Light = targetLight,
            WaitForUserInput = _ => { }
        };
        await _service.SetLight(command);
        return await _service.ListLights(ip, _ => { });
    }
    catch (ArgumentException)
    {
        return "Error: Hue Bridge not found";
    }
    catch (InvalidOperationException)
    {
        return "Error: Not registered with bridge. Run 'guppi hue register' from the CLI first.";
    }
    catch (Exception ex)
    {
        return $"Error: {ex.Message}";
    }
}
```

- [ ] **Step 2: Build to verify it compiles**

Run: `dotnet build Guppi.MCP/Guppi.MCP.csproj`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
git add Guppi.MCP/Tools/HueLightsTools.cs
git commit -m "feat: add TurnOnHueLight MCP tool"
```

---

### Task 4: Add TurnOffHueLight tool

**Files:**
- Modify: `Guppi.MCP/Tools/HueLightsTools.cs`

- [ ] **Step 1: Add TurnOffHueLight method**

```csharp
[McpServerTool, Description("Turns off a Philips Hue light")]
public async Task<object> TurnOffHueLight(
    [Description("IP address of the Hue Bridge. Leave empty to auto-discover.")] string ip = null,
    [Description("Light ID to target. 0 for all lights. Leave empty for default light.")] uint light = 0)
{
    try
    {
        uint targetLight = light == 0 ? _service.GetDefaultLight() : light;
        var command = new SetLightCommand
        {
            IpAddress = ip,
            On = false,
            Off = true,
            Alert = false,
            Light = targetLight,
            WaitForUserInput = _ => { }
        };
        await _service.SetLight(command);
        return await _service.ListLights(ip, _ => { });
    }
    catch (ArgumentException)
    {
        return "Error: Hue Bridge not found";
    }
    catch (InvalidOperationException)
    {
        return "Error: Not registered with bridge. Run 'guppi hue register' from the CLI first.";
    }
    catch (Exception ex)
    {
        return $"Error: {ex.Message}";
    }
}
```

- [ ] **Step 2: Build to verify it compiles**

Run: `dotnet build Guppi.MCP/Guppi.MCP.csproj`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
git add Guppi.MCP/Tools/HueLightsTools.cs
git commit -m "feat: add TurnOffHueLight MCP tool"
```

---

### Task 5: Add AlertHueLight tool

**Files:**
- Modify: `Guppi.MCP/Tools/HueLightsTools.cs`

- [ ] **Step 1: Add AlertHueLight method**

```csharp
[McpServerTool, Description("Triggers an alert flash on a Philips Hue light")]
public async Task<object> AlertHueLight(
    [Description("IP address of the Hue Bridge. Leave empty to auto-discover.")] string ip = null,
    [Description("Light ID to target. 0 for all lights. Leave empty for default light.")] uint light = 0,
    [Description("Brightness percentage, 0-100.")] byte? brightness = null,
    [Description("Color as hex (FF0000 or #FF0000) or named color (red, blue).")] string color = null)
{
    try
    {
        uint targetLight = light == 0 ? _service.GetDefaultLight() : light;
        var command = new SetLightCommand
        {
            IpAddress = ip,
            On = false,
            Off = false,
            Alert = true,
            Brightness = brightness,
            Color = color,
            Light = targetLight,
            WaitForUserInput = _ => { }
        };
        await _service.SetLight(command);
        return await _service.ListLights(ip, _ => { });
    }
    catch (ArgumentException)
    {
        return "Error: Hue Bridge not found";
    }
    catch (InvalidOperationException)
    {
        return "Error: Not registered with bridge. Run 'guppi hue register' from the CLI first.";
    }
    catch (Exception ex)
    {
        return $"Error: {ex.Message}";
    }
}
```

- [ ] **Step 2: Build to verify it compiles**

Run: `dotnet build Guppi.MCP/Guppi.MCP.csproj`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
git add Guppi.MCP/Tools/HueLightsTools.cs
git commit -m "feat: add AlertHueLight MCP tool"
```

---

### Task 6: Add SetHueLight tool

**Files:**
- Modify: `Guppi.MCP/Tools/HueLightsTools.cs`

- [ ] **Step 1: Add SetHueLight method**

```csharp
[McpServerTool, Description("Sets the brightness and/or color of a Philips Hue light without changing its on/off state")]
public async Task<object> SetHueLight(
    [Description("IP address of the Hue Bridge. Leave empty to auto-discover.")] string ip = null,
    [Description("Light ID to target. 0 for all lights. Leave empty for default light.")] uint light = 0,
    [Description("Brightness percentage, 0-100.")] byte? brightness = null,
    [Description("Color as hex (FF0000 or #FF0000) or named color (red, blue).")] string color = null)
{
    try
    {
        uint targetLight = light == 0 ? _service.GetDefaultLight() : light;
        var command = new SetLightCommand
        {
            IpAddress = ip,
            On = false,
            Off = false,
            Alert = false,
            Brightness = brightness,
            Color = color,
            Light = targetLight,
            WaitForUserInput = _ => { }
        };
        await _service.SetLight(command);
        return await _service.ListLights(ip, _ => { });
    }
    catch (ArgumentException)
    {
        return "Error: Hue Bridge not found";
    }
    catch (InvalidOperationException)
    {
        return "Error: Not registered with bridge. Run 'guppi hue register' from the CLI first.";
    }
    catch (Exception ex)
    {
        return $"Error: {ex.Message}";
    }
}
```

- [ ] **Step 2: Build to verify it compiles**

Run: `dotnet build Guppi.MCP/Guppi.MCP.csproj`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
git add Guppi.MCP/Tools/HueLightsTools.cs
git commit -m "feat: add SetHueLight MCP tool"
```

---

### Task 7: Register HueLightsTools in MCP Program.cs

**Files:**
- Modify: `Guppi.MCP/Program.cs`

- [ ] **Step 1: Append `.WithTools<HueLightsTools>()` to the builder chain**

In `Guppi.MCP/Program.cs`, find the line `.WithTools<UtilitiesTools>();` and replace it with:

```csharp
    .WithTools<UtilitiesTools>()
    .WithTools<HueLightsTools>();
```

This is an append-only edit — change the `;` to a method chain continuation and add the new line.

- [ ] **Step 2: Build the full solution to verify everything wires up**

Run: `dotnet build`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
git add Guppi.MCP/Program.cs
git commit -m "feat: register HueLightsTools in MCP server"
```

---

### Task 8: Update AGENTS.md with MCP SDK return type docs

**Files:**
- Modify: `AGENTS.md`

- [ ] **Step 1: Add MCP tool return types section**

Insert a new section after `## Key Dependencies` (after its bullet list ends) and before `## Code Style`. Add:

```markdown
## MCP Tool Return Types

The `[McpServerTool]` method return type determines how the MCP SDK serializes the response:

- `string` → returned as a `TextContentBlock`
- Any other object → auto-serialized to JSON as text content
- `IEnumerable<ContentBlock>` → returned directly as content blocks
- `CallToolResult` → returned as-is for full control over the response

For tools that can return either structured data (success) or an error message (failure), use `Task<object>` as the return type. Return domain entities on success (auto-serialized to JSON) and a descriptive `string` on error.
```

- [ ] **Step 2: Commit**

```bash
git add AGENTS.md
git commit -m "docs: add MCP SDK return type documentation to AGENTS.md"
```

---

### Task 9: Final build and test verification

- [ ] **Step 1: Run full solution build**

Run: `dotnet build`
Expected: Build succeeded with no warnings in `Guppi.MCP`

- [ ] **Step 2: Run all tests**

Run: `dotnet test --no-restore --verbosity normal`
Expected: All tests pass (existing tests should not be affected)

- [ ] **Step 3: Verify MCP server starts without errors**

Run: `dotnet run --project Guppi.MCP/Guppi.MCP.csproj`
Expected: Server starts and waits for STDIO input (Ctrl+C to stop). No errors on stderr.
