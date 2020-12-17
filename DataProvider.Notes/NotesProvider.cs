using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;
using Guppi.Core;

namespace DataProvider.Notes
{
    public class NotesProvider : IDataProvider
    {
        NotesConfiguration _configuration;

        public NotesProvider()
        {
            _configuration = Configuration.Load<NotesConfiguration>("notes");
        }

        public Command GetCommand()
        {
            var notes = new Command("notes", "Opens the notes directory and creates a new note for today if it doesn't exist.")
            {
                new Option<bool>(new string[]{"--nocreate", "-n"}, "Do not create today's note."),
                new Option<bool>(new string[]{"--configure", "-c"}, "Set the notes directory.")
            };
            notes.Handler = CommandHandler.Create((bool nocreate, bool configure) => OpenNotes(nocreate, configure));
            return notes;
        }

        void OpenNotes(bool nocreate, bool configure)
        {
            if (!_configuration.Configured || configure)
                Configure();

            if (!Directory.Exists(_configuration.NotesDirectory))
                Directory.CreateDirectory(_configuration.NotesDirectory);

            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string todayFile = Path.Combine(_configuration.NotesDirectory, $"{today}.md");
            if(!nocreate && !File.Exists(todayFile))
            {
                File.WriteAllText(todayFile, $"# {today}\n\n");
            }

            string cmd = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\Microsoft VS Code\Code.exe");
            string args = nocreate ?
                $"\"{_configuration.NotesDirectory}\"" :
                $"-g \"{todayFile}:3\" \"{_configuration.NotesDirectory}\"";

            var psi = new ProcessStartInfo
            {
                FileName = cmd,
                Arguments = args
            };
            System.Diagnostics.Process.Start(psi);
        }

        private void Configure()
        {
            _configuration.RunConfiguration("Notes", "[Set the Notes Directory.]");
        }
    }
}
