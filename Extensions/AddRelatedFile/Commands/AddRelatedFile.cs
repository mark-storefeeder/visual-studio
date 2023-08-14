using System.ComponentModel.Design;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;

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

        commandService.AddCommand(new MenuCommand(AddRelatedCsFile, new CommandID(CommandSetGuid, PackageIds.AddRelatedCsFileCommandId)));
        commandService.AddCommand(new MenuCommand(AddRelatedJsFile, new CommandID(CommandSetGuid, PackageIds.AddRelatedJsFileCommandId)));
        commandService.AddCommand(new MenuCommand(AddRelatedResxFile, new CommandID(CommandSetGuid, PackageIds.AddRelatedResxFileCommandId)));
        commandService.AddCommand(new MenuCommand(AddRelatedScssFile, new CommandID(CommandSetGuid, PackageIds.AddRelatedScssFileCommandId)));
    }

    public static void Initialize(Package package) => Instance = new AddRelatedFile(package);

    private void AddRelatedCsFile(object sender, EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        AddFile("cs");
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

    private void AddFile(string extension)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        var dte = ServiceProvider.GetService<EnvDTE.DTE, EnvDTE.DTE>();

        if (dte.SelectedItems is null)
        {
            return;
        }

        foreach (EnvDTE.SelectedItem selectedItem in dte.SelectedItems)
        {
            if (selectedItem.ProjectItem is null)
            {
                break;
            }

            var filePath = selectedItem.ProjectItem.Properties.Item("FullPath").Value.ToString();
            var newFilePath = $"{filePath}.{extension}";

            if (File.Exists(newFilePath))
            {
                VsShellUtilities.ShowMessageBox(ServiceProvider, $"{newFilePath} already exists so has not been created.", "Add Related File",
                    OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                return;
            }

            File.Create(newFilePath).Close();
        }
    }
}
