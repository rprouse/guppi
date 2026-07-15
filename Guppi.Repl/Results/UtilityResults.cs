using System;

namespace Guppi.Repl.Results;

public sealed record DateResult(DateOnly Value, bool IsUtc);

public sealed record GuidResult(Guid Value);
