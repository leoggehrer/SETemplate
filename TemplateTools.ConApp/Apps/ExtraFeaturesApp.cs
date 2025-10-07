//@BaseCode

using TemplateTools.Logic;
using TemplateTools.Logic.Git;

namespace TemplateTools.ConApp.Apps
{
    /// <summary>
    /// Represents an application for copying template solutions to a target solution.
    /// </summary>
    public partial class ExtraFeaturesApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// This is the static constructor for the CopierApp class.
        /// </summary>
        /// <remarks>
        /// This constructor is responsible for initializing the static members of the CopierApp class.
        /// </remarks>
        static ExtraFeaturesApp()
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
        /// Gets or sets the application arguments that are passed to sub-applications.
        /// </summary>
        private string[] AppArgs { get; set; } = [];
        /// <summary>
        /// Gets or sets the path of the source solution.
        /// </summary>
        private string SourceSolutionPath { get; set; } = SolutionPath;
        #endregion Properties

        #region overrides
        /// <summary>
        /// Creates an array of menu items for the application menu.
        /// </summary>
        /// <returns>An array of MenuItem objects representing the menu items.</returns>
        protected override MenuItem[] CreateMenuItems()
        {
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
                    Action = (self) => SourceSolutionPath = ChangeTemplateSolutionPath(SourceSolutionPath, MaxSubPathDepth, ReposPath),
                },

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
                    OptionalKey = "chatgpt_entities",
                    Text = ToLabelText("ChatGpt entities", "Generate the entities with ChatGpt"),
                    Action = (self) => new ChatGptEntityCreatorApp().Run(AppArgs),
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "chatgpt_import",
                    Text = ToLabelText("ChatGpt import", "Generates a date import for all entities"),
                    Action = (self) => new ChatGptDataImporterApp().Run(AppArgs),
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "chatgpt_forms",
                    Text = ToLabelText("ChatGpt forms", "Generates html forms for all entities"),
                    Action = (self) => new ChatGptHtmlFormCreatorApp().Run(AppArgs),
                },

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
                    OptionalKey = "code_generator",
                    Text = ToLabelText("CodeGenerator", "Open the menu for code generation"),
                    Action = (self) =>
                    {
                        var appArgs = $"CodeSolutionPath={SourceSolutionPath}";

                        new CodeGeneratorApp().Run([ appArgs ]);
                    },
                },

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
                    OptionalKey = "code_exporter",
                    Text = ToLabelText("CodeExporter", "Open the menu for the code exporter"),
                    Action = (self) => new CodeExporterApp().Run(AppArgs),
                },

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
                    OptionalKey = "restserver",
                    Text = ToLabelText("Tools Server", "Open the menu for the Tools server options"),
                    Action = (self) =>
                    {
                        var appArgs = $"CodeSolutionPath={SourceSolutionPath}";

                        new ToolsRestServerApp().Run([ appArgs ]);
                    },
                },

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
                    OptionalKey = "delete_gpt_files",
                    Text = ToLabelText("Delete gpt-files", "Delete all files created by chatgpt"),
                    Action = (self) => StartDeleteChatGptFiles(),
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    OptionalKey = "del_gen_files",
                    Text =  ToLabelText("Delete gen-files", "Delete all files generated by the code generator"),
                    Action = (self) => DeleteGeneratedFiles(),
                },

            };

            return [.. menuItems.Union(CreateExitMenuItems())];
        }
        /// <summary>
        /// Prints the header for the application.
        /// </summary>
        /// <param name="sourcePath">The path of the solution.</param>
        protected override void PrintHeader()
        {
            List<KeyValuePair<string, object>> headerParams = [new("Solution path:", SolutionPath)];

            base.PrintHeader("Template Extra Features", [.. headerParams]);
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
                if (arg.Key.Equals(nameof(SourceSolutionPath), StringComparison.OrdinalIgnoreCase))
                {
                    SourceSolutionPath = arg.Value;
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
            AppArgs = [.. appArgs];
            base.BeforeRun([.. appArgs]);
        }
        #endregion overrides

        #region app methods
        /// <summary>
        /// Deletes all generated files and directories from the solution path.
        /// </summary>
        private void DeleteGeneratedFiles()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Delete all generated files...");
            Generator.DeleteGeneratedFiles(SourceSolutionPath);
            PrintLine("Delete all empty folders...");
            Generator.CleanDirectories(SourceSolutionPath);
            PrintLine("Delete all generated files ignored from git...");
            GitIgnoreManager.DeleteIgnoreEntries(SourceSolutionPath);
            StopProgressBar();
        }
        /// <summary>
        /// Starts the process of importing program code from the specified import file.
        /// Prints the application header, starts a progress bar, logs the import action,
        /// performs the import operation, and restarts the progress bar.
        /// </summary>
        public void StartDeleteChatGptFiles()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Delete chatgpt files...");
            DeleteChatGptFiles(SourceSolutionPath);
            StartProgressBar();
        }
        #endregion app methods
    }
}

