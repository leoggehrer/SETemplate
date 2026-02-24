//@BaseCode
using System.Text;
using TemplateTools.Logic;
using TemplateTools.Logic.Extensions;

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
        /// This method is called after the object has been initialized.
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
        private string EntitiesImportFileName { get; set; } = "gpt_entities.cs";
        /// <summary>
        /// Gets or sets the name of the file to import program from.
        /// </summary>
        private string ProgramImportFileName { get; set; } = "gpt_program.cs";

        /// <summary>
        /// Gets or sets the path from which code will be exported.
        /// </summary>
        private string ExportPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the name of the file to export entities from.
        /// </summary>
        private string EntitiesExportFileName { get; set; } = "exp_entities.cs";
        #endregion properties

        #region overrides
        /// <summary>
        /// Creates an array of menu items for the application menu.
        /// </summary>
        /// <returns>An array of MenuItem objects representing the menu items.</returns>
        protected override MenuItem[] CreateMenuItems()
        {
            var importEntitiesFilePath = Path.Combine(ImportPath, EntitiesImportFileName);
            var importEntitiesFileExists = File.Exists(importEntitiesFilePath);
            var entitiesExportFilePath = Path.Combine(ExportPath, EntitiesExportFileName);
            var entitiesExportFileExists = File.Exists(entitiesExportFilePath);

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
                    Key = "-----",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
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
                    Text = ToLabelText("Filename", $"Change the entities import filename '{EntitiesImportFileName}'"),
                    Action = (self) =>
                    {
                        var result = ReadLine("Entities import filename: ");

                        if (result.HasContent())
                        {
                            EntitiesImportFileName = result;
                        }
                    }
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Filename", $"Change the program import filename '{ProgramImportFileName}'"),
                    Action = (self) =>
                    {
                        var result = ReadLine("Program import filename: ");

                        if (result.HasContent())
                        {
                            ProgramImportFileName = result;
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

            if (importEntitiesFileExists)
            {
                menuItems.Add(new()
                {
                    IsDisplayed = false,
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
                    IsDisplayed = false,
                    Key = $"{++mnuIdx}",
                    OptionalKey = "import_entities",
                    Text = ToLabelText("Import entities", "Start import from entities"),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.Red,
                });
            }

            var importProgramFilePath = Path.Combine(ImportPath, ProgramImportFileName);
            var importProgramFileExists = File.Exists(importProgramFilePath);

            if (importProgramFileExists)
            {
                menuItems.Add(new()
                {
                    IsDisplayed = false,
                    Key = $"{++mnuIdx}",
                    OptionalKey = "import_program",
                    Text = ToLabelText("Import program", "Start import partial program"),
                    Action = (self) => StartImportProgram(),
                });
            }
            else
            {
                menuItems.Add(new()
                {
                    IsDisplayed = false,
                    Key = $"{++mnuIdx}",
                    OptionalKey = "import_program",
                    Text = ToLabelText("Import entities", "Start import partial program"),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.Red,
                });
            }

            menuItems.Add(new()
            {
                IsDisplayed = false,
                Key = "-----",
                Text = new string('-', 65),
                Action = (self) => { },
                ForegroundColor = ConsoleColor.DarkGreen,
            });
            menuItems.Add(new()
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
            });
            menuItems.Add(new()
            {
                Key = (++mnuIdx).ToString(),
                Text = ToLabelText("Export filename", $"Change the entities export filename '{EntitiesExportFileName}'"),
                Action = (self) =>
                {
                    var result = ReadLine("Entities export filename: ");

                    if (result.HasContent())
                    {
                        EntitiesExportFileName = result;
                    }
                }
            });
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
                Action = (self) =>
                {
                    if (entitiesExportFileExists == false || Force)
                    {
                        StartExportEntities();
                    }
                },
                ForegroundColor = entitiesExportFileExists == false || Force ? ForegroundColor : ConsoleColor.Red,
            });
            menuItems.Add(new()
            {
                Key = $"{++mnuIdx}",
                OptionalKey = "open_export_entities",
                Text = ToLabelText("Edit entities", "Edit entities file"),
                Action = (self) =>
                {
                    if (entitiesExportFileExists)
                    {
                        OpenTextFile(entitiesExportFilePath);
                    }
                },
                ForegroundColor = entitiesExportFileExists ? ForegroundColor : ConsoleColor.Red,
            });

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
                OptionalKey = "delete_gptfiles",
                Text = ToLabelText("Delete gpt-files", "Delete all chatgpt generated files"),
                Action = (self) => StartDeleteChatGptFiles(),
            });

            return [.. menuItems.Union(CreateExitMenuItems())];
        }
        /// <summary>
        /// Prints the header for the application.
        /// </summary>
        protected override void PrintHeader()
        {
            bool hasPathInfo = false;

            List<KeyValuePair<string, object>> headerParams =
            [
                new("Override files:", $"{Force}"),
                new("Solution path:", CodeSolutionPath),
                new(new string('-', 25), ""),
            ];
            if (CodeSolutionPath != ImportPath)
            {
                hasPathInfo = true;
                headerParams.Add(new($"{nameof(ImportPath).ToCamelCaseSplit()}:", $"{ImportPath}"));
            }
            if (CodeSolutionPath != ExportPath)
            {
                hasPathInfo = true;
                headerParams.Add(new($"{nameof(ExportPath).ToCamelCaseSplit()}:", $"{ExportPath}"));
            }
            if (hasPathInfo)
            {
                headerParams.Add(new(new string('-', 25), string.Empty));
            }

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
                else if (arg.Key.Equals(nameof(EntitiesImportFileName), StringComparison.OrdinalIgnoreCase))
                {
                    EntitiesImportFileName = arg.Value;
                }
                else if (arg.Key.Equals(nameof(ProgramImportFileName), StringComparison.OrdinalIgnoreCase))
                {
                    ProgramImportFileName = arg.Value;
                }
                else if (arg.Key.Equals(nameof(ExportPath), StringComparison.OrdinalIgnoreCase))
                {
                    ExportPath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(EntitiesExportFileName), StringComparison.OrdinalIgnoreCase))
                {
                    EntitiesExportFileName = arg.Value;
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
        /// Starts the process of importing entities from the specified import file.
        /// Prints the application header, starts a progress bar, logs the import action,
        /// performs the import operation, and restarts the progress bar.
        /// </summary>
        public void StartImportEntities()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Import entities ...");
            StartProgressBar();
        }

        /// <summary>
        /// Starts the process of importing program code from the specified import file.
        /// Prints the application header, starts a progress bar, logs the import action,
        /// performs the import operation, and restarts the progress bar.
        /// </summary>
        public void StartImportProgram()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Import program ...");
            StartProgressBar();
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
            var exportFilePath = Path.Combine(ExportPath, EntitiesExportFileName);
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
        /// Starts the process of importing program code from the specified import file.
        /// Prints the application header, starts a progress bar, logs the import action,
        /// performs the import operation, and restarts the progress bar.
        /// </summary>
        public void StartDeleteChatGptFiles()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Delete chatgpt files...");
            DeleteChatGptFiles(CodeSolutionPath);
            StartProgressBar();
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
        #endregion app methods
    }
}
