//@BaseCode
namespace TemplateTools.ConApp.Apps
{
    internal partial class ChatGptEntityCreatorApp : ChatGptApp
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the CodeGeneratorApp class.
        /// </summary>
        static ChatGptEntityCreatorApp()
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
        public ChatGptEntityCreatorApp()
        {
            Constructing();
            InstrcutionsFileName = "entities_instructions.txt";
            OutputFileName = "gpt_entities.cs";
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
                    Text = ToLabelText("Create entities", "Start create entities with ChatGpt"),
                    Action = (self) => StartCreateEntities(),
                });
            }
            else
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "start",
                    Text = ToLabelText("Create entities", "Start create entities with ChatGpt"),
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
                    Text = ToLabelText("Import entities", "Start import entities from ChatGpt"),
                    Action = (self) => StartImportEntities(),
                });
            }
            else
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "start",
                    Text = ToLabelText("Import entities", "Start import entities from ChatGpt"),
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
                    OptionalKey = "start",
                    Text = ToLabelText("Create and import", "Start create entities with ChatGpt and import"),
                    Action = (self) =>
                    {
                        StartCreateEntities();
                        StartImportEntities();
                    },
                });
            }
            else
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "start",
                    Text = ToLabelText("Create and import", "Start create entities with ChatGpt and import"),
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
            base.PrintHeader("Template ChatGpt Entities", [.. headerParams]);
        }
        #endregion overrides

        #region app methods
        /// <summary>
        /// Starts the process of importing entities from the specified import file.
        /// Prints the application header, starts a progress bar, logs the import action,
        /// performs the import operation, and restarts the progress bar.
        /// </summary>
        public void StartCreateEntities()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Create entities with ChatGpt...");
            Task.Run(async () => await CreateEntitiesByChatGptAsync()).Wait();
            StartProgressBar();
        }
        /// <summary>
        /// Asynchronously creates entities by delegating the operation to the ChatGPT workflow.
        /// </summary>
        /// <remarks>
        /// This method invokes <see cref="LetChatGptDoItsWork"/> to perform the entity creation process using ChatGPT.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation of creating entities.
        /// </returns>
        protected Task CreateEntitiesByChatGptAsync()
        {
            return LetChatGptDoItsWork();
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
            var importFilePath = Path.Combine(OutputPath, OutputFileName);
            var fileExists = File.Exists(importFilePath);

            if (sourceSolutionName.HasContent() && fileExists)
            {
                var importText = File.ReadAllText(importFilePath);

                Modules.CodeImporter.ImportCSharpItems(CodeSolutionPath, importText, Force);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"File {importFilePath} does not exist.");
            }
        }
        #endregion app methods
    }
}
