using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Guppi.Application;
using Guppi.Domain.Interfaces;

namespace Guppi.Infrastructure.Services.Notes
{
    internal class NotesService : INotesService
    {
        NotesConfiguration _configuration;

        public NotesService()
        {
            _configuration = Configuration.Load<NotesConfiguration>("notes");
        }

        public void OpenNotes(bool nocreate, string filename)
        {
            if (!_configuration.Configured)
                Configure();

            if (!Directory.Exists(_configuration.NotesDirectory))
                Directory.CreateDirectory(_configuration.NotesDirectory);

            string fullname = filename is null ? _configuration.NotesDirectory : Path.Combine(_configuration.NotesDirectory, filename);
            var fi = new FileInfo(fullname);
            if (string.IsNullOrWhiteSpace(fi.Extension))
                fullname += ".md";

            if (!nocreate && !File.Exists(fullname))
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

        public void Configure()
        {
            _configuration.RunConfiguration("Notes", "Set the Notes Directory.");
        }
    }
}
