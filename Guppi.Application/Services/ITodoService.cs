using System.Threading.Tasks;

namespace Guppi.Application.Services;

public interface ITodoService
{
    Task Sync();
}
