using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Text;
using Guppi.Application.Extensions;
using Spectre.Console;

namespace Guppi.Console.Skills
{
    internal class RC2014Skill : ISkill
    {
        public IEnumerable<Command> GetCommands()
        {
            var convert = new Command("convert", "Converts a file to a format that can be loaded into the RC2014 using DOWNLOAD.COM")
            {
                new Option<byte>(new [] { "-u", "--user" }, () => 0, "The user number to use. Defaults to 0"),
                new Argument<string>("file", "The file to convert")
            };
            convert.AddAlias("conv");
            convert.Handler = CommandHandler.Create<byte, string>(Convert);

            return new[] 
            { 
                new Command("rc2014", "Interacts with my RC2014 computer")
                {
                    convert
                }
            };
        }

        private void Convert(byte user, string file)
        {
            if (user > 15)
            {
                AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: Sorry, user number must be between 0 and 15.]][/]");
                return;
            }

            if (!File.Exists(file))
            {
                AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: File {file} does not exist]][/]");
                return;
            }
            var bytes = File.ReadAllBytes(file);

            if (bytes.Length > 131072)
            {
                AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: Sorry, your file is too large, Limit 131072 bytes.]][/]");
                return;
            }

            AnsiConsoleHelper.TitleRule(":satellite_antenna: Satellite scans complete. Ready to upload...");

            byte check = 0;
            byte count = 0;

            var sb = new StringBuilder();
            sb.AppendLine($"A:DOWNLOAD {Path.GetFileName(file).ToUpperInvariant()}");
            sb.AppendLine($"U{user}");
            sb.Append(":");

            foreach (var b in bytes)
            {
                sb.Append(b.ToString("X2"));
                check += b;
                count++;
            }
            byte pad = (byte)(128 - bytes.Length % 128);
            sb.Append('0', pad * 2);
            count += pad;

            sb.AppendLine($">{count:X2}{check:X2}");

            TextCopy.ClipboardService.SetText( sb.ToString() );
            AnsiConsole.Write(sb.ToString());
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green]:green_circle: Copied to clipboard[/]");
            AnsiConsoleHelper.Rule("white");
        }
    }
}
