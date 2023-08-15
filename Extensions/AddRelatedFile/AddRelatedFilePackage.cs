global using System;
global using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace AddRelatedFile;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
[Guid(PackageGuidString)]
[ProvideMenuResource("Menus.ctmenu", 1)]
public sealed class VSPackageCommandFileContextMenu : AsyncPackage
{
    public const string PackageGuidString = PackageGuids.guidAddRelatedFileString;

    protected override async System.Threading.Tasks.Task InitializeAsync(System.Threading.CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        await base.InitializeAsync(cancellationToken, progress);

        // When initialized asynchronously, we *may* be on a background thread at this point. Do any initialization that requires the UI thread after switching to the UI
        // thread. Otherwise, remove the switch to the UI thread if you don't need it.
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        Commands.AddRelatedFile.Initialize(this);
    }
}

// TODO:
// 1. Only show the menu item if you've right-clicked on a .razor file.
