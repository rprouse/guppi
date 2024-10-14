using Guppi.Domain.Entities.Ascii;

namespace Guppi.Core.Services;

public interface IAsciiService
{
    AsciiData[] GetAsciiTable();
}
