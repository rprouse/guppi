using System;
using Guppi.Repl.Results;
using Repl;
using Repl.Mcp;
using Repl.Parameters;

namespace Guppi.Repl.Skills;

public sealed class UtilitiesModule(TimeProvider timeProvider) : IReplModule
{
    public void Map(IReplMap map)
    {
        map.Map("date", ([ReplOption(Aliases = ["-u"])] bool utc = false) =>
            {
                var now = utc ? timeProvider.GetUtcNow() : timeProvider.GetLocalNow();
                return new DateResult(DateOnly.FromDateTime(now.DateTime), utc);
            })
            .WithDescription("Get the current date")
            .ReadOnly();

        map.Map("guid", static () => new GuidResult(Guid.NewGuid()))
            .WithDescription("Create a new GUID")
            .ReadOnly();
    }
}
