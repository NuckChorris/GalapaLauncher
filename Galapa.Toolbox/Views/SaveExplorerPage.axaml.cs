using Avalonia.Controls;
using Galapa.Toolbox.Controls;
using Galapa.Toolbox.Services;
using Galapa.Toolbox.Tools;

namespace Galapa.Toolbox.Views;

public partial class SaveExplorerPage : UserControl
{
    public SaveExplorerPage()
    {
        this.InitializeComponent();

        // Create tools
        var annotationTool = new KnownFileAnnotationTool();
        var knownFileTool = new ExportKnownFileTool();
        var fixedTool = new ExportFixedObfuscatorTool();
        var usernameTool = new ExportUsernameObfuscatorTool();
        var explorerTool = new OpenInExplorerTool();

        // Configure the FolderExplorer
        this.Explorer.RootPath = Settings.Instance.SaveFolderPath;

        // Register all tools (including annotation tools)
        this.Explorer.Tools = new IFileTool[]
        {
            annotationTool,
            knownFileTool,
            fixedTool,
            usernameTool,
            explorerTool
        };

        // Define menu layout (separators between sections)
        this.Explorer.MenuSections = new[]
        {
            new MenuSection { Tools = [knownFileTool] },
            new MenuSection { Tools = [fixedTool, usernameTool] },
            new MenuSection { Tools = [explorerTool] }
        };
    }
}