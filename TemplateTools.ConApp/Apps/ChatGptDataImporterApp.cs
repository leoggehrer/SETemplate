//@BaseCode
namespace TemplateTools.ConApp.Apps
{
    internal partial class ChatGptDataImporterApp : ChatGptApp
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the CodeGeneratorApp class.
        /// </summary>
        static ChatGptDataImporterApp()
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
        /// Initializes a new instance of the <see cref="ChatGptEntityCreatorApp"/> class.
        /// </summary>
        public ChatGptDataImporterApp()
        {
            Constructing();
            InstrcutionsFileName = "import_instructions.txt";
            RequirementFileName = "README.md";
            OutputFileName = "gpt_program.cs";
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

        #region overrides
        /// <summary>
        /// Creates an array of menu items for the application menu.
        /// </summary>
        /// <returns>An array of MenuItem objects representing the menu items.</returns>
        protected override MenuItem[] CreateMenuItems()
        {
            var taskFilePath = Path.Combine(RequirementPath, RequirementFileName);
            var taskFileExists = File.Exists(taskFilePath);
            var outpuFilePath = Path.Combine(OutputPath, OutputFileName);
            var outputFileExists = File.Exists(outpuFilePath);

            var menuItems = new List<MenuItem>(base.CreateMenuItems());
            var mnuIdx = menuItems.Where(e => e.Key.All(char.IsDigit))
                                  .Select(e => int.Parse(e.Key))
                                  .DefaultIfEmpty(0).Max();

            if (taskFileExists && (outputFileExists == false || Force))
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "start",
                    Text = ToLabelText("Create import", "Start create import [csv] with ChatGpt"),
                    Action = (self) => StartCreateImport(),
                });
            }
            else
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "start",
                    Text = ToLabelText("Create import", "Start create import [csv] with ChatGpt"),
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

            if (outputFileExists)
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "start",
                    Text = ToLabelText("Import program", "Start import program from ChatGpt"),
                    Action = (self) => StartImportProgram(),
                });
            }
            else
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "start",
                    Text = ToLabelText("Import program", "Start import program from ChatGpt"),
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

            if (taskFileExists && (outputFileExists == false || Force))
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "start_import",
                    Text = ToLabelText("Create and import", "Start create import [csv] with ChatGpt and import"),
                    Action = (self) => StartCreateImport(),
                });
            }
            else
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "start_import",
                    Text = ToLabelText("Create and import", "Start create import [csv] with ChatGpt and import"),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.Red,
                });
            }
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
                new(new string('-', 25), string.Empty),
                new($"{nameof(ChatGptUrl)}:", $"{ChatGptUrl}"),
                new($"{nameof(ChatGptApiKey)}:", $"{GetDisplayChatCptAppKey()}"),
                new($"{nameof(ChatGptModel)}:", $"{ChatGptModel}"),
                new(new string('-', 25), string.Empty),
            ];
            if (CodeSolutionPath != InstructionPath)
            {
                hasPathInfo = true;
                headerParams.Add(new($"{nameof(InstructionPath).ToCamelCaseSplit()}:", $"{InstructionPath}"));
            }
            if (CodeSolutionPath != RequirementPath)
            {
                hasPathInfo = true;
                headerParams.Add(new($"{nameof(RequirementPath).ToCamelCaseSplit()}:", $"{RequirementPath}"));
            }
            if (CodeSolutionPath != OutputPath)
            {
                hasPathInfo = true;
                headerParams.Add(new($"{nameof(OutputPath).ToCamelCaseSplit()}:", $"{OutputPath}"));
            }
            if (hasPathInfo)
            {
                headerParams.Add(new(new string('-', 25), string.Empty));
            }
            base.PrintHeader("Template ChatGpt Import", [.. headerParams]);
        }
        #endregion overrides

        #region app methods
        /// <summary>
        /// Starts the process of importing entities from the specified import file.
        /// Prints the application header, starts a progress bar, logs the import action,
        /// performs the import operation, and restarts the progress bar.
        /// </summary>
        public void StartCreateImport()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Create import with ChatGpt...");
            Task.Run(async () => await CreateImportByChatGptAsync()).Wait();
            StartProgressBar();
        }
        /// <summary>
        /// Asynchronously creates an import by invoking the ChatGPT API to process the import instructions and task file.
        /// This method delegates the actual work to <see cref="LetChatGptDoItsWork"/> and returns its task.
        /// </summary>
        /// <remarks>
        /// This method invokes <see cref="LetChatGptDoItsWork"/> to perform the entity creation process using ChatGPT.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected Task CreateImportByChatGptAsync()
        {
            return LetChatGptDoItsWork();
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
            ImportProgram();
            StartProgressBar();
        }

        /// <summary>
        /// Imports program code from the specified import file into the solution's ConApp project.
        /// Reads the program file, extracts namespaces and class definitions, and writes them to the appropriate
        /// location in the ConApp project, creating directories and files as needed. If the <c>Force</c> property is set,
        /// existing files will be overwritten.
        /// </summary>
        private void ImportProgram()
        {
            var sourceSolutionName = TemplatePath.GetSolutionName(CodeSolutionPath);
            var importFilePath = Path.Combine(OutputPath, OutputFileName);
            var fileExists = File.Exists(importFilePath);

            if (sourceSolutionName.HasContent() && fileExists)
            {
                var importCode = File.ReadAllText(importFilePath);

                Modules.CodeCreator.CreateImportWithData(CodeSolutionPath, importCode, Force);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"File {importFilePath} does not exist.");
            }
        }
        #endregion app methods
    }
}
