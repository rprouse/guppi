# IP Skill to MCP Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Expose the existing IP address skill as MCP tools so AI assistants can query WAN and local IP addresses.

**Architecture:** The `IPService` is already clean (no Spectre.Console dependencies), so we only need to create a new `IPTools` MCP tool class that calls the existing `IIPService`, register it in the MCP server, and update the README. The `NetworkInterface` system type doesn't serialize cleanly to JSON, so the MCP tool will transform local interface data into simple objects before returning.

**Tech Stack:** .NET 10, ModelContextProtocol SDK 1.1.0, System.Net.NetworkInformation

---

## File Structure

| Action | File | Responsibility |
|--------|------|---------------|
| Create | `Guppi.MCP/Tools/IPTools.cs` | MCP tool class with `GetWanIpAddress` and `GetLocalIpAddresses` tools |
| Modify | `Guppi.MCP/Program.cs:18` | Register `IPTools` with `.WithTools<IPTools>()` |
| Modify | `README.md:160-164` | Add IP tools to MCP server tool list |
| Modify | `Directory.Build.props:3` | Bump version from 9.1.0 to 9.2.0 |

---

### Task 1: Create IPTools MCP Tool Class

**Files:**
- Create: `Guppi.MCP/Tools/IPTools.cs`

- [ ] **Step 1: Create `Guppi.MCP/Tools/IPTools.cs`**

```csharp
using System;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Guppi.Core.Interfaces.Services;
using ModelContextProtocol.Server;

namespace Guppi.MCP.Tools
{
    [McpServerToolType]
    public class IPTools
    {
        private readonly IIPService _service;

        public IPTools(IIPService service)
        {
            _service = service;
        }

        [McpServerTool, Description("Gets the public (WAN) IP address of the current machine")]
        public async Task<object> GetWanIpAddress()
        {
            try
            {
                var ip = await _service.GetWanIPAddress();
                if (ip == null)
                    return "Error: Could not determine WAN IP address";

                return new { WanIpAddress = ip.ToString() };
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        [McpServerTool, Description("Lists all local network interfaces and their IPv4 addresses")]
        public Task<object> GetLocalIpAddresses()
        {
            try
            {
                var interfaces = _service.GetNetworkInterfaces()
                    .Where(n => n.OperationalStatus == OperationalStatus.Up)
                    .SelectMany(n => n.GetIPProperties().UnicastAddresses
                        .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
                        .Select(a => new
                        {
                            InterfaceName = n.Name,
                            IpAddress = a.Address.ToString()
                        }))
                    .ToList();

                return Task.FromResult<object>(interfaces);
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>($"Error: {ex.Message}");
            }
        }
    }
}
```

- [ ] **Step 2: Build to verify it compiles**

Run: `dotnet build Guppi.MCP/Guppi.MCP.csproj`
Expected: Build succeeds (tool is not registered yet, but the class should compile)

- [ ] **Step 3: Commit**

```bash
git add Guppi.MCP/Tools/IPTools.cs
git commit -m "feat: add IP address MCP tools"
```

---

### Task 2: Register IPTools in MCP Server

**Files:**
- Modify: `Guppi.MCP/Program.cs:18`

- [ ] **Step 1: Add `.WithTools<IPTools>()` to the MCP server builder**

In `Guppi.MCP/Program.cs`, after line 18 (`.WithTools<HueLightsTools>()`), add:

```csharp
    .WithTools<IPTools>();
```

The final chain should be:
```csharp
builder.Services
    .AddCore()
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<UtilitiesTools>()
    .WithTools<HueLightsTools>()
    .WithTools<IPTools>();
```

- [ ] **Step 2: Build to verify registration**

Run: `dotnet build Guppi.MCP/Guppi.MCP.csproj`
Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add Guppi.MCP/Program.cs
git commit -m "feat: register IPTools in MCP server"
```

---

### Task 3: Update README and Bump Version

**Files:**
- Modify: `README.md:160-164`
- Modify: `Directory.Build.props:3`

- [ ] **Step 1: Update the MCP tools list in README.md**

Replace lines 160-164:
```markdown
The MCP server currently exposes:
- **GetDateTime** — Returns the current date and time
- **GetGuid** — Generates a new GUID

More tools will be added progressively.
```

With:
```markdown
The MCP server currently exposes:
- **GetDateTime** — Returns the current date and time
- **GetGuid** — Generates a new GUID
- **ListHueBridges** — Lists Philips Hue bridges on the local network
- **ListHueLights** — Lists all Hue lights and their current state
- **TurnOnHueLight** — Turns on a Hue light with optional brightness and color
- **TurnOffHueLight** — Turns off a Hue light
- **AlertHueLight** — Triggers an alert flash on a Hue light
- **SetHueLight** — Sets brightness and/or color without changing on/off state
- **GetWanIpAddress** — Gets the public (WAN) IP address
- **GetLocalIpAddresses** — Lists local network interfaces and their IPv4 addresses

More tools will be added progressively.
```

- [ ] **Step 2: Bump version in `Directory.Build.props` from 9.1.0 to 9.2.0**

This is a minor version bump (new feature, no breaking changes).

- [ ] **Step 3: Build the full solution**

Run: `dotnet build`
Expected: Build succeeds

- [ ] **Step 4: Run tests**

Run: `dotnet test`
Expected: All tests pass

- [ ] **Step 5: Commit**

```bash
git add README.md Directory.Build.props
git commit -m "docs: update README MCP tools list and bump version to 9.2.0"
```
