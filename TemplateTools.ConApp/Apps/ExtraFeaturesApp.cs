//@BaseCode

using TemplateTools.Logic;
using TemplateTools.Logic.Git;

namespace TemplateTools.ConApp.Apps
{
    /// <summary>
    /// Represents an application for copying template solutions to a target solution.
    /// </summary>
    public partial class ExtraFeaturesApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// This is the static constructor for the CopierApp class.
        /// </summary>
        /// <remarks>
        /// This constructor is responsible for initializing the static members of the CopierApp class.
        /// </remarks>
        static ExtraFeaturesApp()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called when the class is being constructed.
        /// </summary>
        /// <remarks>
        /// This is a partial method and must be implemented in a partial class.
        /// </remarks>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the application arguments that are passed to sub-applications.
        /// </summary>
        private string[] AppArgs { get; set; } = [];
        private bool _executeWithTokenSource = false;
        private static string _webApiPath = string.Empty;
        private static string _webApiDirectory = string.Empty;
        // Use CancellationTokenSource so we can request cancellation.
        private static CancellationTokenSource? _webApiCancellationSource = null;
        #endregion Properties

        #region overrides
        /// <summary>
        /// Prints the header for the application.
        /// </summary>
        /// <param name="sourcePath">The path of the solution.</param>
        protected override void PrintHeader()
        {
            List<KeyValuePair<string, object>> headerParams = [new("Solution path:", SolutionPath)];

            base.PrintHeader("Template Extra Features", [.. headerParams]);
        }
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
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Path", "Change solution path"),
                    Action = (self) =>
                    {
                        var previousPath = SolutionPath;
                        var pathChangerApp = new PathChangerApp(
                            "Template Tools",
                            "Source solution path",
                            () => SolutionPath,
                            (value) => SolutionPath = value);

                        pathChangerApp.Run([]);

                        if (string.IsNullOrEmpty(SolutionPath) || Directory.GetFiles(SolutionPath, "*.sln").Length == 0)
                        {
                            PrintLine();
                            PrintErrorLine("The selected solution path is invalid or does not exist.");
                            SolutionPath = previousPath;
                            Thread.Sleep(3000);
                        }
                    },
                },
            };

            var solutionName = TemplatePath.GetSolutionName(SolutionPath);

            if (Directory.Exists(SolutionPath))
            {
                menuItems.Add(new()
                {
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                });
                menuItems.Add(new MenuItem()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText($"Open {solutionName}", $"Open the solution folder ({solutionName}) with VSCode"),
                    Params = new() { { "path", SolutionPath } },
                    Action = (self) =>
                    {
                        var path = self.Params["path"]?.ToString() ?? string.Empty;
                        // start the Angular app with VSCode on Mac or Windows
                        OpenWithVScode(path);
                    },
                });
            }

            var angularAppProjectPath = Path.Combine(SolutionPath, $"{solutionName}.AngularApp");

            if (Directory.Exists(angularAppProjectPath))
            {
                menuItems.Add(new()
                {
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                });
                menuItems.Add(new MenuItem()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Open AngularApp", $"Open the AngularApp project folder ({solutionName}.AngularApp) with VSCode"),
                    Params = new() { { "path", angularAppProjectPath } },
                    Action = (self) =>
                    {
                        var path = self.Params["path"]?.ToString() ?? string.Empty;
                        // start the Angular app with VSCode on Mac or Windows
                        OpenWithVScode(path);
                    },
                });
            }

            var conAppProjectPath = Path.Combine(SolutionPath, $"{solutionName}.ConApp");

            if (Directory.Exists(conAppProjectPath))
            {
                menuItems.Add(new()
                {
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                });
                menuItems.Add(new MenuItem()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Execute ConApp", $"Execute the ConApp project folder ({solutionName}.ConApp)"),
                    Params = new() { { "path", conAppProjectPath } },
                    Action = (self) =>
                    {
                        var path = self.Params["path"]?.ToString() ?? string.Empty;
                        // execute the ConApp
                        ExecuteDotnetApp(path);
                    },
                });
            }

            var webApiAppProjectPath = Path.Combine(SolutionPath, $"{solutionName}.WebApi");

            if (Directory.Exists(webApiAppProjectPath))
            {
                menuItems.Add(new()
                {
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                });
                menuItems.Add(new MenuItem()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Execute WebApi", $"Execute the WebApi project folder ({solutionName}.WebApi)"),
                    Params = new() { { "path", webApiAppProjectPath } },
                    Action = (self) =>
                    {
                        var path = self.Params["path"]?.ToString() ?? string.Empty;
                        // execute the WebApi
                        ExecuteDotnetApp(path);
                    },
                });
            }

            if (_executeWithTokenSource && _webApiCancellationSource != null)
            {
                // A WebApi run is active -> show Terminate option.
                menuItems.Add(new MenuItem()
                {
                    Key = $"{++mnuIdx}",
                    ForegroundColor = ConsoleColor.Red,
                    Text = ToLabelText("Terminate WebApi", $"Terminate the WebApi project folder ({_webApiDirectory})"),
                    Action = (self) =>
                    {
                        try
                        {
                            if (_webApiCancellationSource != null && !_webApiCancellationSource.IsCancellationRequested)
                            {
                                PrintLine("Requesting WebApi termination...");
                                _webApiCancellationSource.Cancel();
                            }
                            else
                            {
                                PrintLine("WebApi is not running or cancellation already requested.");
                            }
                        }
                        catch (Exception ex)
                        {
                            PrintErrorLine($"Error while requesting termination: {ex.Message}");
                        }
                    },
                });
            }
            else if (_executeWithTokenSource)
            {
                var webApiProjectPath = Path.Combine(SolutionPath, $"{solutionName}.WebApi");

                if (Directory.Exists(webApiProjectPath))
                {
                    _webApiPath = webApiProjectPath;
                    _webApiDirectory = $"{solutionName}.WebApi";
                    menuItems.Add(new MenuItem()
                    {
                        Key = $"{++mnuIdx}",
                        ForegroundColor = ConsoleColor.Green,
                        Text = ToLabelText("Execute WebApi", $"Execute the WebApi project folder ({_webApiDirectory})"),
                        Action = (self) =>
                        {
                            try
                            {
                                PrintHeader();
                                // create a cancellation source that can be used to stop the child process
                                _webApiCancellationSource = new CancellationTokenSource();

                                // start the process asynchronously; on completion dispose and clear the source
                                var task = ExecuteDotnetAppAsync(_webApiPath, _webApiCancellationSource.Token)
                                            .ContinueWith(t =>
                                            {
                                                try
                                                {
                                                    if (t.IsFaulted)
                                                    {
                                                        PrintErrorLine($"WebApi execution failed: {t.Exception?.GetBaseException().Message}");
                                                    }
                                                    else
                                                    {
                                                        PrintLine($"WebApi finished with exit code: {t.Result}");
                                                    }
                                                }
                                                finally
                                                {
                                                    try
                                                    {
                                                        _webApiCancellationSource?.Dispose();
                                                    }
                                                    catch { /* ignore */ }
                                                    finally
                                                    {
                                                        _webApiCancellationSource = null;
                                                    }
                                                }
                                            });
                                Task.Delay(10_000).Wait(); // give some time to start
                                PrintLine();
                                PrintLine("WebApi started.");
                                PrintLine("Press any key to return to menu (WebApi will continue running in background)...");
                                Console.ReadKey(intercept: true);
                            }
                            catch (Exception ex)
                            {
                                PrintErrorLine($"Error while starting WebApi: {ex.Message}");
                                try
                                {
                                    _webApiCancellationSource?.Dispose();
                                }
                                catch { }
                                finally
                                {
                                    _webApiCancellationSource = null;
                                }
                            }
                        },
                    });
                }
            }

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
                if (arg.Key.Equals(nameof(Force), StringComparison.OrdinalIgnoreCase))
                {
                    if (bool.TryParse(arg.Value, out bool result))
                    {
                        Force = result;
                    }
                }
                if (arg.Key.Equals(nameof(SolutionPath), StringComparison.OrdinalIgnoreCase))
                {
                    SolutionPath = arg.Value;
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
            AppArgs = [.. appArgs];
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
            Generator.DeleteGeneratedFiles(SolutionPath);
            PrintLine("Delete all empty folders...");
            Generator.CleanDirectories(SolutionPath);
            PrintLine("Delete all generated files ignored from git...");
            GitIgnoreManager.DeleteIgnoreEntries(SolutionPath);
            StopProgressBar();
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
            DeleteChatGptFiles(SolutionPath);
            StartProgressBar();
        }
        /// <summary>
        /// Opens the specified project path in Visual Studio Code.
        /// </summary>
        /// <param name="projectPath">The path to the project to open in Visual Studio Code.</param>
        private void OpenWithVScode(string projectPath)
        {
            try
            {
                // start the Angular app with VSCode on Mac or Windows
                var startInfo = new System.Diagnostics.ProcessStartInfo();

                // Detect operating system and configure accordingly
                if (OperatingSystem.IsMacOS())
                {
                    // On macOS, use 'open' command with VS Code
                    startInfo.FileName = "open";
                    startInfo.Arguments = $"-a \"Visual Studio Code\" \"{projectPath}\"";
                }
                else if (OperatingSystem.IsWindows())
                {
                    // On Windows, try 'code' command
                    startInfo.FileName = "code";
                    startInfo.Arguments = $"\"{projectPath}\"";
                }
                else
                {
                    // On Linux, use 'code' command
                    startInfo.FileName = "code";
                    startInfo.Arguments = $"\"{projectPath}\"";
                }

                startInfo.WorkingDirectory = projectPath;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                var process = System.Diagnostics.Process.Start(startInfo);
                if (process == null)
                {
                    PrintErrorLine("Failed to start VS Code. Make sure 'code' command is installed in PATH.");
                    PrintLine("To install: Open VS Code → Command Palette (Cmd+Shift+P) → 'Shell Command: Install 'code' command in PATH'");
                }
                else
                {
                    process.WaitForExit();
                    process.Dispose();
                }
            }
            catch (Exception ex)
            {
                PrintErrorLine($"Error while starting project: {ex.Message}");
                PrintLine("Make sure Visual Studio Code is installed and 'code' command is available in PATH.");
            }
        }

        /// <summary>
        /// Executes a .NET application in a new console/terminal window.
        /// </summary>
        /// <param name="appPath">The path to the .NET application (project folder, .csproj, .dll, or .exe).</param>
        protected void ExecuteDotnetApp(string appPath)
        {
            if (string.IsNullOrWhiteSpace(appPath))
            {
                PrintErrorLine("Invalid path provided for dotnet execution.");
                return;
            }

            try
            {
                string dotnetCommand = "dotnet";
                string dotnetArguments = string.Empty;
                string workingDir = Environment.CurrentDirectory;

                // Determine the dotnet command arguments
                if (Directory.Exists(appPath))
                {
                    var csproj = Directory.GetFiles(appPath, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
                    workingDir = appPath;
                    dotnetArguments = !string.IsNullOrEmpty(csproj) ? $"run --project \"{csproj}\"" : "run";
                }
                else if (File.Exists(appPath))
                {
                    var ext = Path.GetExtension(appPath).ToLowerInvariant();
                    workingDir = Path.GetDirectoryName(appPath) ?? workingDir;

                    if (ext == ".dll")
                    {
                        dotnetArguments = $"\"{appPath}\"";
                    }
                    else if (ext == ".exe")
                    {
                        dotnetCommand = appPath;
                        dotnetArguments = string.Empty;
                    }
                    else // .csproj or unknown
                    {
                        dotnetArguments = $"run --project \"{appPath}\"";
                    }
                }
                else
                {
                    dotnetArguments = $"run --project \"{appPath}\"";
                }

                var startInfo = new System.Diagnostics.ProcessStartInfo();

                // Detect operating system and configure terminal accordingly
                if (OperatingSystem.IsMacOS())
                {
                    // On macOS, use Terminal.app with proper escaping
                    startInfo.FileName = "/bin/bash";
                    startInfo.Arguments = $"-c \"osascript -e 'tell application \\\"Terminal\\\" to do script \\\"cd \\\\\\\"{workingDir}\\\\\\\" && {dotnetCommand} {dotnetArguments.Replace("\"", "\\\\\\\"")}\\\"'\"";
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = false;
                }
                else if (OperatingSystem.IsWindows())
                {
                    // On Windows, use cmd.exe with start command
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = $"/c start \"Dotnet App\" cmd /k \"cd /d \"{workingDir}\" && {dotnetCommand} {dotnetArguments}\"";
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = false;
                }
                else
                {
                    // On Linux, try gnome-terminal or xterm
                    startInfo.FileName = "/bin/bash";
                    startInfo.Arguments = $"-c \"gnome-terminal -- bash -c 'cd \\\"{workingDir}\\\" && {dotnetCommand} {dotnetArguments}; exec bash'\"";
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = false;
                }

                PrintLine($"Starting dotnet app in new terminal: {appPath}");
                PrintLine($"Command: {startInfo.FileName} {startInfo.Arguments}");

                var process = System.Diagnostics.Process.Start(startInfo);

                if (process == null)
                {
                    PrintErrorLine("Failed to start process in new terminal.");
                }
                else
                {
                    PrintLine("Process started successfully.");
                }
            }
            catch (Exception ex)
            {
                PrintErrorLine($"Error while starting dotnet app in terminal: {ex.Message}");
                PrintErrorLine($"Stack trace: {ex.StackTrace}");
            }
        }
        /// <summary>
        /// Executes a .NET application located at the specified path.
        /// </summary>  
        /// <param name="appPath">The path to the .NET application (project folder, .csproj, .dll, or .exe).</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param
        /// <returns>The exit code of the executed application.</returns>
        protected async Task<int> ExecuteDotnetAppAsync(string appPath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(appPath))
            {
                PrintErrorLine("Invalid path provided for dotnet execution.");
                return -1;
            }

            try
            {
                string fileName = "dotnet";
                string arguments = string.Empty;
                string workingDir = Environment.CurrentDirectory;

                if (Directory.Exists(appPath))
                {
                    var csproj = Directory.GetFiles(appPath, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
                    workingDir = appPath;
                    arguments = !string.IsNullOrEmpty(csproj) ? $"run --project \"{csproj}\"" : "run";
                }
                else if (File.Exists(appPath))
                {
                    var ext = Path.GetExtension(appPath).ToLowerInvariant();
                    workingDir = Path.GetDirectoryName(appPath) ?? workingDir;

                    if (ext == ".dll")
                    {
                        arguments = $"\"{appPath}\"";
                    }
                    else if (ext == ".exe")
                    {
                        fileName = appPath;
                    }
                    else // .csproj or unknown
                    {
                        arguments = $"run --project \"{appPath}\"";
                    }
                }
                else
                {
                    arguments = $"run --project \"{appPath}\"";
                }

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    WorkingDirectory = workingDir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using var process = new System.Diagnostics.Process
                {
                    StartInfo = startInfo,
                    EnableRaisingEvents = true
                };

                process.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) PrintLine(e.Data); };
                process.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) PrintErrorLine(e.Data); };

                PrintLine($"Starting: {startInfo.FileName} {startInfo.Arguments}");
                if (!process.Start())
                {
                    PrintErrorLine("Failed to start process.");
                    return -1;
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                using (cancellationToken.Register(() =>
                {
                    try
                    {
                        if (!process.HasExited)
                        {
                            PrintLine("Cancellation requested � killing child process...");
                            process.Kill(entireProcessTree: true);
                        }
                    }
                    catch { /* ignore */ }
                }))
                {
                    try
                    {
                        await process.WaitForExitAsync(cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // ensure it's killed
                        if (!process.HasExited)
                        {
                            try { process.Kill(entireProcessTree: true); } catch { }
                            await process.WaitForExitAsync();
                        }
                        PrintLine("Execution cancelled.");
                        return -1;
                    }
                }

                PrintLine($"Process exited with code: {process.ExitCode}");
                return process.ExitCode;
            }
            catch (Exception ex)
            {
                PrintErrorLine($"Error while executing dotnet app: {ex.Message}");
                return -1;
            }
        }
        #endregion app methods
    }
}

