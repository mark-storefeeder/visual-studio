global using System;
global using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace AddRelatedFile;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
[Guid(PackageGuidString)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[ProvideUIContextRule(UiContextSupportedFilesString,
    name: "Supported Files",
    expression: "Razor",
    termNames: new[] { "Razor" },
    termValues: new[] { "HierSingleSelectionName:.razor$" })]
public sealed class VSPackageCommandFileContextMenu : AsyncPackage
{
    public const string PackageGuidString = PackageGuids.guidAddRelatedFileString;

    public const string UiContextSupportedFilesString = PackageGuids.uiContextSupportedFilesString;

    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        await base.InitializeAsync(cancellationToken, progress);

        // When initialized asynchronously, we *may* be on a background thread at this point. Do any initialization that requires the UI thread after switching to the UI
        // thread. Otherwise, remove the switch to the UI thread if you don't need it.
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        Commands.AddRelatedFile.Initialize(this);
    }
}
