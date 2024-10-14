using System;

namespace Guppi.Core.Services.Hue;

public sealed class SetLightCommand
{
    public string IpAddress { get; init; }

    public bool On { get; init; }

    public bool Off { get; init; }

    public bool Alert { get; init; }

    public byte? Brightness { get; init; }

    public string Color { get; init; }

    public uint Light { get; init; }

    public Action<string> WaitForUserInput { get; set; }
}
