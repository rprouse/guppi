using System.Collections.Generic;
using System.CommandLine;

namespace Guppi.Application
{
    public interface IMultipleActionProvider
    {
        /// <summary>
        /// Gets multiple commands that a provider supports
        /// </summary>
        /// <returns></returns>
        IEnumerable<Command> GetCommands();
    }
}
