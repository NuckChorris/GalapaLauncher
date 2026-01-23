using System;
using System.IO;
using Galapa.Core.StreamObfuscator;
using Galapa.Toolbox.Controls;

namespace Galapa.Toolbox.Tools;

/// <summary>
///     Exports files deobfuscated with the FixedObfuscator (XOR 0xA7).
/// </summary>
public class ExportFixedObfuscatorTool : ExportToolBase
{
    public override string MenuLabel => "Export with FixedObfuscator";

    protected override Func<Stream, Stream> GetObfuscator(FolderExplorerItem item)
    {
        return FixedObfuscator.Factory;
    }
}