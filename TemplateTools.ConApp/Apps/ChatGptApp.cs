//@BaseCode
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace TemplateTools.ConApp.Apps
{
    internal abstract partial class ChatGptApp : ConsoleApplication
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
        /// Initializes a new instance of the <see cref="ChatGptEntityCreatorApp"/> class.
        /// </summary>
        public ChatGptApp()
        {
            Constructing();
            try
            {
                var configuration = CommonModules.Configuration.AppSettings.Instance;

                ChatGptUrl = configuration["OpenAI:Url"] ?? string.Empty;
                ChatGptApiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;
                ChatGptModel = configuration["OpenAI:Model"] ?? "gpt-4.o-mini";
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
            var requirementFilePath = Path.Combine(RequirementPath, RequirementFileName);
            var requirementFileExists = File.Exists(requirementFilePath);
            var instructionFilePath = Path.Combine(InstructionPath, InstrcutionsFileName);
            var instructionsFileExists = File.Exists(instructionFilePath);
            var outputFilePath = Path.Combine(OutputPath, OutputFileName);
            var outputFileExists = File.Exists(outputFilePath);

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
              Text = ToLabelText(nameof(ChatGptUrl).ToCamelCaseSplit(), $"Change the chatgpt url:     '{ChatGptUrl}'"),
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
              Text = ToLabelText(nameof(ChatGptApiKey).ToCamelCaseSplit(), $"Change the chatgpt api key: '{GetDisplayChatCptAppKey()}'"),
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
              Text = ToLabelText(nameof(ChatGptModel).ToCamelCaseSplit(), $"Change the chatgpt model:   '{ChatGptModel}'"),
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
              Text = ToLabelText("Filename", $"Change the instruction filename '{InstrcutionsFileName}'"),
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
              Text = ToLabelText("Edit instructions", $"Edit instructions file '{InstrcutionsFileName}'"),
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
              Text = ToLabelText(nameof(RequirementPath).ToCamelCaseSplit(), "Change the requirement path"),
              Action = (self) =>
              {
                  var result = ChangePath($"{nameof(RequirementPath).ToCamelCaseSplit()}: ", RequirementPath);

                  if (result.HasContent())
                  {
                      RequirementPath = result;
                  }
              }
          },
          new()
          {
              Key = (++mnuIdx).ToString(),
              Text = ToLabelText("Filename", $"Change the requirement filename '{RequirementFileName}'"),
              Action = (self) =>
              {
                  var result = ReadLine($"{nameof(RequirementFileName).ToCamelCaseSplit()}: ");

                  if (result.HasContent())
                  {
                      RequirementFileName = result;
                  }
              }
          },
          new()
          {
              Key = $"{++mnuIdx}",
              OptionalKey = "edit_requirement",
              Text = ToLabelText("Edit requirements", $"Edit requirement file '{RequirementFileName}'"),
              Action = (self) => {
                  if (requirementFileExists)
                      OpenTextFile(requirementFilePath);
              },
              ForegroundColor = requirementFileExists ? ForegroundColor : ConsoleColor.Red,
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
              Text = ToLabelText("Filename", $"Change the output filename '{OutputFileName}'"),
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
              Text = ToLabelText("Edit output", $"Edit output file '{OutputFileName}'"),
              Action = (self) => {
                  if (outputFileExists)
                      OpenTextFile(outputFilePath);
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

            return [.. menuItems];
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
                else if (arg.Key.Equals(nameof(RequirementPath), StringComparison.OrdinalIgnoreCase))
                {
                    RequirementPath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(RequirementFileName), StringComparison.OrdinalIgnoreCase))
                {
                    RequirementFileName = arg.Value;
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

        #region properties
        /// <summary>
        /// Gets or sets the path of the solution.
        /// </summary>
        protected string CodeSolutionPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the URL endpoint for the ChatGPT API.
        /// </summary>
        protected string ChatGptUrl { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the API key used for authenticating requests to the ChatGPT API.
        /// </summary>
        protected string ChatGptApiKey { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the model name to be used for ChatGPT API requests.
        /// </summary>
        protected string ChatGptModel { get; set; } = "gpt-4.1";

        /// <summary>
        /// Gets or sets the path from which code will be instruction.
        /// </summary>
        protected string InstructionPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the name of the file to import entities from.
        /// </summary>
        protected string InstrcutionsFileName { get; set; } = "instructions.txt";

        /// <summary>
        /// Gets or sets the path where the task file is located.
        /// </summary>
        protected string RequirementPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the name of the task file to be used (e.g., readme.md).
        /// </summary>
        protected string RequirementFileName { get; set; } = "README.md";

        /// <summary>
        /// Gets or sets the path from which code will be exported.
        /// </summary>
        protected string OutputPath { get; set; } = SolutionPath;
        /// <summary>
        /// Gets or sets the name of the file to export entities from.
        /// </summary>
        protected string OutputFileName { get; set; } = "output.cs";
        /// <summary>
        /// Determines whether a file can be written to, based on its existence and the 'Force' flag.
        /// If the file does not exist, returns true.
        /// If the file exists and 'Force' is true, checks if the first line contains the ChatGpt code label;
        /// if so, returns true, otherwise false.
        /// </summary>
        /// <param name="filePath">The path of the file to check.</param>
        /// <returns>True if the file can be written to; otherwise, false.</returns>
        protected bool CanWriteFile(string filePath)
        {
            var result = File.Exists(filePath) == false;

            if (result == false && Force)
            {
                var lines = File.ReadAllLines(filePath, Encoding.Default);

                if (lines.Length > 0 && lines.First().Contains(Common.StaticLiterals.AiCodeLabel))
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// Writes the specified text to a file at the given file path using UTF-8 encoding without BOM.
        /// If the directory does not exist, it is created.
        /// </summary>
        /// <param name="filePath">The full path of the file to write to.</param>
        /// <param name="text">The text content to write to the file.</param>
        protected static void WriteAllTextToFile(string filePath, string text)
        {
            var path = Path.GetDirectoryName(filePath);

            if (string.IsNullOrEmpty(path) == false)
            {
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                var utf8OhneBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

                File.WriteAllText(filePath, text, utf8OhneBOM);
            }
        }
        /// <summary>
        /// Returns a masked version of the ChatGPT API key for display purposes.
        /// If the API key is at least 6 characters long, only the first 3 and last 3 characters are shown, with '...' in between.
        /// Otherwise, the full API key is returned.
        /// </summary>
        /// <returns>A masked string representation of the ChatGPT API key.</returns>
        protected string GetDisplayChatCptAppKey()
        {
            var result = ChatGptApiKey;

            if (ChatGptApiKey.Length >= 6)
            {
                result = ChatGptApiKey[..3] + "..." + ChatGptApiKey[^3..];
            }
            return result;
        }
        /// <summary>
        /// Sends a prompt to the ChatGPT API using the configured model, system prompt, and user prompt.
        /// Reads the system prompt from the instructions file and the user prompt from the task file.
        /// The response from the API is written to the output file.
        /// </summary>
        /// <remarks>
        /// This method handles file existence checks, builds the API request, sends it, and processes the response.
        /// Any exceptions encountered are written to the console.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task LetChatGptDoItsWork()
        {
            try
            {
                var instructioFilePath = Path.Combine(InstructionPath, InstrcutionsFileName);
                var instructionFileExists = File.Exists(instructioFilePath);
                var taskFilePath = Path.Combine(RequirementPath, RequirementFileName);
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

                var httpClient = new HttpClient() { Timeout = new TimeSpan(0, 5, 0) }; // Set a timeout of 5 minutes
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
