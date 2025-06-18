//@CodeCopy
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using TemplateTools.Logic.Extensions;

namespace TemplateTools.ConApp.Apps
{
    internal partial class ChatGptApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the CodeGeneratorApp class.
        /// </summary>
        static ChatGptApp()
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
        /// Initializes a new instance of the <see cref="ChatGptApp"/> class.
        /// </summary>
        public ChatGptApp()
        {
            Constructing();
            try
            {
                var configuration = CommonModules.Configuration.AppSettings.Instance;

                ChatGptUrl = configuration["ChatGPT:Url"] ?? string.Empty;
                ChatGptApiKey = configuration["ChatGPT:ApiKey"] ?? string.Empty;
                ChatGptModel = configuration["ChatGPT:Model"] ?? "gpt-4.o-mini";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(message: $"Error in {MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
            }
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
        /// Gets or sets the URL endpoint for the ChatGPT API.
        /// </summary>
        private string ChatGptUrl { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the API key used for authenticating requests to the ChatGPT API.
        /// </summary>
        private string ChatGptApiKey { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the model name to be used for ChatGPT API requests.
        /// </summary>
        private string ChatGptModel { get; set; } = "gpt-4.1";
        /// <summary>
        /// Gets or sets the path from which code will be instruction.
        /// </summary>
        private string InstructionPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the name of the file to import entities from.
        /// </summary>
        private string InstrcutionsFileName { get; set; } = "instruction_set.txt";

        /// <summary>
        /// Gets or sets the path where the task file is located.
        /// </summary>
        private string TaskPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the name of the task file to be used (e.g., readme.md).
        /// </summary>
        private string TaskFileName { get; set; } = "readme.md";

        /// <summary>
        /// Gets or sets the path from which code will be exported.
        /// </summary>
        private string OutputPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the name of the file to export entities from.
        /// </summary>
        private string OutputFileName { get; set; } = "entities.cs";
        #endregion properties

        #region overrides
        /// <summary>
        /// Creates an array of menu items for the application menu.
        /// </summary>
        /// <returns>An array of MenuItem objects representing the menu items.</returns>
        protected override MenuItem[] CreateMenuItems()
        {
            var taskFilePath = Path.Combine(TaskPath, TaskFileName);
            var taskFileExists = File.Exists(taskFilePath);
            var instructionFilePath = Path.Combine(InstructionPath, InstrcutionsFileName);
            var instructionsFileExists = File.Exists(instructionFilePath);
            var outpuFilePath = Path.Combine(OutputPath, OutputFileName);
            var outputFileExists = File.Exists(outpuFilePath);

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
                    Text = ToLabelText(nameof(ChatGptUrl).ToCamelCaseSplit(), "Change the chatgpt url"),
                    Action = (self) =>
                    {
                        var result = ReadLine($"{nameof(ChatGptUrl).ToCamelCaseSplit()}: ");

                        if (result.HasContent())
                        {
                            ChatGptUrl = result;
                        }
                    }
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText(nameof(ChatGptApiKey).ToCamelCaseSplit(), "Change the chatgpt api key"),
                    Action = (self) =>
                    {
                        var result = ReadLine($"{nameof(ChatGptApiKey).ToCamelCaseSplit()}: ");

                        if (result.HasContent())
                        {
                            ChatGptApiKey = result;
                        }
                    }
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText(nameof(ChatGptModel).ToCamelCaseSplit(), "Change the chatgpt model"),
                    Action = (self) =>
                    {
                        var result = ReadLine($"{nameof(ChatGptModel).ToCamelCaseSplit()}: ");

                        if (result.HasContent())
                        {
                            ChatGptModel = result;
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
                    Text = ToLabelText(nameof(InstructionPath).ToCamelCaseSplit(), "Change the instruction path"),
                    Action = (self) =>
                    {
                        var result = ChangePath($"{nameof(InstructionPath).ToCamelCaseSplit()}: ", InstructionPath);

                        if (result.HasContent())
                        {
                            InstructionPath = result;
                        }
                    }
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText("Instr. file name", "Change the instruction file name"),
                    Action = (self) =>
                    {
                        var result = ReadLine($"{nameof(InstrcutionsFileName).ToCamelCaseSplit()}: ");

                        if (result.HasContent())
                        {
                            InstrcutionsFileName = result;
                        }
                    }
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "edit_instructions",
                    Text = ToLabelText("Edit instructions", "Edit instructions file"),
                    Action = (self) => {
                        if (instructionsFileExists)
                            OpenTextFile(instructionFilePath);
                    },
                    ForegroundColor = instructionsFileExists ? ForegroundColor : ConsoleColor.Red,
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
                    Text = ToLabelText(nameof(TaskPath).ToCamelCaseSplit(), "Change the task path"),
                    Action = (self) =>
                    {
                        var result = ChangePath($"{nameof(TaskPath).ToCamelCaseSplit()}: ", TaskPath);

                        if (result.HasContent())
                        {
                            TaskPath = result;
                        }
                    }
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText(nameof(TaskFileName).ToCamelCaseSplit(), "Change the task file name"),
                    Action = (self) =>
                    {
                        var result = ReadLine($"{nameof(TaskFileName).ToCamelCaseSplit()}: ");

                        if (result.HasContent())
                        {
                            TaskFileName = result;
                        }
                    }
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "edit_task",
                    Text = ToLabelText("Edit task", "Edit task file"),
                    Action = (self) => {
                        if (taskFileExists)
                            OpenTextFile(taskFilePath);
                    },
                    ForegroundColor = taskFileExists ? ForegroundColor : ConsoleColor.Red,
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
                    Text = ToLabelText(nameof(OutputPath).ToCamelCaseSplit(), "Change the output path"),
                    Action = (self) =>
                    {
                        var result = ChangePath($"{nameof(OutputPath).ToCamelCaseSplit()}: ", OutputPath);

                        if (result.HasContent())
                        {
                            OutputPath = result;
                        }
                    }
                },
                new()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = ToLabelText(nameof(OutputFileName).ToCamelCaseSplit(), "Change the output file name"),
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
                    Key = $"{++mnuIdx}",
                    OptionalKey = "edit_output",
                    Text = ToLabelText("Edit output", "Edit output file"),
                    Action = (self) => {
                        if (outputFileExists)
                            OpenTextFile(outpuFilePath);
                    },
                    ForegroundColor = outputFileExists ? ForegroundColor : ConsoleColor.Red,
                },
                new()
                {
                    Key = "-----",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
            };

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
                new($"{nameof(CodeSolutionPath).ToCamelCaseSplit()}:", CodeSolutionPath),
                new(new string('-', 25), string.Empty),
                new($"{nameof(ChatGptUrl)}:", $"{ChatGptUrl}"),
                new($"{nameof(ChatGptApiKey)}:", $"{ChatGptApiKey}"),
                new($"{nameof(ChatGptModel)}:", $"{ChatGptModel}"),
                new(new string('-', 25), string.Empty),
                new($"{nameof(InstructionPath).ToCamelCaseSplit()}:", $"{InstructionPath}"),
                new($"{nameof(InstrcutionsFileName).ToCamelCaseSplit()}:", $"{InstrcutionsFileName}"),
                new($"{nameof(TaskPath).ToCamelCaseSplit()}:", $"{TaskPath}"),
                new($"{nameof(TaskFileName).ToCamelCaseSplit()}:", $"{TaskFileName}"),
                new(new string('-', 25), string.Empty),
                new($"{nameof(OutputPath).ToCamelCaseSplit()}:", $"{OutputPath}"),
                new($"{nameof(OutputFileName).ToCamelCaseSplit()}:", $"{OutputFileName}"),
                new(new string('-', 25), ""),
            ];

            base.PrintHeader("Template ChatGpt", [.. headerParams]);
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
                else if (arg.Key.Equals(nameof(InstructionPath), StringComparison.OrdinalIgnoreCase))
                {
                    InstructionPath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(ChatGptUrl), StringComparison.OrdinalIgnoreCase))
                {
                    ChatGptUrl = arg.Value;
                }
                else if (arg.Key.Equals(nameof(ChatGptApiKey), StringComparison.OrdinalIgnoreCase))
                {
                    ChatGptApiKey = arg.Value;
                }
                else if (arg.Key.Equals(nameof(ChatGptModel), StringComparison.OrdinalIgnoreCase))
                {
                    ChatGptModel = arg.Value;
                }
                else if (arg.Key.Equals(nameof(InstrcutionsFileName), StringComparison.OrdinalIgnoreCase))
                {
                    InstrcutionsFileName = arg.Value;
                }
                else if (arg.Key.Equals(nameof(TaskPath), StringComparison.OrdinalIgnoreCase))
                {
                    TaskPath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(TaskFileName), StringComparison.OrdinalIgnoreCase))
                {
                    TaskFileName = arg.Value;
                }
                else if (arg.Key.Equals(nameof(OutputPath), StringComparison.OrdinalIgnoreCase))
                {
                    OutputPath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(OutputFileName), StringComparison.OrdinalIgnoreCase))
                {
                    OutputFileName = arg.Value;
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
        private void OpenTextFile(string filePath)
        {
            var fileExists = File.Exists(filePath);

            if (!fileExists)
                throw new FileNotFoundException($"Datei nicht gefunden: {filePath}");

            ProcessStartInfo psi;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Notepad ist auf allen Windows-Systemen vorhanden
                psi = new ProcessStartInfo("notepad.exe", $"\"{filePath}\"");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // xdg-open öffnet die Datei mit dem Standard-Programm
                psi = new ProcessStartInfo("xdg-open", $"\"{filePath}\"");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // open ist das macOS-Äquivalent zu xdg-open
                psi = new ProcessStartInfo("open", $"\"{filePath}\"");
            }
            else
            {
                throw new PlatformNotSupportedException("Unbekanntes Betriebssystem");
            }

            // Bei diesen direkten Befehlen muss UseShellExecute=false sein (default),
            // da wir hier ja selbst das Programm (notepad/xdg-open/open) starten.
            Process.Start(psi);
        }

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
            Task.Run(async() => await CreateEntitiesByChatGptAsync()).Wait();
            StartProgressBar();
        }

        private async Task CreateEntitiesByChatGptAsync()
        {
            try
            {
                var instructioFilePath = Path.Combine(InstructionPath, InstrcutionsFileName);
                var instructionFileExists = File.Exists(instructioFilePath);
                var taskFilePath = Path.Combine(TaskPath, TaskFileName);
                var taskFileExists = File.Exists(taskFilePath);
                var outpuFilePath = Path.Combine(OutputPath, OutputFileName);
                var outputFileExists = File.Exists(outpuFilePath);

                var systemPrompt = instructionFileExists ? File.ReadAllText(instructioFilePath) : string.Empty;
                var userPrompt = taskFileExists ? File.ReadAllText(taskFilePath) : string.Empty;

                // Baue die API-Anfrage
                var messages = new[]
                {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            };

                var requestBody = new
                {
                    model = ChatGptModel,
                    messages,
                    temperature = 0.2
                };

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ChatGptApiKey);

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(ChatGptUrl, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseBody);

                // Extrahiere den generierten Text
                var result = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                File.WriteAllText(outpuFilePath, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadLine();
            }
        }
        #endregion app methods
    }
}
