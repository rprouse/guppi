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
    public record OpenNotesCommand(string Filename, bool NoCreate) : IRequest;

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

            string fullname = request.Filename is null ? configuration.NotesDirectory : Path.Combine(configuration.NotesDirectory, request.Filename);
            var fi = new FileInfo(fullname);
            if (string.IsNullOrWhiteSpace(fi.Extension))
                fullname += ".md";

            if (!request.NoCreate && !File.Exists(fullname))
            {
                File.WriteAllText(fullname, $"# {request.Filename}\n\n");
            }

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            string cmd = isWindows ?
                         Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\Microsoft VS Code\Code.exe") :
                         "/usr/bin/code";
            string args = request.NoCreate ?
                $"\"{configuration.NotesDirectory}\"" :
                $"-g \"{fullname}:3\" \"{configuration.NotesDirectory}\"";

            _process.Start(cmd, args);
            return await Unit.Task;
        }
    }
}
