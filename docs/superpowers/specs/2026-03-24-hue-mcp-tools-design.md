# Hue MCP Tools Design

## Summary

Add Philips Hue light control tools to the Guppi MCP server (`Guppi.MCP`), exposing the existing `IHueLightService` functionality as six MCP tools. This follows the established pattern of CLI skills and MCP tools sharing the same service layer.

## Tools

Six tools in a single `HueLightsTools` class:

| Tool | Parameters | Returns | Description |
|------|-----------|---------|-------------|
| `ListHueBridges` | — | `IEnumerable<HueBridge>` | Lists Hue bridges found on the network |
| `ListHueLights` | `ip?` | `IEnumerable<HueLight>` | Lists all lights and their current state |
| `TurnOnHueLight` | `ip?`, `light?`, `brightness?`, `color?` | `IEnumerable<HueLight>` | Turns on a light with optional brightness/color |
| `TurnOffHueLight` | `ip?`, `light?` | `IEnumerable<HueLight>` | Turns off a light |
| `AlertHueLight` | `ip?`, `light?`, `brightness?`, `color?` | `IEnumerable<HueLight>` | Triggers an alert flash on a light |
| `SetHueLight` | `ip?`, `light?`, `brightness?`, `color?` | `IEnumerable<HueLight>` | Sets brightness/color without changing on/off state |

### Parameters

- **`ip`** (`string?`): IP address of the Hue Bridge. Defaults to auto-discovering the first bridge on the network.
- **`light`** (`uint`): The light ID to target. Defaults to the user's configured default light. `0` targets all lights.
- **`brightness`** (`byte?`): Brightness as a percentage, 0–100.
- **`color`** (`string?`): Color as hex (`FF0000` or `#FF0000`) or a named color (`red`, `blue`).

### Return Values

- **Success:** Returns domain entity objects (`HueBridge`, `HueLight`) which the MCP SDK auto-serializes to JSON.
- **Action tools** (on/off/alert/set): After executing the action, call `ListLights` to return the updated light states as confirmation.

### Error Handling

All exceptions are caught and returned as descriptive `string` values (rendered as text content by the SDK):

- Bridge not found: `"Error: Hue Bridge not found"`
- Not registered: `"Error: Not registered with bridge. Run 'guppi hue register' from the CLI first."`
- Other: `"Error: {exception.Message}"`

## Registration & Bridge Pairing

The `Register` and `Configure` commands from the CLI skill are **not** exposed as MCP tools. Bridge registration requires the user to physically press a button on the Hue bridge, which is interactive and not suitable for MCP.

If a tool is called and the bridge is not registered, the error message directs the user to run `guppi hue register` from the CLI.

## WaitForUserInput Handling

The `IHueLightService.ListLights()` and `SetLight()` methods accept an `Action<string>` callback used during auto-registration. MCP tools pass a no-op lambda `(_ => { })`. If registration is actually needed, the provider throws `InvalidOperationException`, which is caught and returned as the registration error message.

## DI & Wiring

- `HueLightsTools` receives `IHueLightService` via constructor injection (supported by MCP SDK).
- `IHueLightService` and `IHueProvider` are already registered in `Guppi.Core/DependencyInjection.cs`.
- Registration in `Guppi.MCP/Program.cs`: add `.WithTools<HueLightsTools>()` to the builder chain.

## Files Changed

| File | Change |
|------|--------|
| `Guppi.MCP/Tools/HueLightsTools.cs` | **New** — six MCP tool methods |
| `Guppi.MCP/Program.cs` | Add `.WithTools<HueLightsTools>()` |
| `AGENTS.md` | Add MCP SDK return type documentation |

## AGENTS.md Update

Add documentation about MCP SDK supported return types to the existing MCP/architecture section:

- `string` → `TextContentBlock`
- Any other object → auto-serialized to JSON as text content
- `IEnumerable<ContentBlock>` → returned directly
- `CallToolResult` → returned as-is for full control
- On error, return a `string` with a descriptive message

## No Changes To

- `Guppi.Core` — service, provider, entities, configurations remain as-is
- `Guppi.Console` — CLI skill unchanged
- `Guppi.Core/DependencyInjection.cs` — already registers all needed types
