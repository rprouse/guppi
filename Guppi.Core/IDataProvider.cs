using System.Collections.Generic;
using System.CommandLine;
using System.Threading.Tasks;

namespace Guppi.Core
{
    public interface IDataProvider
    {
        /// <summary>
        /// Gets the command line Command for this provider
        /// </summary>
        /// <returns></returns>
        Command GetCommand();
    }

    public interface IMultipleDataProvider
    {
        /// <summary>
        /// Gets multiple commands that a provider supports
        /// </summary>
        /// <returns></returns>
        IEnumerable<Command> GetCommands();
    }
}
