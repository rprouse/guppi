using System.Threading.Tasks;

namespace Guppi.Core.Services;

public interface ITodoService
{
    Task Sync();
}
