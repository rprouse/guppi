using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using ColoredConsole;
using Guppi.Core;

namespace DataProvider.Utilities
{
    public class UtilitiesProvider : IMultipleDataProvider
    {
        public IEnumerable<Command> GetCommands()
        {
            var time = new Command("time", "Displays the current date/time")
            {
                new Option<bool>(new string[]{"--utc", "-u"}, "Displays the time as UTC")
            };
            time.Handler = CommandHandler.Create((bool utc) => DisplayTime(utc));
            yield return time;

            var date = new Command("date", "Displays the current date")
            {
                new Option<bool>(new string[]{"--utc", "-u"}, "Displays the date as UTC")
            };
            date.Handler = CommandHandler.Create((bool utc) => DisplayDate(utc));
            yield return date;

            var guid = new Command("guid", "Creates a new Guid");
            guid.Handler = CommandHandler.Create(() => NewGuid());
            yield return guid;
        }

        void DisplayTime(bool utc)
        {
            var now = utc ? DateTime.UtcNow : DateTime.Now;
            ColorConsole.WriteLine(now.ToString("[yyyy-MM-dd.HH:mm:ss.fff]").Yellow());
        }

        void DisplayDate(bool utc)
        {
            var now = utc ? DateTime.UtcNow : DateTime.Now;
            ColorConsole.WriteLine(now.ToString("[yyyy-MM-dd]").Yellow());
        }

        void NewGuid()
        {
            ColorConsole.WriteLine($"[{Guid.NewGuid().ToString("f")}]".Yellow());
        }
    }
}
