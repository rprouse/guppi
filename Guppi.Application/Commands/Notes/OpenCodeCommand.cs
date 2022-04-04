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
    public record OpenCodeCommand() : IRequest;

    internal sealed class OpenCodeCommandHandler : IRequestHandler<OpenCodeCommand>
    {
        private readonly IProcessService _process;

        public OpenCodeCommandHandler(IProcessService process)
        {
            _process = process;
        }

        public async Task<Unit> Handle(OpenCodeCommand request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<NotesConfiguration>("notes");
            if (!configuration.Configured)
                throw new UnconfiguredException("Please configure the Notes provider.");

            if (!Directory.Exists(configuration.NotesDirectory))
                Directory.CreateDirectory(configuration.NotesDirectory);

            LaunchVSCode(configuration);

            return await Unit.Task;
        }

        private void LaunchVSCode(NotesConfiguration configuration)
        {
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            string cmd = isWindows
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Programs\Microsoft VS Code\bin\Code.cmd")
                : "/usr/bin/code";

            string args = $"\"{configuration.NotesDirectory}\"";

            _process.Start(cmd, args);
        }
    }
}
