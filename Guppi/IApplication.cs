using System.Threading.Tasks;

namespace Alteridem.Guppi
{
    internal interface IApplication
    {
        Task Run(string[] args);
    }
}
