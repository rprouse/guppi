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
}
