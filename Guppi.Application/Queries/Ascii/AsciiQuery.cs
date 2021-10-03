using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Ascii;
using MediatR;

namespace Guppi.Application.Queries.Ascii
{
    public sealed class AsciiQuery : IRequest<AsciiData[]>
    {
    }

    internal sealed class AsciiQueryHandler : IRequestHandler<AsciiQuery, AsciiData[]>
    {
        public Task<AsciiData[]> Handle(AsciiQuery request, CancellationToken cancellationToken)
        {
            var result = new List<AsciiData>(128)
            {
                new AsciiData(0, "NUL", "Null"),
                new AsciiData(1, "SOH", "Start of Header"),
                new AsciiData(2, "STX", "Start of Text"),
                new AsciiData(3, "ETX", "End of Text"),
                new AsciiData(4, "EOT", "End of Transmission"),
                new AsciiData(5, "ENQ", "Enquiry"),
                new AsciiData(6, "ACK", "Acknowledge"),
                new AsciiData(7, "BEL", "Bell"),
                new AsciiData(8, "BS", "Backspace"),
                new AsciiData(9, "HT", "Horizontal Tab"),
                new AsciiData(10, "LF", "Line Feed"),
                new AsciiData(11, "VT", "Vertical Tab"),
                new AsciiData(12, "FF", "Form Feed"),
                new AsciiData(13, "CR", "Carriage Return"),
                new AsciiData(14, "SO", "Shift Out"),
                new AsciiData(15, "SI", "Shift In"),
                new AsciiData(16, "DLE", "Data Link Escape"),
                new AsciiData(17, "DC1", "Device Control 1"),
                new AsciiData(18, "DC2", "Device Control 2"),
                new AsciiData(19, "DC3", "Device Control 3"),
                new AsciiData(20, "DC4", "Device Control 4"),
                new AsciiData(21, "NAK", "Negative Acknowledge"),
                new AsciiData(22, "SYN", "Synchronize"),
                new AsciiData(23, "ETB", "End of Transmission Block"),
                new AsciiData(24, "CAN", "Cancel"),
                new AsciiData(25, "EM", "End of Medium"),
                new AsciiData(26, "SUB", "Substitute"),
                new AsciiData(27, "ESC", "Escape"),
                new AsciiData(28, "FS", "File Separator"),
                new AsciiData(29, "GS", "Group Separator"),
                new AsciiData(30, "RS", "Record Separator"),
                new AsciiData(31, "US", "Unit Separator"),
                new AsciiData(32, "SPACE", "Space")
            };

            for (char c = '!'; c <= '~'; c++)
            {
                result.Add(new AsciiData(c, $"{c}", ""));
            }
            result.Add(new AsciiData(127, "DEL", "Delete"));

            return Task.FromResult(result.ToArray());
        }
    }
}
