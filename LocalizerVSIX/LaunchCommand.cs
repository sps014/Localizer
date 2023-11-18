using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using LocalizerVSIX.Utils;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace LocalizerVSIX
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class LaunchCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("f9fc65dd-a67f-4668-b030-bebd455880ba");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="LaunchCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private LaunchCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static LaunchCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in LaunchCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new LaunchCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var proj = GetSelectedSolutionExplorerItem()?.Object as EnvDTE.Project;
                var folder = Path.GetDirectoryName(proj.FileName);

                var path = Environment.GetEnvironmentVariable("Localizer", EnvironmentVariableTarget.User);

                if (!File.Exists(path))
                {
                    VsShellUtilities.ShowMessageBox(
                        this.package,
                        "Path of localizer is not found please run Localizer exe from its path for 1st time",
                        "Error",
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                    return;

                }
                var projectName = $"\"{Path.GetDirectoryName(proj.FullName)}\"";
                
                FluentProcess.Create(path)
                    .WithArguments(projectName)
                    .WithWorkingDirectory(folder)
                    .Run();
            }
            catch (Exception ex)
            {
                VsShellUtilities.ShowMessageBox(
               this.package,
               ex.Message,
               "Error",
               OLEMSGICON.OLEMSGICON_CRITICAL,
               OLEMSGBUTTON.OLEMSGBUTTON_OK,
               OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
            
        }
        private EnvDTE.UIHierarchyItem GetSelectedSolutionExplorerItem()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte = ServiceProvider.GetServiceAsync(typeof(DTE)).Result as DTE2;
            if (dte == null) return null;
            var solutionExplorer = dte.ToolWindows.SolutionExplorer;
            object[] items = solutionExplorer.SelectedItems as object[];
            if (items.Length != 1)
                return null;

            return items[0] as UIHierarchyItem;
        }
    }
}
