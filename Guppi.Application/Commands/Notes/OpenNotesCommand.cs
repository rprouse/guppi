using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Application.Exceptions;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Notes
{
    public record OpenNotesCommand(string Title, string Filename, bool NoCreate) : IRequest;

    internal sealed class OpenNotesCommandHandler : IRequestHandler<OpenNotesCommand>
    {
        private readonly IProcessService _process;

        public OpenNotesCommandHandler(IProcessService process)
        {
            _process = process;
        }

        public async Task<Unit> Handle(OpenNotesCommand request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<NotesConfiguration>("notes");
            if (!configuration.Configured)
                throw new UnconfiguredException("Please configure the Notes provider.");

            if (!Directory.Exists(configuration.NotesDirectory))
                Directory.CreateDirectory(configuration.NotesDirectory);

            var fullname = GetFilename(request.Filename, request.Title, configuration.NotesDirectory);
            CreateMarkdownFile(request, fullname);
            LaunchVSCode(request, configuration, fullname);
            
            return await Unit.Task;
        }

        private void LaunchVSCode(OpenNotesCommand request, NotesConfiguration configuration, string fullname)
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            string cmd = isWindows
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Programs\Microsoft VS Code\bin\Code.cmd")
                : "/usr/bin/code";
            string args = request.NoCreate
                ? $"\"{configuration.NotesDirectory}\""
                : $"-g \"{fullname}:3\" \"{configuration.NotesDirectory}\"";

            _process.Start(cmd, args);
        }

        private static void CreateMarkdownFile(OpenNotesCommand request, string fullname)
        {
            if (!request.NoCreate && !File.Exists(fullname))
            {
                string content = $"# {request.Filename}\n\n";
                if (!string.IsNullOrWhiteSpace(request.Title))
                    content += $"## {request.Title}\n\n";

                File.WriteAllText(fullname, content);
            }
        }

        internal static string GetFilename(string filename, string title, string notesDirectory)
        {
            string fullname = filename is null
                ? notesDirectory
                : Path.Combine(notesDirectory, filename);
            var fi = new FileInfo(fullname);

            if (!string.IsNullOrWhiteSpace(title))
            {
                fullname = Path.Combine(notesDirectory,
                    $"{Path.GetFileNameWithoutExtension(fi.Name)} - {title}{fi.Extension}");
                fi = new FileInfo(fullname);
            }
            
            if (string.IsNullOrWhiteSpace(fi.Extension))
            {
                fullname += ".md";
            }

            return fullname;
        }
    }
}
