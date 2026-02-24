//@BaseCode
using TemplateTools.Logic;
using TemplateTools.Logic.Contracts;
using TemplateTools.Logic.Git;

namespace SETemplate.CodeGenApp
{
    public partial class CodeGeneratorApp : ConsoleApplication
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
            WriteToGroupFile = false;
            WriteInfoHeader = true;
            IncludeDeleteEmptyFolders = true;
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
        /// This method is called after the object has been initialized.
        /// </summary>
        partial void Constructed();
        #endregion Instance-Constructors 

        #region properties
        /// <summary>
        /// Gets or sets a value indicating whether the file should be grouped.
        /// </summary>
        private static bool WriteToGroupFile { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the file should be added info header.
        /// </summary>
        private static bool WriteInfoHeader { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the empty folders in the source path will be deleted.
        /// </summary>
        private static bool IncludeDeleteEmptyFolders { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether generated files are excluded from GIT.
        /// </summary>
        private static bool ExcludeGeneratedFilesFromGIT { get; set; }
        #endregion properties

        #region overrides
        /// <summary>
        /// Prints the header for the application.
        /// </summary>
        protected override void PrintHeader()
        {
            var solutionName = GetSolutionName(SourcePath);
            List<KeyValuePair<string, object>> headerParams =
            [
                new("Solution path:", SourcePath),
                new("Code generation for:", solutionName),
                new(new string('-', 33), ""),
                new("Write generated source into:", WriteToGroupFile ? "Group files" : "Single files"),
                new("Write info header into source:", WriteInfoHeader),
                new("Delete empty folders in the path:", IncludeDeleteEmptyFolders),
                new("Exclude generated files from GIT:", ExcludeGeneratedFilesFromGIT),
            ];

            base.PrintHeader("Source Code Generator", [.. headerParams]);
        }
        /// <summary>
        /// Prints the footer for the application.
        /// </summary>
        protected override void PrintFooter()
        {
            PrintLine();
            Print("Choose [n|n,n|x|X]: ");
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
                    OptionalKey = "chg_gen_option",
                    Text = ToLabelText("Generation file", "Change generation file option"),
                    Action = (self) => WriteToGroupFile = !WriteToGroupFile
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    OptionalKey = "chg_info_header",
                    Text = ToLabelText("Add info header", "Change add info header option"),
                    Action = (self) => WriteInfoHeader = !WriteInfoHeader
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    OptionalKey = "chg_del_folders",
                    Text = ToLabelText("Delete folders", "Change delete empty folders option"),
                    Action = (self) => IncludeDeleteEmptyFolders = !IncludeDeleteEmptyFolders
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    OptionalKey = "chg_exc_files",
                    Text = ToLabelText("Exclude files", "Change the exclusion of generated files from GIT"),
                    Action = (self) => ExcludeGeneratedFilesFromGIT = !ExcludeGeneratedFilesFromGIT
                },
                new()
                {
                    Key = "-----",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    OptionalKey = "del_gen_files",
                    Text =  ToLabelText("Delete files", "Delete all generated files"),
                    Action = (self) => DeleteGeneratedFiles(),
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    OptionalKey = "del_emp_folders",
                    Text =  ToLabelText("Delete folders", "Delete empty folders in the path"),
                    Action = (self) => DeleteEmptyFolders(),
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    OptionalKey = "start",
                    Text =  ToLabelText("Start", "Start code generation"),
                    Action = (self) =>
                    {
                        DeleteGeneratedFiles();
                        StartCodeGeneration();
                    },
                },
                new()
                {
                    IsDisplayed = false,
                    Key = (++mnuIdx).ToString(),
                    OptionalKey = "start_code_gen",
                    Text =  ToLabelText("Start", "Start code generation"),
                    Action = (self) =>
                    {
                        StartCodeGeneration();
                    },
                },
            };
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
                if (arg.Key.Equals(nameof(WriteToGroupFile), StringComparison.OrdinalIgnoreCase))
                {
                    if (bool.TryParse(arg.Value, out bool result))
                    {
                        WriteToGroupFile = result;
                    }
                }
                else if (arg.Key.Equals(nameof(WriteInfoHeader), StringComparison.OrdinalIgnoreCase))
                {
                    if (bool.TryParse(arg.Value, out bool result))
                    {
                        WriteInfoHeader = result;
                    }
                }
                else if (arg.Key.Equals(nameof(IncludeDeleteEmptyFolders), StringComparison.OrdinalIgnoreCase))
                {
                    if (bool.TryParse(arg.Value, out bool result))
                    {
                        IncludeDeleteEmptyFolders = result;
                    }
                }
                else if (arg.Key.Equals(nameof(ExcludeGeneratedFilesFromGIT), StringComparison.OrdinalIgnoreCase))
                {
                    if (bool.TryParse(arg.Value, out bool result))
                    {
                        ExcludeGeneratedFilesFromGIT = result;
                    }
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
        /// Deletes all generated files and directories from the solution path.
        /// </summary>
        private void DeleteGeneratedFiles()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Delete all generated files...");
            Generator.DeleteGeneratedFiles(SourcePath);
            if (IncludeDeleteEmptyFolders)
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
            var invalidEntities = 0;
            var logicAssemblyTypes = Logic.Modules.CodeGenerator.AssemblyAccess.AllTypes;
            var solutionProperties = SolutionProperties.Create(SourcePath, logicAssemblyTypes);
            IEnumerable<IGeneratedItem>? generatedItems;

            PrintHeader();
            StartProgressBar();
            PrintLine("Generate code...");

/*
            Console.WriteLine("Delete all generated files...");
            Generator.DeleteGeneratedFiles(SourcePath);
            if (IncludeDeleteEmptyFolders)
            {
                Console.WriteLine("Delete all empty folders...");
                Generator.CleanDirectories(SourcePath);
            }
*/
            CanProgressBarPrint = false;
            Console.WriteLine("Check entity types...");
            foreach (var item in Logic.Modules.CodeGenerator.AssemblyAccess.EntityTypes)
            {
                if (Generator.IsEntity(item) == false && Generator.IsView(item) == false)
                {
                    var saveColor = Console.ForegroundColor;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($" + Invalid entity type: {item.Name} - this type is not an entity or a view type!");
                    Console.ForegroundColor = saveColor;
                    invalidEntities++;
                }
            }

            if (invalidEntities > 0)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.WriteLine();
            }

            CanProgressBarPrint = true;
            Console.WriteLine("Create code items...");
            generatedItems = Generator.Generate(solutionProperties);

            Console.WriteLine("Write code items to files...");
            Writer.WriteToGroupFile = WriteToGroupFile;
            Writer.WriteInfoHeader = WriteInfoHeader;
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
