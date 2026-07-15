namespace Guppi.Repl.Results;

public sealed record HueBridgeResult(string BridgeId, string IpAddress);

public sealed record HueLightResult(
    string Id,
    string Name,
    bool On,
    byte Brightness,
    string Color,
    string Type);

public sealed record HueActionResult(
    string LightId,
    string LightName,
    bool On,
    byte? Brightness,
    string Color,
    string IpAddress);
