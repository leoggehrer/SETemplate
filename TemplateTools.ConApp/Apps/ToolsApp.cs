//@BaseCode
//MdStart
using TemplateTools.Logic;
using TemplateTools.Logic.Git;

namespace TemplateTools.ConApp.Apps
{
    /// <summary>
    /// Represents the main application class for the ToolsApp.
    /// </summary>
    public partial class ToolsApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="Program"/> class.
        /// This static constructor sets up the necessary properties for the program.
        /// </remarks>
        static ToolsApp()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called during the construction of the class.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// Represents a method that is called when a class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region Instance-Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        public ToolsApp()
        {
            Constructing();
            Constructed();
        }
        /// <summary>
        /// This method is called during the construction of the object.
        /// </summary>
        partial void Constructing();
        /// <summary>
        /// This method is called when the object is constructed.
        /// </summary>
        partial void Constructed();
        #endregion Instance-Constructors

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
                    Key = "----",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },

                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Force", "Change force flag"),
                    Action = (self) => ChangeForce(),
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Path", "Change solution path"),
                    Action = (self) => ChangeSolutionPath(),
                },

                new()
                {
                    Key = "----",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },

                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Copier", "Copy this solution to a domain solution"),
                    Action = (self) => new CopierApp().Run([]),
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Preprocessor", "Setting defines for project options"),
                    Action = (self) => new PreprocessorApp().Run([]),
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("CodeGenerator", "Generate code for this solution"),
                    Action = (self) => new CodeGeneratorApp().Run([]),
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Comparison", "Compares a project with the template"),
                    Action = (self) => new ComparisonApp().Run([]),
                },
                //new()
                //{
                //    Key = (++mnuIdx).ToString(),
                //    Text = ToLabelText("Documentation", "Generate documentation for this solution"),
                //    Action = (self) => new DocuGeneratorApp().Run([]),
                //},
                //new()
                //{
                //    Key = (++mnuIdx).ToString(),
                //    Text = ToLabelText("Formatting", "Formatting source code files"),
                //    Action = (self) => new FormatterApp().Run([]),
                //},
                //new()
                //{
                //    Key = (++mnuIdx).ToString(),
                //    Text = ToLabelText("Deleting", "Deleting generated files"),
                //    Action = (self) => DeleteGeneratedFiles(),
                //},
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Cleanup", "Deletes the temporary directories"),
                    Action = (self) => new CleanupApp().Run([]),
                },
                //new()
                //{
                //    Key = (++mnuIdx).ToString(),
                //    Text = ToLabelText("Html-Tools", "Useful html tools like formatter etc."),
                //    Action = (self) => new HtmlToolsApp().Run([]),
                //},
                //new()
                //{
                //    Key = (++mnuIdx).ToString(),
                //    Text = ToLabelText("ChatGPT", "Question to ChatGPT"),
                //    Action = (self) => new ChatGPTApp().Run([]),
                //},
            };
            return [.. menuItems.Union(CreateExitMenuItems())];
        }

        /// <summary>
        /// Prints the header for the PlantUML application.
        /// </summary>
        /// <param name="sourcePath">The path of the solution.</param>
        protected override void PrintHeader()
        {
            List<KeyValuePair<string, object>> headerParams = [new("Force flag:", Force), new("Solution path:", SolutionPath)];

            base.PrintHeader("Template Tools", [.. headerParams]);
        }
        #endregion overrides

        #region app methods
        /// <summary>
        /// Deletes all generated files from the solution path.
        /// </summary>
        internal void DeleteGeneratedFiles()
        {
            PrintHeader();
            StartProgressBar();
            Console.WriteLine("Delete all generated files...");
            Generator.DeleteGeneratedFiles(SolutionPath);
            Console.WriteLine("Delete all generated files ignored from git...");
            GitIgnoreManager.DeleteIgnoreEntries(SolutionPath);
        }
        #endregion app methods
    }
}
//MdEnd
