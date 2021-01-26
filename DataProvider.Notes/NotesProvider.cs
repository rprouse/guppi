using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
            notes.AddArgument(new Argument<string>("filename", () => DateTime.Now.ToString("yyyy-MM-dd")));
            notes.Handler = CommandHandler.Create((bool nocreate, bool configure, string filename) => OpenNotes(nocreate, configure, filename));
            return notes;
        }

        void OpenNotes(bool nocreate, bool configure, string filename)
        {
            if (!_configuration.Configured || configure)
                Configure();

            if (!Directory.Exists(_configuration.NotesDirectory))
                Directory.CreateDirectory(_configuration.NotesDirectory);

            string fullname = Path.Combine(_configuration.NotesDirectory, filename);
            var fi = new FileInfo(fullname);
            if (string.IsNullOrWhiteSpace(fi.Extension))
                fullname += ".md";

            if(!nocreate && !File.Exists(fullname))
            {
                File.WriteAllText(fullname, $"# {filename}\n\n");
            }

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            string cmd = isWindows ? 
                         Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\Microsoft VS Code\Code.exe") :
                         "/usr/bin/code";
            string args = nocreate ?
                $"\"{_configuration.NotesDirectory}\"" :
                $"-g \"{fullname}:3\" \"{_configuration.NotesDirectory}\"";

            var psi = new ProcessStartInfo
            {
                FileName = cmd,
                Arguments = args
            };
            System.Diagnostics.Process.Start(psi);
        }

        private void Configure()
        {
            _configuration.RunConfiguration("Notes", "Set the Notes Directory.");
        }
    }
}
