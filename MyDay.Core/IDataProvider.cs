using System.CommandLine;
using System.Threading.Tasks;

namespace MyDay.Core
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
