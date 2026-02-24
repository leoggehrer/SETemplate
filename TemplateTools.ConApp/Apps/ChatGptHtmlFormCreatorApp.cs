//@BaseCode
using System.Text;

namespace TemplateTools.ConApp.Apps
{
    internal partial class ChatGptHtmlFormCreatorApp : ChatGptApp
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the CodeGeneratorApp class.
        /// </summary>
        static ChatGptHtmlFormCreatorApp()
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
        public ChatGptHtmlFormCreatorApp()
        {
            Constructing();
            InstrcutionsFileName = "forms_instructions.txt";
            RequirementFileName = "gpt_entities.cs";
            OutputFileName = "gpt_forms.html";
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
                    Text = ToLabelText("Create forms", "Start create html forms with ChatGpt"),
                    Action = (self) => StartCreateForms(),
                });
            }
            else
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "start",
                    Text = ToLabelText("Create forms", "Start create html forms with ChatGpt"),
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
                    Text = ToLabelText("Import forms", "Start import forms from ChatGpt"),
                    Action = (self) => StartImportForms(),
                });
            }
            else
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "start",
                    Text = ToLabelText("Import forms", "Start import forms from ChatGpt"),
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
                    Text = ToLabelText("Create and import", "Start create html forms with ChatGpt and import"),
                    Action = (self) =>
                    {
                        StartCreateForms();
                        StartImportForms();
                    },
                });
            }
            else
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "start_import",
                    Text = ToLabelText("Create and import", "Start create html forms with ChatGpt and import"),
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
                    OptionalKey = "open_preview",
                    Text = ToLabelText("Open preview", "Open the html preview"),
                    Action = (self) =>
                    {
                        CreateAndOpenHtmlPreview();
                    },
                });
            }
            else
            {
                menuItems.Add(new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "open_preview",
                    Text = ToLabelText("Open preview", "Open the html preview"),
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
            base.PrintHeader("Template ChatGpt HtmlForm", [.. headerParams]);
        }
        #endregion overrides

        #region AngularApp-Definitions
        public static string SrcAppPath => Path.Combine("src", "app");
        /// <summary>
        /// Gets the subfolder path for the generated enums.
        /// </summary>
        public static string EnumsSubFolder => Path.Combine(SourcePath, "enums");
        /// <summary>
        /// Gets the subfolder path for models under the core app.
        /// </summary>
        public static string ModelsSubFolder => Path.Combine(SourcePath, "models");
        /// <summary>
        ///     Gets the subfolder path for the services in the application's core.
        /// </summary>
        public static string ServicesSubFolder => Path.Combine(SourcePath, "services", "http");
        /// <summary>
        ///     Gets the subfolder path for the components in the application's core.
        /// </summary>
        public static string ComponentsSubFolder => Path.Combine(SourcePath, "components");
        /// <summary>
        ///     Gets the subfolder path for the pages in the application's core.
        /// </summary>
        public static string PagesSubFolder => Path.Combine(SourcePath, "pages");
        /// <summary>
        /// Gets or sets the source namespace.
        /// </summary>
        public static string SourceNameSpace => "src";
        #endregion AngularApp-Definitions

        #region app methods
        /// <summary>
        /// Starts the process of importing entities from the specified import file.
        /// Prints the application header, starts a progress bar, logs the import action,
        /// performs the import operation, and restarts the progress bar.
        /// </summary>
        public void StartCreateForms()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Create forms with ChatGpt...");
            Task.Run(async () => await CreateFormsByChatGptAsync()).Wait();
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
        protected Task CreateFormsByChatGptAsync()
        {
            return LetChatGptDoItsWork();
        }
        /// <summary>
        /// Starts the process of importing entities from the specified import file.
        /// Prints the application header, starts a progress bar, logs the import action,
        /// performs the import operation, and restarts the progress bar.
        /// </summary>
        public void StartImportForms()
        {
            PrintHeader();
            StartProgressBar();
            PrintLine("Import forms ...");
            ImportForms();
            StartProgressBar();
        }
        /// <summary>
        /// Imports entity classes from the specified import file into the solution's Logic project.
        /// Reads the entities file, extracts namespaces and class definitions, and writes them to the appropriate
        /// location in the Logic project, creating directories and files as needed. If the <c>Force</c> property is set,
        /// existing files will be overwritten.
        /// </summary>
        private void ImportForms()
        {
            var sourceSolutionName = TemplatePath.GetSolutionName(CodeSolutionPath);
            var importFilePath = Path.Combine(OutputPath, OutputFileName);
            var fileExists = File.Exists(importFilePath);

            if (sourceSolutionName.HasContent() && fileExists)
            {
                var importText = File.ReadAllText(importFilePath);

                Modules.CodeImporter.ImportHtmlForms(CodeSolutionPath, importText, Force);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"File {importFilePath} does not exist.");
            }
        }

        /// <summary>
        /// Creates an HTML preview file by extracting form HTML code from the output file and embedding it into a Bootstrap-based template.
        /// The generated preview file is saved as "preview.html" in the output path and opened in the default browser.
        /// </summary>
        private void CreateAndOpenHtmlPreview()
        {
            var importFilePath = Path.Combine(OutputPath, OutputFileName);
            var importFileExists = File.Exists(importFilePath);

            if (importFileExists)
            {
                var importText = File.ReadAllText(importFilePath);

                Modules.CodeCreator.CreateAndOpenHtmlPreview(importText);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"File {importFilePath} does not exist.");
            }
        }

        /// <summary>
        /// Converts a file item into a normalized format.
        /// </summary>
        /// <param name="fileItem">The file item to be converted.</param>
        /// <returns>A string representing the normalized file item.</returns>
        public static string ConvertFileItem(string fileItem)
        {
            var result = new StringBuilder();

            foreach (var item in fileItem)
            {
                if (result.Length == 0)
                {
                    result.Append(char.ToLower(item));
                }
                else if (item == '\\')
                {
                    result.Append('/');
                }
                else if (char.IsUpper(item))
                {
                    if (result[^1] != '-' && result[^1] != '/' && result[^1] != '\\')
                    {
                        result.Append('-');
                    }
                    result.Append(char.ToLower(item));
                }
                else
                {
                    result.Append(char.ToLower(item));
                }
            }
            return result.ToString();
        }
        #endregion app methods
    }
}
