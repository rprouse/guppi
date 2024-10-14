using Guppi.Core.Entities.Ascii;

namespace Guppi.Core.Interfaces.Services;

public interface IAsciiService
{
    AsciiData[] GetAsciiTable();
}
