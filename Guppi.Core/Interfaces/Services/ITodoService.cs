using System.Threading.Tasks;

namespace Guppi.Core.Interfaces.Services;

public interface ITodoService
{
    Task Sync();
}
