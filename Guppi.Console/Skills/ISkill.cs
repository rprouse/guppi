using System.Collections.Generic;
using System.CommandLine;

namespace Guppi.Console.Skills;

public interface ISkill
{
    /// <summary>
    /// Gets multiple commands that a provider supports
    /// </summary>
    /// <returns></returns>
    IEnumerable<Command> GetCommands();
}
