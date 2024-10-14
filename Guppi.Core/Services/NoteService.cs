using System;
using System.IO;
using System.Runtime.InteropServices;
using Guppi.Core.Configurations;
using Guppi.Core.Exceptions;
using Guppi.Core.Interfaces;

namespace Guppi.Core.Services;

internal sealed class NoteService(IProcessProvider process) : INoteService
{
    private readonly IProcessProvider _process = process;

    public void AddFile(string title, string vault)
    {
        var configuration = Configuration.Load<NotesConfiguration>("notes");
        if (!configuration.Configured)
            throw new UnconfiguredException("Please configure the Notes provider.");

        vault = string.IsNullOrWhiteSpace(vault) ? configuration.VaultName : vault;
        string uri = $"obsidian://new?vault={vault}&name={title}&append=true";

        _process.Open(uri);
    }

    public void Configure()
    {
        var configuration = Configuration.Load<NotesConfiguration>("notes");
        configuration.RunConfiguration("Notes", "Set the Notes Directory.");
    }

    public void OpenObsidian(string vault, string filename)
    {
        var configuration = Configuration.Load<NotesConfiguration>("notes");
        if (!configuration.Configured)
            throw new UnconfiguredException("Please configure the Notes provider.");

        vault = string.IsNullOrWhiteSpace(vault) ? configuration.VaultName : vault;
        string uri = GetUriHandler(vault, filename);

        _process.Open(uri);
    }

    internal static string GetUriHandler(string vault, string filename) =>
        string.IsNullOrWhiteSpace(filename) ?
            $"obsidian://open?vault={vault}" :
            $"obsidian://open?vault={vault}&file={filename}";

    public void OpenVsCode()
    {
        var configuration = Configuration.Load<NotesConfiguration>("notes");
        if (!configuration.Configured)
            throw new UnconfiguredException("Please configure the Notes provider.");

        if (!Directory.Exists(configuration.NotesDirectory))
            Directory.CreateDirectory(configuration.NotesDirectory);

        LaunchVSCode(configuration);
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
