using System.Collections.Generic;
using System.Linq;
using Guppi.Core.Entities.Ascii;

namespace Guppi.Core.Services;

internal sealed class AsciiService : IAsciiService
{
    public AsciiData[] GetAsciiTable()
    {
        var result = new List<AsciiData>(128)
        {
            new (0, "NUL", "Null"),
            new (1, "SOH", "Start of Header"),
            new (2, "STX", "Start of Text"),
            new (3, "ETX", "End of Text"),
            new (4, "EOT", "End of Transmission"),
            new (5, "ENQ", "Enquiry"),
            new (6, "ACK", "Acknowledge"),
            new (7, "BEL", "Bell"),
            new (8, "BS", "Backspace"),
            new (9, "HT", "Horizontal Tab"),
            new (10, "LF", "Line Feed"),
            new (11, "VT", "Vertical Tab"),
            new (12, "FF", "Form Feed"),
            new (13, "CR", "Carriage Return"),
            new (14, "SO", "Shift Out"),
            new (15, "SI", "Shift In"),
            new (16, "DLE", "Data Link Escape"),
            new (17, "DC1", "Device Control 1"),
            new (18, "DC2", "Device Control 2"),
            new (19, "DC3", "Device Control 3"),
            new (20, "DC4", "Device Control 4"),
            new (21, "NAK", "Negative Acknowledge"),
            new (22, "SYN", "Synchronize"),
            new (23, "ETB", "End of Transmission Block"),
            new (24, "CAN", "Cancel"),
            new (25, "EM", "End of Medium"),
            new (26, "SUB", "Substitute"),
            new (27, "ESC", "Escape"),
            new (28, "FS", "File Separator"),
            new (29, "GS", "Group Separator"),
            new (30, "RS", "Record Separator"),
            new (31, "US", "Unit Separator"),
            new (32, "SPACE", "Space"),
            new (33, "!", "Exclamation Point"),
            new (34, "\"", "Double Quote"),
            new (35, "#", "Hash"),
            new (36, "$", "Dollar"),
            new (37, "%", "Percent"),
            new (38, "&", "Ampersand"),
            new (39, "'", "Single Quote"),
            new (40, "(", "Left Parenthesis"),
            new (41, ")", "Right Parenthesis"),
            new (42, "*", "Asterisk"),
            new (43, "+", "Plus"),
            new (44, ",", "Comma"),
            new (45, "-", "Minus"),
            new (46, ".", "Period"),
            new (47, "/", "Slash"),
        };

        result.AddRange(CreateAsciiDataFor('0', '9'));

        result.AddRange(new AsciiData[] {
            new (58, ":", "Colon"),
            new (59, ";", "Semicolon"),
            new (60, "<", "Less Than"),
            new (61, "=", "Equals"),
            new (62, ">", "Greater Than"),
            new (63, "?", "Question Mark"),
            new (64, "@", "At Sign"),
        });

        result.AddRange(CreateAsciiDataFor('A', 'Z'));

        result.AddRange(new AsciiData[] {
            new (91, "[", "Left Square Bracket"),
            new (92, "\\", "Backslash"),
            new (93, "]", "Right Square Bracket"),
            new (94, "^", "Circumflex"),
            new (95, "_", "Underscore"),
            new (96, "`", "Grave/Accent"),
        });

        result.AddRange(CreateAsciiDataFor('a', 'z'));

        result.Add(new (123, "{", "Left Curly Brace"));
        result.Add(new (124, "|", "Vertical Bar"));
        result.Add(new (125, "}", "Right Curly Brace"));
        result.Add(new (126, "~", "Tilde"));
        result.Add(new (127, "DEL", "Delete"));

        return result.ToArray();
    }

    private static IEnumerable<AsciiData> CreateAsciiDataFor(char start, char end)
    {
        for (char c = start; c <= end; c++)
        {
            yield return new (c, $"{c}", "");
        }
    }
}
