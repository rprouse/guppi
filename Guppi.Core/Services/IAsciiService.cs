using Guppi.Core.Entities.Ascii;

namespace Guppi.Core.Services;

public interface IAsciiService
{
    AsciiData[] GetAsciiTable();
}
