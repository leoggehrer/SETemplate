//@BaseCode

using TemplateTools.Logic;

namespace TemplateTools.ConApp.Apps
{
    /// <summary>
    /// Represents an application for copying template solutions to a target solution.
    /// </summary>
    public partial class CopierApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// This is the static constructor for the CopierApp class.
        /// </summary>
        /// <remarks>
        /// This constructor is responsible for initializing the static members of the CopierApp class.
        /// </remarks>
        static CopierApp()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called when the class is being constructed.
        /// </summary>
        /// <remarks>
        /// This is a partial method and must be implemented in a partial class.
        /// </remarks>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the path of the source solution.
        /// </summary>
        private string SourceSolutionPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the target solution path.
        /// </summary>
        private string TargetSolutionSubPath { get; set; } = Directory.GetParent(SolutionPath)?.FullName ?? string.Empty;
        /// <summary>
        /// Gets or sets the name of the target solution.
        /// </summary>
        private string TargetSolutionName { get; set; } = "TargetSolution";
        #endregion Properties

        #region overrides
        /// <summary>
        /// Prints the header for the application.
        /// </summary>
        /// <param name="sourcePath">The path of the solution.</param>
        protected override void PrintHeader()
        {
            var solutionProperties = SolutionProperties.Create(SourceSolutionPath);
            var sourceSolutionName = solutionProperties.SolutionName;
            var sourceLabel = $"'{sourceSolutionName}' from:";
            var targetLabel = $"'{TargetSolutionName}' to:";

            List<KeyValuePair<string, object>> headerParams =
            [
                new(sourceLabel, SourceSolutionPath),
                new("  -> copy ->  ", string.Empty),
                new(targetLabel, Path.Combine(TargetSolutionSubPath, TargetSolutionName)),
            ];

            base.PrintHeader("Template Copier", [.. headerParams]);
        }
        /// <summary>
        /// Creates an array of menu items for the application menu.
        /// </summary>
        /// <returns>An array of MenuItem objects representing the menu items.</returns>
        protected override MenuItem[] CreateMenuItems()
        {
            var targetSolutionPath = Path.Combine(TargetSolutionSubPath, TargetSolutionName);
            var targetSolutionPathExists = Directory.Exists(targetSolutionPath);
            var mnuIdx = 0;
            var menuItems = new List<MenuItem>
            {
                new()
                {
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "sourcepath",
                    Text = ToLabelText("Source path", "Change the source solution path"),
                    Action = (self) =>
                    {
                        var previousPath = SourceSolutionPath;
                        var pathChangerApp = new PathChangerApp(
                             "Template Copier",
                             "Source solution path",
                             () => SourceSolutionPath,
                             (value) => SourceSolutionPath = value,
                             (path) => Directory.GetFiles(path, "*.sln").Length > 0);

                        pathChangerApp.Run([]);

                        if (string.IsNullOrEmpty(SourceSolutionPath) || Directory.GetFiles(SourceSolutionPath, "*.sln").Length == 0)
                        {
                            PrintLine();
                            PrintErrorLine("The selected solution path is invalid or does not exist.");
                            SourceSolutionPath = previousPath;
                            Thread.Sleep(3000);
                        }
                    },
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "targetpath",
                    Text = ToLabelText("Target path", "Change the target solution path"),
                    Action = (self) =>
                    {
                        var previousPath = TargetSolutionSubPath;
                        var pathChangerApp = new PathChangerApp(
                            "Template Copier",
                            "Target solution path",
                            () => TargetSolutionSubPath,
                            (value) => TargetSolutionSubPath = value);

                        pathChangerApp.Run([]);

                        if (string.IsNullOrEmpty(TargetSolutionSubPath))
                        {
                            PrintLine();
                            PrintErrorLine("The selected solution path is invalid or does not exist.");
                            TargetSolutionSubPath = previousPath;
                            Thread.Sleep(3000);
                        }
                    },
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "targetname",
                    Text = ToLabelText("Target name", "Change the target solution name"),
                    Action = (self) => ChangeTargetSolutionName(),
                },
                new()
                {
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
            };

            if (targetSolutionPathExists == false || (targetSolutionPathExists && Force))
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "start",
                    Text = ToLabelText("Start", "Start copy process"),
                    Action = (self) => CopySolution(),
                });
            }
            else
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "start",
                    Text = ToLabelText("Start", "Cannot be started because the path already exists."),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.Red,
                });
            }

            return [.. menuItems.Union(CreateExitMenuItems())];
        }
        /// <summary>
        /// Performs any necessary setup or initialization before running the application.
        /// </summary>
        /// <param name="args">The command-line arguments passed to the application.</param>
        protected override void BeforeRun(string[] args)
        {
            var convertedArgs = ConvertArgs(args);
            var appArgs = new List<string>();

            foreach (var arg in convertedArgs)
            {
                if (arg.Key.Equals(nameof(Force), StringComparison.OrdinalIgnoreCase))
                {
                    if (bool.TryParse(arg.Value, out bool result))
                    {
                        Force = result;
                    }
                }
                if (arg.Key.Equals(nameof(TargetSolutionName), StringComparison.OrdinalIgnoreCase))
                {
                    TargetSolutionName = arg.Value;
                }
                else if (arg.Key.Equals(nameof(SourceSolutionPath), StringComparison.OrdinalIgnoreCase))
                {
                    SourceSolutionPath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(TargetSolutionSubPath), StringComparison.OrdinalIgnoreCase))
                {
                    TargetSolutionSubPath = arg.Value;
                }
                else if (arg.Key.Equals("AppArg", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var item in arg.Value.ToLower().Split(','))
                    {
                        CommandQueue.Enqueue(item);
                    }
                }
                else
                {
                    appArgs.Add($"{arg.Key}={arg.Value}");
                }
            }
            base.BeforeRun([.. appArgs]);
        }
        #endregion overrides

        #region app methods
        /// <summary>
        /// Changes the target solution name based on user input.
        /// </summary>
        private void ChangeTargetSolutionName()
        {
            PrintLine();
            Print("Enter target solution name: ");
            var name = ReadLine();

            if (string.IsNullOrEmpty(name) == false)
            {
                TargetSolutionName = name;
            }
        }
        /// <summary>
        /// Copies the source solution to the target solution path, including all template projects.
        /// </summary>
        private void CopySolution()
        {
            var copier = new Modules.Copier();
            var targetSolutionPath = Path.Combine(TargetSolutionSubPath, TargetSolutionName);
            var solutionProperties = SolutionProperties.Create(SourceSolutionPath);
            var allSourceProjectNames = solutionProperties.AllTemplateProjectNames;

            PrintHeader();
            StartProgressBar();
            PrintLine($"Copying '{solutionProperties.SolutionName}' to '{TargetSolutionName}'...");
            copier.Copy(SourceSolutionPath, targetSolutionPath, allSourceProjectNames);
            StopProgressBar();

            TemplatePath.OpenSolutionFolder(targetSolutionPath);
        }
        #endregion app methods
    }
}

