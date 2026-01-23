using System;
using System.IO;
using Galapa.Core.StreamObfuscator;
using Galapa.Toolbox.Controls;

namespace Galapa.Toolbox.Tools;

/// <summary>
///     Exports files deobfuscated with the UsernameObfuscator.
/// </summary>
public class ExportUsernameObfuscatorTool : ExportToolBase
{
    public override string MenuLabel => "Export with UsernameObfuscator";

    protected override Func<Stream, Stream> GetObfuscator(FolderExplorerItem item)
    {
        return UsernameObfuscator.Factory;
    }
}