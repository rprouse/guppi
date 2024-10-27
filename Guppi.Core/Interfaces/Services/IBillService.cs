using System.Threading.Tasks;

namespace Guppi.Core.Interfaces.Services;

public interface IBillService
{
    Task DownloadAllBills(int months);
    Task DownloadAlectraBills(int months);
    Task DownloadEnbridgeBills(int months);
    void InstallPlaywright();
    void Configure();
}
