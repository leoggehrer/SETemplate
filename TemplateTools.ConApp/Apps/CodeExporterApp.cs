//@BaseCode
namespace TemplateTools.ConApp.Apps
{
    using CommonTool.Extensions;
    using System.Text;
    using TemplateTools.ConApp.Modules;
    using TemplateTools.Logic;

    internal partial class CodeExporterApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the CodeGeneratorApp class.
        /// </summary>
        static CodeExporterApp()
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
        public CodeExporterApp()
        {
            Constructing();
            ExportSubPath = Path.Combine($"{TemplatePath.GetSolutionName(CodeSolutionPath)}{StaticLiterals.LogicExtension}", "Modules");
            OutputSubPath = "Export";
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
        /// Gets or sets the path from which code will be exported.
        /// </summary>
        private string ExportPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the sub path from which code will be exported.
        /// </summary>
        private string ExportSubPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the output path where exported files will be saved.
        /// </summary>
        private string OutputPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the output sub path where exported files will be saved.
        /// </summary>
        private string OutputSubPath { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the name of the file to export.
        /// </summary>
        private string OutputFileName { get; set; } = "full_export.cs";
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
            var exportFullPath = Path.Combine(ExportPath, ExportSubPath);
            var exportFullPathExits = Directory.Exists(exportFullPath);

            var outputFullPath = Path.Combine(OutputPath, OutputSubPath);
            var outputFilePath = Path.Combine(outputFullPath, OutputFileName);
            var outputFilePathExists = File.Exists(outputFilePath);

            var entitiesExportFilePath = Path.Combine(outputFullPath, EntitiesExportFileName);
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
                    OptionalKey = $"chg_force",
                    Text = ToLabelText("Override files", "Change force file option"),
                    Action = (self) => Force = !Force
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    OptionalKey = $"chg_codesolutionpath",
                    Text = ToLabelText($"{nameof(CodeSolutionPath).ToCamelCaseSplit()}", $"Change the {nameof(CodeSolutionPath).ToCamelCaseSplit().ToLowerInvariant()}"),
                    Action = (self) =>
                    {
                        var result = ChangeTemplateSolutionPath(CodeSolutionPath, MaxSubPathDepth, ReposPath);

                        if (result.HasContent() && Directory.Exists(result))
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
                    OptionalKey = $"chg_exportpath",
                    Text = ToLabelText($"{nameof(ExportPath).ToCamelCaseSplit()}", $"Change the {nameof(ExportPath).ToCamelCaseSplit().ToLowerInvariant()}"),
                    Action = (self) =>
                    {
                        var result = ChangePath(ExportPath);

                        if (result.HasContent() && Directory.Exists(result))
                        {
                            ExportPath = result;
                        }
                    }
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    OptionalKey = $"chg_exportsubpath",
                    Text = ToLabelText($"{nameof(ExportSubPath).ToCamelCaseSplit()}", $"Change the {nameof(ExportSubPath).ToCamelCaseSplit().ToLowerInvariant()} '{ExportSubPath}'"),
                    Action = (self) =>
                    {
                        var result = ChangePath(ExportSubPath);

                        if (result.HasContent() && Directory.Exists(Path.Combine(ExportPath, result)))
                        {
                            ExportSubPath = result;
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
                    OptionalKey = $"chg_outputpath",
                    Text = ToLabelText($"{nameof(OutputPath).ToCamelCaseSplit()}", $"Change the {nameof(OutputPath).ToCamelCaseSplit().ToLowerInvariant()}"),
                    Action = (self) =>
                    {
                        var result = ChangePath(OutputPath);

                        if (result.HasContent() && Directory.Exists(result))
                        {
                            OutputPath = result;
                        }
                    }
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    OptionalKey = $"chg_outputsubpath",
                    Text = ToLabelText($"{nameof(OutputSubPath).ToCamelCaseSplit()}", $"Change the {nameof(OutputSubPath).ToCamelCaseSplit().ToLowerInvariant()}"),
                    Action = (self) =>
                    {
                        var result = ChangePath(OutputSubPath);

                        if (result.HasContent() && Directory.Exists(Path.Combine(ExportPath, result)))
                        {
                            OutputSubPath = result;
                        }
                    }
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    OptionalKey = $"chg_outputfilename",
                    Text = ToLabelText($"{nameof(OutputFileName).ToCamelCaseSplit()}", $"Change the {nameof(OutputFileName).ToCamelCaseSplit().ToLowerInvariant()} '{OutputFileName}'"),
                    Action = (self) =>
                    {
                        var result = ReadLine($"{nameof(OutputFileName).ToCamelCaseSplit()}: ");

                        if (result.HasContent())
                        {
                            OutputFileName = result;
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
                    OptionalKey = $"start_export",
                    Text = ToLabelText($"Start export", $"Starts the export of classes from the '{ExportSubPath}'"),
                    Action = (self) =>
                    {
                        if (exportFullPathExits && (outputFilePathExists == false || Force))
                        {
                            StartExportClasses();
                        }
                    },
                    ForegroundColor = exportFullPathExits && (outputFilePathExists == false || Force) ? ForegroundColor : ConsoleColor.Red,
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "open_export",
                    Text = ToLabelText("Edit export", $"Edit full export file '{OutputFileName}'"),
                    Action = (self) =>
                    {
                        if (outputFilePathExists)
                        {
                            OpenTextFile(outputFilePath);
                        }
                    },
                    ForegroundColor = outputFilePathExists ? ForegroundColor : ConsoleColor.Red,
                },
                new()
                {
                    IsDisplayed = true,
                    Key = "-----",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
                new()
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
                },
                new()
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
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "open_export_entities",
                    Text = ToLabelText("Edit entities", "Edit export from entities"),
                    Action = (self) =>
                    {
                        if (entitiesExportFileExists)
                        {
                            OpenTextFile(entitiesExportFilePath);
                        }
                    },
                    ForegroundColor = entitiesExportFileExists ? ForegroundColor : ConsoleColor.Red,
                },
            };

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
                new($"{nameof(CodeSolutionPath).ToCamelCaseSplit()}:", CodeSolutionPath),
                new(new string('-', 25), ""),
                new($"{nameof(ExportPath).ToCamelCaseSplit()}:", ExportPath),
                new($"{nameof(ExportSubPath).ToCamelCaseSplit()}:", ExportSubPath),
                new(new string('-', 25), ""),
                new($"{nameof(OutputPath).ToCamelCaseSplit()}:", OutputPath),
                new($"{nameof(OutputSubPath).ToCamelCaseSplit()}:", OutputSubPath),
                new($"{nameof(OutputFileName).ToCamelCaseSplit()}:", OutputFileName
                ),
                new(new string('-', 25), ""),
            ];
            if (CodeSolutionPath != ExportPath)
            {
                hasPathInfo = true;
                headerParams.Add(new($"{nameof(ExportPath).ToCamelCaseSplit()}:", $"{ExportPath}"));
            }
            if (hasPathInfo)
            {
                headerParams.Add(new(new string('-', 25), string.Empty));
            }

            base.PrintHeader("Template Code Exporter", [.. headerParams]);
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
                else if (arg.Key.Equals(nameof(ExportPath), StringComparison.OrdinalIgnoreCase))
                {
                    ExportPath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(ExportSubPath), StringComparison.OrdinalIgnoreCase))
                {
                    ExportSubPath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(OutputPath), StringComparison.OrdinalIgnoreCase))
                {
                    OutputPath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(OutputSubPath), StringComparison.OrdinalIgnoreCase))
                {
                    OutputSubPath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(OutputFileName), StringComparison.OrdinalIgnoreCase))
                {
                    OutputFileName = arg.Value;
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
        /// Starts the process of exporting entities from the Logic project to the specified export file.
        /// Prints the application header, starts a progress bar, logs the export action,
        /// performs the export operation, and restarts the progress bar.
        /// </summary>
        public void StartExportClasses()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Export classes ...");
            ExportClasses();
            StartProgressBar();
        }
        /// <summary>
        /// Exports entity classes from the Logic project's Entities folder to a single export file.
        /// Reads all .cs files in the Entities directory (excluding ignored files), concatenates their contents,
        /// and writes the result to the specified export file. If the <c>Force</c> property is set or the export file does not exist,
        /// the export file will be created or overwritten.
        /// </summary>
        private void ExportClasses()
        {
            var exportFullPath = Path.Combine(ExportPath, ExportSubPath);
            var exportFullPathExits = Directory.Exists(exportFullPath);

            var outputFullPath = Path.Combine(OutputPath, OutputSubPath);
            var outputFilePath = Path.Combine(outputFullPath, OutputFileName);

            if (exportFullPathExits)
            {
                var exportCode = new StringBuilder();
                var files = GetExportCodeFiles(exportFullPath, "*.cs");

                foreach (var file in files)
                {
                    var classCode = File.ReadAllText(file).NormalizeLineEndings();
                    var preparedCode = CodeExporter.PrepareCSharpItemForExport(file, classCode);

                    if (exportCode.Length > 0)
                    {
                        exportCode.AppendLine();
                    }
                    exportCode.Append(preparedCode);

                    var copyFilePath = file.Replace(exportFullPath, outputFullPath);
                    var copyPath = Path.GetDirectoryName(copyFilePath);

                    if (string.IsNullOrEmpty(copyPath) == false)
                    {
                        if (Directory.Exists(copyPath) == false)
                        {
                            Directory.CreateDirectory(copyPath);
                        }
                        File.WriteAllText(copyFilePath, preparedCode.ToString(), encoding: Encoding.UTF8);
                    }
                }

                if (Force || File.Exists(outputFilePath) == false)
                {
                    if (Directory.Exists(outputFullPath) == false)
                    {
                        Directory.CreateDirectory(outputFullPath);
                    }
                    File.WriteAllText(outputFilePath, exportCode.ToString(), encoding: Encoding.UTF8);
                }
            }
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
            var logicProject = $"{sourceSolutionName}.Logic";
            var entityPath = Path.Combine(CodeSolutionPath, logicProject, "Entities");
            var entityPathExits = Directory.Exists(entityPath);

            var outputFullPath = Path.Combine(OutputPath, OutputSubPath);
            var outputFilePath = Path.Combine(outputFullPath, EntitiesExportFileName);

            if (entityPathExits)
            {
                var exportCode = new StringBuilder();
                var files = GetExportEntityFiles(entityPath, "*.cs");

                foreach (var file in files)
                {
                    var entityCode = File.ReadAllText(file);
                    var prepareCode = CodeExporter.PrepareCSharpItemForExport(file, entityCode);

                    if (exportCode.Length > 0)
                    {
                        exportCode.AppendLine();
                    }
                    exportCode.Append(prepareCode);

                    var fileName = $"exp_{Path.GetFileName(file)}";
                    var filePath = Path.Combine(outputFullPath, fileName);

                    if (Force || File.Exists(filePath) == false)
                    {
                        if (Directory.Exists(outputFullPath) == false)
                        {
                            Directory.CreateDirectory(outputFullPath);
                        }
                        File.WriteAllText(filePath, prepareCode, encoding: Encoding.UTF8);
                    }
                }

                if (Force || File.Exists(outputFilePath) == false)
                {
                    if (Directory.Exists(outputFullPath) == false)
                    {
                        Directory.CreateDirectory(outputFullPath);
                    }
                    File.WriteAllText(outputFilePath, exportCode.ToString(), encoding: Encoding.UTF8);
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
        private static List<string> GetExportCodeFiles(string path, string searchPattern)
        {
            var result = new List<string>();
            var files = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories)
                                 .Where(f => CommonStaticLiterals.GenerationIgnoreFolders.Any(e => f.Contains(e)) == false)
                                 .OrderBy(i => i);

            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file, Encoding.Default);

                if (lines.Length != 0
                    && lines.First().Contains(Common.StaticLiterals.GeneratedCodeLabel) == false)
                {
                    result.Add(file);
                }
            }
            return result;
        }
        /// <summary>
        /// Retrieves a list of source code files from the specified path, excluding files in ignored folders
        /// and files that contain specific ignore labels in their first line.
        /// </summary>
        /// <param name="path">The root directory to search for source code files.</param>
        /// <param name="searchPattern">The search pattern to match files (e.g., "*.cs").</param>
        /// <returns>A list of file paths that match the criteria.</returns>
        private static List<string> GetExportEntityFiles(string path, string searchPattern)
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
