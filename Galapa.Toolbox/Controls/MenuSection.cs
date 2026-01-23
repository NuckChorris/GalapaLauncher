using System.Collections.Generic;

namespace Galapa.Toolbox.Controls;

/// <summary>
///     Defines a section of context menu tools. Sections are separated by menu separators.
/// </summary>
public class MenuSection
{
    /// <summary>
    ///     Optional header text for this section (not currently rendered, but available for future use).
    /// </summary>
    public string? Header { get; init; }

    /// <summary>
    ///     The tools in this section.
    /// </summary>
    public required IEnumerable<IContextMenuTool> Tools { get; init; }
}