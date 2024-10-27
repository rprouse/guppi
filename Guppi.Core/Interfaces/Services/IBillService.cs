using System.Threading.Tasks;

namespace Guppi.Core.Interfaces.Services;

public interface IBillService
{
    Task DownloadAllBills();
    Task DownloadAlectraBills();
    Task DownloadEnbridgeBills();
    void Configure();
}
