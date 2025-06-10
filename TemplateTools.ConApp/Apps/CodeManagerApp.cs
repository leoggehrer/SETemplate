//@BaseCode
using System.Text;

namespace TemplateTools.ConApp.Apps
{
    internal partial class CodeManagerApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the CodeGeneratorApp class.
        /// </summary>
        static CodeManagerApp()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called when the class is being constructed.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        /// <remarks>
        /// This method is called internally and is intended for internal use only.
        /// </remarks>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region Instance-Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGeneratorApp"/> class.
        /// </summary>
        public CodeManagerApp()
        {
            Constructing();
            Constructed();
        }
        /// <summary>
        /// This method is called during the construction of the object.
        /// </summary>
        partial void Constructing();
        /// <summary>
        /// This method is called after the object is constructed.
        /// </summary>
        partial void Constructed();
        #endregion Instance-Constructors

        #region properties
        /// <summary>
        /// Gets or sets the path of the solution.
        /// </summary>
        private string CodeSolutionPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the path from which code will be imported.
        /// </summary>
        private string ImportPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the name of the file to import entities from.
        /// </summary>
        private string ImportFileName { get; set; } = "entities.cs";
        /// <summary>
        /// Gets or sets the path from which code will be exported.
        /// </summary>
        private string ExportPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the name of the file to export entities from.
        /// </summary>
        private string ExportFileName { get; set; } = "entities.cs";
        #endregion properties

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
                    Key = "-----",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Override files", "Change force file option"),
                    Action = (self) => Force = !Force
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Source path", "Change the source solution path"),
                    Action = (self) =>
                    {
                        var result = ChangeTemplateSolutionPath(CodeSolutionPath, MaxSubPathDepth, ReposPath);

                        if (result.HasContent())
                        {
                            CodeSolutionPath = result;
                        }
                    }
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Import path", "Change the import code path"),
                    Action = (self) =>
                    {
                        var result = ChangePath("Import path: ", ImportPath);

                        if (result.HasContent())
                        {
                            ImportPath = result;
                        }
                    }
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Import file name", "Change the import file name"),
                    Action = (self) =>
                    {
                        var result = ReadLine("Import file name: ");

                        if (result.HasContent())
                        {
                            ImportFileName = result;
                        }
                    }
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
                    Text = ToLabelText("Export path", "Change the export code path"),
                    Action = (self) =>
                    {
                        var result = ChangePath("Export path: ", ExportPath);

                        if (result.HasContent())
                        {
                            ExportPath = result;
                        }
                    }
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Export file name", "Change the export file name"),
                    Action = (self) =>
                    {
                        var result = ReadLine("Export file name: ");

                        if (result.HasContent())
                        {
                            ExportFileName = result;
                        }
                    }
                },
                new()
                {
                    Key = "-----",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
            };

            var importFilePath = Path.Combine(ImportPath, ImportFileName);
            var importFileExists = File.Exists(importFilePath);

            if (importFileExists)
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "import_entities",
                    Text = ToLabelText("Import entities", "Start import from entities"),
                    Action = (self) => StartImportEntities(),
                });
            }
            else
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "import_entities",
                    Text = ToLabelText("Import entities", "Start import from entities"),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.Red,
                });
            }

            menuItems.Add(new()
            {
                Key = "-----",
                Text = new string('-', 65),
                Action = (self) => { },
                ForegroundColor = ConsoleColor.DarkGreen,
            });
            menuItems.Add(new()
            {
                Key = $"{++mnuIdx}",
                OptionalKey = "export_entities",
                Text = ToLabelText("Export entities", "Start export from entities"),
                Action = (self) => StartExportEntities(),
            });

            return [.. menuItems.Union(CreateExitMenuItems())];
        }
        /// <summary>
        /// Prints the header for the PlantUML application.
        /// </summary>
        protected override void PrintHeader()
        {
            List<KeyValuePair<string, object>> headerParams =
            [
                new("Override files:", $"{Force}"),
                new("Solution path:", CodeSolutionPath),
                new("Import path:", ImportPath),
                new("Import file name:", ImportFileName),
                new("Export path:", ExportPath),
                new("Export file name:", ExportFileName),
                new(new string('-', 25), ""),
            ];

            base.PrintHeader("Template Code Manager", [.. headerParams]);
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
                else if (arg.Key.Equals(nameof(CodeSolutionPath), StringComparison.OrdinalIgnoreCase))
                {
                    CodeSolutionPath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(ImportPath), StringComparison.OrdinalIgnoreCase))
                {
                    ImportPath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(ImportFileName), StringComparison.OrdinalIgnoreCase))
                {
                    ImportFileName = arg.Value;
                }
                else if (arg.Key.Equals(nameof(ExportPath), StringComparison.OrdinalIgnoreCase))
                {
                    ExportPath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(ExportFileName), StringComparison.OrdinalIgnoreCase))
                {
                    ExportFileName = arg.Value;
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
        /// <summary>
        /// Starts the process of importing entities from the specified import file.
        /// Prints the application header, starts a progress bar, logs the import action,
        /// performs the import operation, and restarts the progress bar.
        /// </summary>
        public void StartImportEntities()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Import entities ...");
            ImportEntities();
            StartProgressBar();
        }
        /// <summary>
        /// Imports entity classes from the specified import file into the solution's Logic project.
        /// Reads the entities file, extracts namespaces and class definitions, and writes them to the appropriate
        /// location in the Logic project, creating directories and files as needed. If the <c>Force</c> property is set,
        /// existing files will be overwritten.
        /// </summary>
        private void ImportEntities()
        {
            var sourceSolutionName = TemplatePath.GetSolutionName(CodeSolutionPath);
            var importFilePath = Path.Combine(ImportPath, ImportFileName);
            var fileExists = File.Exists(importFilePath);

            if (sourceSolutionName.HasContent() && fileExists)
            {
                var entities = File.ReadAllText(importFilePath);
                var tags = entities.GetAllTags("namespace ", "{");

                System.Diagnostics.Debug.WriteLine($"File {importFilePath} does not exist.");

                foreach (var tag in tags)
                {
                    var logicProject = $"{sourceSolutionName}.Logic";
                    var parentNamespace = CreateParentNamespace(tag.FullText, "Entities");
                    var subNamespace = CreateSubNamespace(tag.FullText, "Entities");
                    var fullNamespace = $"{logicProject}.{subNamespace}";
                    var entityCode = entities.ExtractBetween('{', '}', tag.StartTagIndex).Replace(parentNamespace, logicProject);
                    var entityName = entityCode.ExtractBetween(" class ", ":").RemoveLeftAndRight(' ');
                    var entityPath = Path.Combine(CodeSolutionPath, logicProject, subNamespace.Replace('.', Path.DirectorySeparatorChar));
                    var entityFilePath = Path.Combine(CodeSolutionPath, logicProject, subNamespace.Replace('.', Path.DirectorySeparatorChar), $"{entityName}.cs");
                    var fullEntityCode = ($"namespace {fullNamespace} " + '\n' + '{' + $"{entityCode}" + '}');

                    if (Force || File.Exists(entityFilePath) == false)
                    {
                        if (Directory.Exists(entityPath) == false)
                        {
                            Directory.CreateDirectory(entityPath);
                        }
                        File.WriteAllText(entityFilePath, fullEntityCode, encoding: System.Text.Encoding.UTF8);
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"File {importFilePath} does not exist.");
            }
        }
        /// <summary>
        /// Starts the process of exporting entities from the Logic project to the specified export file.
        /// Prints the application header, starts a progress bar, logs the export action,
        /// performs the export operation, and restarts the progress bar.
        /// </summary>
        public void StartExportEntities()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Export entities ...");
            ExportEntities();
            StartProgressBar();
        }
        /// <summary>
        /// Exports entity classes from the Logic project's Entities folder to a single export file.
        /// Reads all .cs files in the Entities directory (excluding ignored files), concatenates their contents,
        /// and writes the result to the specified export file. If the <c>Force</c> property is set or the export file does not exist,
        /// the export file will be created or overwritten.
        /// </summary>
        private void ExportEntities()
        {
            var sourceSolutionName = TemplatePath.GetSolutionName(CodeSolutionPath);
            var exportFilePath = Path.Combine(ExportPath, ExportFileName);
            var logicProject = $"{sourceSolutionName}.Logic";
            var entityPath = Path.Combine(CodeSolutionPath, logicProject, "Entities");
            var entityPathExits = Directory.Exists(entityPath);

            if (entityPathExits)
            {
                var exportCode = new StringBuilder();
                var files = GetSourceCodeFiles(entityPath, "*.cs");

                foreach (var file in files)
                {
                    var entityCode = File.ReadAllText(file);

                    if (exportCode.Length > 0)
                    {
                        exportCode.AppendLine();
                    }
                    exportCode.Append(entityCode);
                }

                if (Force || File.Exists(exportFilePath) == false)
                {
                    if (Directory.Exists(ExportPath) == false)
                    {
                        Directory.CreateDirectory(ExportPath);
                    }
                    File.WriteAllText(exportFilePath, exportCode.ToString(), encoding: System.Text.Encoding.UTF8);
                }
            }
        }

        /// <summary>
        /// Retrieves a list of source code files from the specified path, excluding files in ignored folders
        /// and files that contain specific ignore labels in their first line.
        /// </summary>
        /// <param name="path">The root directory to search for source code files.</param>
        /// <param name="searchPattern">The search pattern to match files (e.g., "*.cs").</param>
        /// <returns>A list of file paths that match the criteria.</returns>
        private static List<string> GetSourceCodeFiles(string path, string searchPattern)
        {
            var result = new List<string>();
            var files = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories)
                                 .Where(f => CommonStaticLiterals.GenerationIgnoreFolders.Any(e => f.Contains(e)) == false)
                                 .OrderBy(i => i);

            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file, Encoding.Default);

                if (lines.Length != 0
                    && lines.First().Contains(Common.StaticLiterals.GeneratedCodeLabel) == false
                    && lines.First().Contains(Common.StaticLiterals.IgnoreLabel) == false
                    && lines.First().Contains(Common.StaticLiterals.BaseCodeLabel) == false
                    && lines.First().Contains(Common.StaticLiterals.CodeCopyLabel) == false)
                {
                    result.Add(file);
                }
            }
            return result;
        }
        /// <summary>
        /// Creates the parent namespace string from the given full namespace, stopping at the specified item.
        /// Splits the full namespace by '.', and collects each item until <paramref name="toItem"/> is found.
        /// Each item is cleaned of non-letter/digit characters.
        /// </summary>
        /// <param name="fullNamespace">The full namespace string to process.</param>
        /// <param name="toItem">The namespace item at which to stop collecting parent namespace items.</param>
        /// <returns>The parent namespace string up to (but not including) <paramref name="toItem"/>.</returns>
        private static string CreateParentNamespace(string fullNamespace, string toItem)
        {
            var start = true;
            var result = new List<string>();
            var items = fullNamespace.Replace("namespace", string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in items)
            {
                if (start && item == toItem)
                {
                    start = false;
                }
                if (start)
                {
                    result.Add(ClearNamespaceItem(item));
                }
            }
            return string.Join(".", result);
        }
        /// <summary>
        /// Creates a sub-namespace string from the given full namespace, starting from the specified item.
        /// Splits the full namespace by '.', finds the first occurrence of <paramref name="startItem"/>,
        /// and returns the sub-namespace including and after that item, with each item cleaned of non-letter/digit characters.
        /// </summary>
        /// <param name="fullNamespace">The full namespace string to process.</param>
        /// <param name="startItem">The namespace item to start the sub-namespace from.</param>
        /// <returns>The sub-namespace string starting from <paramref name="startItem"/>.</returns>
        private static string CreateSubNamespace(string fullNamespace, string startItem)
        {
            var start = false;
            var result = new List<string>();
            var items = fullNamespace.Split('.', StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in items)
            {
                if (start == false && item == startItem)
                {
                    start = true;
                }
                if (start)
                {
                    result.Add(ClearNamespaceItem(item));
                }
            }
            return string.Join(".", result);
        }
        /// <summary>
        /// Removes all non-letter and non-digit characters from the given namespace item string.
        /// </summary>
        /// <param name="item">The namespace item to clean.</param>
        /// <returns>A string containing only letters and digits from the input.</returns>
        private static string ClearNamespaceItem(string item)
        {
            return string.Concat(item.Where(char.IsLetterOrDigit));
        }
        #endregion app methods
    }
}
