using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using DQXLauncher.Core.Services;

namespace DQXLauncher.Core.Game;

public class Launcher
{
    private static readonly char[] SqEx = "SqEx".ToCharArray();
    private readonly Settings _settings;

    public Launcher(Settings settings)
    {
        this._settings = settings;
    }

    public string? SessionId { get; set; }

    public int? PlayerNumber { get; set; }

    // I don't know what the fuck this means and it's not in DQXGame.exe at all
    // But I'm scared to remove it
    public bool UseApartmentThreaded { get; set; } = true;

    public Task LaunchGame()
    {
        if (this.SessionId is null) throw new InvalidOperationException("SessionId is null");
        if (this._settings.GameFolderPath is null) throw new InvalidOperationException("GameFolderPath is null");

        var gamePath = Path.Combine(this._settings.GameFolderPath, "game", "DQXGame.exe");

        var process = new Process();
        process.StartInfo.WorkingDirectory = Path.Combine(this._settings.GameFolderPath, "game");
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.FileName = gamePath;
        process.StartInfo.Arguments = this.GetArguments();
        process.Start();

        return Task.CompletedTask;
    }

    private string GetArguments()
    {
        var args = new StringBuilder();

        args.Append($"-StartupToken={GetStartupToken()} ");
        if (this.SessionId is not null) args.Append($"-SessionID={this.EncodeSessionId(this.SessionId)} ");
        if (this.PlayerNumber is not null) args.Append($"-PlayerNumber={this.PlayerNumber} ");
        args.Append("-USE_APARTMENTTHREADED");

        return args.ToString();
    }

    private string EncodeSessionId(string sid)
    {
        if (!this.IsValidHex(sid)) throw new ArgumentException("Input must be a 56-character hex string.");

        var timeStr = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 60).ToString();

        var input = $"DQUEST10{sid}";
        var md5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes($"{timeStr}DraqonQuestX"));

        var output = new byte[64];
        // Encoding loop
        for (var i = 0; i < 64; i++)
        {
            int ecx = md5[i % 16];
            var eax = i < input.Length ? input[i] : 0;
            ecx -= 48;
            eax += ecx;
            eax %= 78;
            eax += 48;
            output[i] = (byte)eax;
        }

        return Encoding.UTF8.GetString(output, 0, 64);
    }

    [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
    private static extern uint GetTime();

    private static string GetStartupToken()
    {
        // The official version of this function actually uses an MT RNG seeded from the Windows "true" RNG to generate
        // these 4 chars, but because that makes it *actually random*, they can't check it and we just stuff 0000 in.
        var baseString = "0000" + (GetTime() >>> 1);

        return new string(baseString
            .ToCharArray()
            .Select((person, index) => (char)(person ^ SqEx[index & 3]))
            .ToArray());
    }

    private bool IsValidHex(string str)
    {
        return Regex.IsMatch(str, "^[0-9a-fA-F]{56}$");
    }
}