using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DQXLauncher.Core.Game;

namespace DQXLauncher.Avalonia.Services;

public class Win32Launcher(Settings settings) : Launcher
{
    private readonly Settings _settings = settings;

    public override Task LaunchGame()
    {
        if (SessionId is null) throw new InvalidOperationException("SessionId is null");
        if (_settings.GameFolderPath is null) throw new InvalidOperationException("GameFolderPath is null");

        var gamePath = Path.Combine(_settings.GameFolderPath, "game", "DQXGame.exe");

        var process = new Process();
        process.StartInfo.WorkingDirectory = Path.Combine(_settings.GameFolderPath, "game");
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.FileName = gamePath;
        process.StartInfo.Arguments = GetArguments();
        process.Start();

        return Task.CompletedTask;
    }
}