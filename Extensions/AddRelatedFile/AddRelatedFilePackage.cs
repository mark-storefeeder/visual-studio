global using System;
global using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using AddRelatedFile.Commands;

namespace AddRelatedFile;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
[Guid(PackageGuidString)]
[ProvideMenuResource("Menus.ctmenu", 1)]
public sealed class VSPackageCommandFileContextMenu : AsyncPackage
{
    public const string PackageGuidString = PackageGuids.guidVSPackageCommandFileContextMenuString;

    protected override async System.Threading.Tasks.Task InitializeAsync(System.Threading.CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        await base.InitializeAsync(cancellationToken, progress);

        // When initialized asynchronously, we *may* be on a background thread at this point. Do any initialization that requires the UI thread after switching to the UI
        // thread. Otherwise, remove the switch to the UI thread if you don't need it.
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        AddRelatedCsFile.Initialize(this);
    }
}

// TODO:
// 1. Move everything to submenu.
// 2. Create commands for .scss, .resx and .js
// 3. Only show the menu item if you've right-clicked on a .razor file.
// 4. Add content to files where possible - e.g. .cs files should have an empty class in them already.
// 5. Show a single error for all files which already exist rather than one error per file - i.e. outside the loop.
// 6. Open files in the editor after they're created.
