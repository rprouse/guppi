using Guppi.Domain.Entities.Ascii;

namespace Guppi.Application.Services;

public interface IAsciiService
{
    AsciiData[] GetAsciiTable();
}
