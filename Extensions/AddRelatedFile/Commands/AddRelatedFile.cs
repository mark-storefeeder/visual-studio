using System.ComponentModel.Design;
using EnvDTE;

namespace AddRelatedFile.Commands;

internal sealed class AddRelatedFile
{
    public static readonly Guid CommandSetGuid = PackageGuids.guidAddRelatedFileCmdSet;

    public static AddRelatedFile Instance { get; private set; }

    private IServiceProvider ServiceProvider { get; init; }

    private AddRelatedFile(Package package)
    {
        ServiceProvider = package ?? throw new ArgumentNullException(nameof(package));

        var commandService = ServiceProvider.GetService<IMenuCommandService, OleMenuCommandService>();

        if (commandService is null)
        {
            return;
        }

        var addCsFileCommandId = new CommandID(CommandSetGuid, PackageIds.AddRelatedCsFileCommandId);
        var addJsFileCommandId = new CommandID(CommandSetGuid, PackageIds.AddRelatedJsFileCommandId);
        var addResxFileCommandId = new CommandID(CommandSetGuid, PackageIds.AddRelatedResxFileCommandId);
        var addScssFileCommandId = new CommandID(CommandSetGuid, PackageIds.AddRelatedScssFileCommandId);

        // Defer the visibility logic back to the VisibilityConstraints in the .vsct file:
        var addCsFileCommand = new OleMenuCommand(AddRelatedCsFile, addCsFileCommandId) { Supported = false };
        var addJsFileCommand = new OleMenuCommand(AddRelatedJsFile, addJsFileCommandId) { Supported = false };
        var addResxFileCommand = new OleMenuCommand(AddRelatedResxFile, addResxFileCommandId) { Supported = false };
        var addScssFileCommand = new OleMenuCommand(AddRelatedScssFile, addScssFileCommandId) { Supported = false };

        commandService.AddCommand(addCsFileCommand);
        commandService.AddCommand(addJsFileCommand);
        commandService.AddCommand(addResxFileCommand);
        commandService.AddCommand(addScssFileCommand);
    }

    public static void Initialize(Package package) => Instance = new AddRelatedFile(package);

    private void AddRelatedCsFile(object sender, EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        AddFile("cs", @"Visual C#\Code\Class", callback: dte =>
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            (dte.ActiveDocument.Object() as TextDocument).ReplaceText("public class", "public partial class");
            dte.ActiveDocument.Save();
        });
    }

    private void AddRelatedJsFile(object sender, EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        AddFile("js");
    }

    private void AddRelatedResxFile(object sender, EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        AddFile("resx");
    }

    private void AddRelatedScssFile(object sender, EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        AddFile("scss");
    }

    private void AddFile(string extension, string itemTemplate = @"Visual C#\Code\Code File", Action<DTE> callback = null)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        var dte = ServiceProvider.GetService<DTE, DTE>();

        foreach (SelectedItem selectedItem in dte.SelectedItems)
        {
            if (selectedItem.ProjectItem is null)
            {
                break;
            }

            dte.ItemOperations.AddNewItem(itemTemplate, $"{selectedItem.ProjectItem.Name}.{extension}");

            if (callback is not null)
            {
                callback(dte);
            }
        }
    }
}
