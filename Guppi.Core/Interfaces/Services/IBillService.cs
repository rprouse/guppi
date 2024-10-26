using System.Threading.Tasks;

namespace Guppi.Core.Interfaces.Services;

public interface IBillService
{
    Task DownloadAlectraBills();
    void Configure();
}
