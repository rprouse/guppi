using System.Threading.Tasks;

namespace Guppi.Console
{
    internal interface IApplication
    {
        Task Run(string[] args);
    }
}
