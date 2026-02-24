//@BaseCode
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using TemplateTools.ConApp.Modules;

namespace TemplateTools.ConApp.Apps
{
    internal partial class ToolsRestServerApp : ConsoleApplication
    {
        #region fields
        private static HttpListener? _listener;
        private static CancellationTokenSource? _cts;
        private readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        #endregion fields

        #region Class-Constructors
        /// <summary>
        /// Initializes the CodeGeneratorApp class.
        /// </summary>
        static ToolsRestServerApp()
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
        public ToolsRestServerApp()
        {
            Constructing();
            try
            {
                var configuration = CommonModules.Configuration.AppSettings.Instance;

                UrlPrefix = configuration["RestServer:Url"] ?? string.Empty;
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
                  Text = ToLabelText(nameof(UrlPrefix).ToCamelCaseSplit(), $"Change the listening url:     '{UrlPrefix}'"),
                  Action = (self) =>
                  {
                      if (IsRunning == false)
                      {
                          var result = ReadLine($"{nameof(UrlPrefix).ToCamelCaseSplit()}: ");

                          if (result.HasContent())
                          {
                              UrlPrefix = result;
                          }
                      }
                  },
                  ForegroundColor = IsRunning ? ConsoleColor.Red : ForegroundColor,
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
                  OptionalKey = "start",
                  Text = ToLabelText("Start", $"Starts the server..."),
                  Action = (self) =>
                  {
                      if (IsRunning == false)
                      {
                          Start();
                      }
                  },
                  ForegroundColor = IsRunning ? ConsoleColor.Red : ForegroundColor,
              },
              new()
              {
                  Key = (++mnuIdx).ToString(),
                  OptionalKey = "start",
                  Text = ToLabelText("Stop", $"Stopped the server..."),
                  Action = (self) =>
                  {
                      if (IsRunning)
                      {
                          Stop();
                      }
                  },
                  ForegroundColor = IsRunning == false ? ConsoleColor.Red : ForegroundColor,
              },
            };

            return [.. menuItems.Union(CreateExitMenuItems())];
        }
        /// <summary>
        /// Prints the header for the application.
        /// </summary>
        protected override void PrintHeader()
        {
            List<KeyValuePair<string, object>> headerParams =
            [
                new("Override files:", $"{Force}"),
                new($"{nameof(CodeSolutionPath).ToCamelCaseSplit()}:", CodeSolutionPath),
                new(new string('-', 25), string.Empty),
                new($"The server is running:", $"{IsRunning}"),
                new(new string('-', 25), string.Empty),
            ];

            base.PrintHeader("Template Tools Rest-Server", [.. headerParams]);
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
                else if (arg.Key.Equals(nameof(UrlPrefix), StringComparison.OrdinalIgnoreCase))
                {
                    UrlPrefix = arg.Value;
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
        protected string UrlPrefix { get; set; } = string.Empty;
        protected static bool IsRunning => _listener?.IsListening ?? false;
        #endregion properties

        /// <summary>
        /// Startet den Server asynchron im Hintergrund.
        /// </summary>
        protected void Start()
        {
            if (IsRunning == false)
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add(UrlPrefix);
                _listener.Start();
                _cts = new CancellationTokenSource();
                Task.Run(() => ListenLoop(_cts.Token));
            }
        }

        /// <summary>
        /// Stoppt den Server.
        /// </summary>
        protected static void Stop()
        {
            if (IsRunning)
            {
                _cts?.Cancel();
                _listener?.Stop();
                _listener?.Close();
                _listener = null;
            }
        }

        /// <summary>
        /// Listens for incoming HTTP requests in a loop until cancellation is requested.
        /// For each request, it starts a new task to handle the request asynchronously.
        /// Handles shutdown and logs any request errors.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        private async Task ListenLoop(CancellationToken cancellationToken)
        {
            while (_listener != null && cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    var context = await _listener.GetContextAsync();

                    _ = Task.Run(() => HandleRequest(context), cancellationToken);
                }
                catch (HttpListenerException) when (cancellationToken.IsCancellationRequested)
                {
                    break; // Shutdown
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Handles an incoming HTTP request, processes it based on the request method and URL, and writes an appropriate JSON response.
        /// Supports CORS headers, preflight OPTIONS requests, a GET endpoint for a hello message, and a POST endpoint for executing commands.
        /// </summary>
        /// <param name="context">The HTTP listener context containing the request and response objects.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            // CORS Header setzen
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

            // Preflight-Request (OPTIONS)
            if (request.HttpMethod == "OPTIONS")
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Close();
                return;
            }

            try
            {
                if (request.HttpMethod == "GET" && request.Url?.AbsolutePath == "/api/hello")
                {
                    string json = "{\"message\":\"Hello from RestServer class!\"}";
                    await WriteJsonResponse(response, json);
                }
                else if (request.HttpMethod == "POST" && request.Url?.AbsolutePath == "/api/execute")
                {
                    using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    var requestBody = await reader.ReadToEndAsync();
                    var json = $"{{\"received\":{requestBody}}}";
                    var model = JsonSerializer.Deserialize<Models.McpRequest>(requestBody, options);

                    if (model != null && model.Payload != null && model.Payload.Data != null)
                    {
                        ExecuteRequest(model);
                    }
                    await WriteJsonResponse(response, json);
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling request: {ex.Message}");
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Close();
            }
        }

        /// <summary>
        /// Writes a JSON response to the specified <see cref="HttpListenerResponse"/>.
        /// Sets the content type, encoding, and content length, writes the JSON data to the response stream, and closes the response.
        /// </summary>
        /// <param name="response">The <see cref="HttpListenerResponse"/> to write to.</param>
        /// <param name="json">The JSON string to write as the response body.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private static async Task WriteJsonResponse(HttpListenerResponse response, string json)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;

            await response.OutputStream.WriteAsync(buffer);
            response.Close();
        }

        /// <summary>
        /// Executes a request received by the MCP REST server.
        /// Parses the target parameters from the request, and if the command is "create_entities" and valid payload data is present,
        /// invokes the entity creation logic using the current solution path and force option.
        /// </summary>
        /// <param name="mcpRequest">The <see cref="Models.McpRequest"/> containing the command and payload data.</param>
        private void ExecuteRequest(Models.McpRequest mcpRequest)
        {
            if (mcpRequest != null && mcpRequest.TargetParams != null && mcpRequest.Payload != null)
            {
                var commands = ConvertArgs([mcpRequest.TargetParams]);

                foreach (var item in commands)
                {
                    if (item.Value.Equals("import_cs_items") && mcpRequest.Payload.Data != null)
                    {
                        CodeImporter.ImportCSharpItems(CodeSolutionPath, mcpRequest.Payload.Data, Force);
                    }
                    else if (item.Value.Equals("import_csv_data") && mcpRequest.Payload.Data != null)
                    {
                        CodeImporter.ImportCsvData(CodeSolutionPath, mcpRequest.Payload.Data);
                    }
                    else if (item.Value.Equals("import_html_forms") && mcpRequest.Payload.Data != null)
                    {
                        CodeImporter.ImportHtmlForms(CodeSolutionPath, mcpRequest.Payload.Data, Force);
                    }
                    else if (item.Value.Equals("import_file_data") && mcpRequest.Payload.Data != null)
                    {
                        CodeImporter.ImportFileData(CodeSolutionPath, mcpRequest.Payload.Data);
                    }
                    else if (item.Value.Equals("create_html_preview") && mcpRequest.Payload.Data != null)
                    {
                        CodeCreator.CreateAndOpenHtmlPreview(mcpRequest.Payload.Data);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Unknown command value: {item.Value}");
                        System.Diagnostics.Debug.WriteLine($"Unknown command data:  {JsonSerializer.Serialize(mcpRequest)}");
                    }
                }
            }
        }
    }
}
