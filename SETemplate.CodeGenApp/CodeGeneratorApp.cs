//@BaseCode
using TemplateTools.Logic;
using TemplateTools.Logic.Contracts;
using TemplateTools.Logic.Git;

namespace SETemplate.CodeGenApp
{
    public partial class CodeGeneratorApp : CommonTool.ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="ConsoleApplication"/> class.
        /// </summary>
        /// <remarks>
        /// This static constructor sets up the necessary properties for the program.
        /// </remarks>
        static CodeGeneratorApp()
        {
            ClassConstructing();
            ToGroupFile = false;
            IncludeCleanDirectory = true;
            ExcludeGeneratedFilesFromGIT = true;
            SourcePath = SolutionPath = TemplatePath.GetSolutionPathByExecution();
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
        /// Initializes a new instance of the <see cref="ConsoleApplication"/> class.
        /// </summary>
        public CodeGeneratorApp()
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

        #region properties
        /// <summary>
        /// Gets or sets a value indicating whether the file should be grouped.
        /// </summary>
        private static bool ToGroupFile { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the empty folders in the source path will be deleted.
        /// </summary>
        private static bool IncludeCleanDirectory { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether generated files are excluded from GIT.
        /// </summary>
        private static bool ExcludeGeneratedFilesFromGIT { get; set; }
        #endregion properties

        #region overrides
        /// <summary>
        /// Prints the header for the PlantUML application.
        /// </summary>
        protected override void PrintHeader()
        {
            var saveForeColor = ForegroundColor;

            ForegroundColor = ConsoleColor.Green;
            Clear();
            int count = PrintLine($"Code generation for: {nameof(SETemplate)}");
            PrintLine('=', count);
            PrintLine();
            ForegroundColor = saveForeColor;
            PrintLine($"Solution path:            {SourcePath}");
            PrintLine($"Code generation for:      {GetSolutionName(SourcePath)}");
            PrintLine('-', 80);
            PrintLine($"Write generated code into:        {(ToGroupFile ? "Group files" : "Single files")}");
            PrintLine($"Delete empty folders in the path: {(IncludeCleanDirectory ? "Yes" : "No")}");
            PrintLine($"Exclude generated files from git: {(ExcludeGeneratedFilesFromGIT ? "Yes" : "No")}");
            PrintLine();
        }

        /// <summary>
        /// Creates an array of menu items for the code generator application.
        /// </summary>
        /// <returns>An array of <see cref="MenuItem"/> objects.</returns>
        protected override MenuItem[] CreateMenuItems()
        {
            var mnuIdx = 0;
            var menuItems = new MenuItem[]
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
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Group file", "Change group file flag"),
                    Action = (self) => ToGroupFile = !ToGroupFile
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Delete folders", "Change delete empty folders flag"),
                    Action = (self) => IncludeCleanDirectory = !IncludeCleanDirectory
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Exclude files", "Change exclude generated files from GIT"),
                    Action = (self) => ExcludeGeneratedFilesFromGIT = !ExcludeGeneratedFilesFromGIT
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text =  ToLabelText("Delete files", "Delete generated files"),
                    Action = (self) => DeleteGeneratedFiles(),
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text =  ToLabelText("Delete folders", "Delete empty folders in the path"),
                    Action = (self) => DeleteEmptyFolders(),
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text =  ToLabelText("Start", "Start code generation"),
                    Action = (self) => StartCodeGeneration(),
                },
            };
            return [.. menuItems.Union(CreateExitMenuItems())];
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
            Generator.DeleteGeneratedFiles(SourcePath);
            if (IncludeCleanDirectory)
            {
                PrintLine("Delete all empty folders...");
                Generator.CleanDirectories(SourcePath);
            }
            PrintLine("Delete all generated files ignored from git...");
            GitIgnoreManager.DeleteIgnoreEntries(SourcePath);
            StopProgressBar();
        }

        /// <summary>
        /// Deletes all empty folders within the specified solution path.
        /// </summary>
        /// <remarks>
        /// This method invokes the <see cref="ProgressBar.Start"/> method to display a progress bar
        /// and writes a message to the console indicating that all empty folders are being deleted.
        /// It then calls the <see cref="Generator.CleanDirectories(string)"/> method to perform the deletion.
        /// </remarks>
        private void DeleteEmptyFolders()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Delete all empty folders...");
            Generator.CleanDirectories(SourcePath);
            StopProgressBar();
        }
        /// <summary>
        /// Generates code based on the specified solution properties and logic assembly types.
        /// Deletes any previously generated files and writes the new code items to files.
        /// Optionally adds or removes generated files from the gitignore file.
        /// </summary>
        private void StartCodeGeneration()
        {
            var logicAssemblyTypes = Logic.Modules.CodeGenerator.AssemblyAccess.AllTypes;
            var solutionProperties = SolutionProperties.Create(SourcePath, logicAssemblyTypes);
            IEnumerable<IGeneratedItem>? generatedItems;

            PrintHeader();
            StartProgressBar();
            PrintLine("Generate code...");
            Console.WriteLine("Create code items...");
            generatedItems = Generator.Generate(solutionProperties);

            Console.WriteLine("Delete all generated files...");
            Generator.DeleteGeneratedFiles(SourcePath);
            if (IncludeCleanDirectory)
            {
                Console.WriteLine("Delete all empty folders...");
                Generator.CleanDirectories(SourcePath);
            }
            Console.WriteLine("Write code items to files...");
            Writer.WriteToGroupFile = ToGroupFile;
            Writer.WriteAll(SourcePath, solutionProperties, generatedItems);
            if (ExcludeGeneratedFilesFromGIT)
            {
                Console.WriteLine("All generated files are added to gitignore...");
                GitIgnoreManager.Run(SourcePath);
            }
            else
            {
                Console.WriteLine("Remove all generated files from gitignore...");
                GitIgnoreManager.DeleteIgnoreEntries(SourcePath);
            }
            StopProgressBar();
            Thread.Sleep(700);
        }
        /// <summary>
        /// Retrieves the name of the solution file without the extension from the given solution path.
        /// </summary>
        /// <param name="solutionPath">The path to the solution file.</param>
        /// <returns>The name of the solution file without the extension, or an empty string if the file does not exist.</returns>
        private static string GetSolutionName(string solutionPath)
        {
            var fileInfo = new DirectoryInfo(solutionPath).GetFiles().SingleOrDefault(f => f.Extension.Equals(".sln", StringComparison.CurrentCultureIgnoreCase));
            
            return fileInfo != null ? Path.GetFileNameWithoutExtension(fileInfo.Name) : string.Empty;
        }
        #endregion app methods
    }
}
