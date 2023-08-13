using System.ComponentModel.Design;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;

namespace AddRelatedFile.Commands;

internal sealed class AddRelatedCsFile
{
    public const int CommandId = PackageIds.AddRelatedCsFileCommandId;

    public static readonly Guid CommandSet = PackageGuids.guidVSPackageCommandFileContextMenuCmdSet;

    private readonly Package _package;

    public static AddRelatedCsFile Instance { get; private set; }

    private IServiceProvider ServiceProvider => _package;

    private AddRelatedCsFile(Package package)
    {
        _package = package ?? throw new ArgumentNullException(nameof(package));

        var commandService = ServiceProvider.GetService<IMenuCommandService, OleMenuCommandService>();

        if (commandService is null)
        {
            return;
        }

        commandService.AddCommand(new MenuCommand(MenuItemCallback, new CommandID(CommandSet, CommandId)));
    }

    public static void Initialize(Package package)
    {
        Instance = new AddRelatedCsFile(package);
    }

    private void MenuItemCallback(object sender, EventArgs e)
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
            var newFilePath = $"{filePath}.cs";

            if (File.Exists(newFilePath))
            {
                VsShellUtilities.ShowMessageBox(ServiceProvider, $"{newFilePath} already exists so has not been created.", "VSIXProject1",
                    OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                return;
            }

            File.Create(newFilePath).Close();
        }
    }
}
