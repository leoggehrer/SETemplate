//@BaseCode
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using TemplateTools.Logic;

namespace TemplateTools.ConApp
{
    public abstract partial class ConsoleApplication : CommonTool.ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="ConsoleApplication"/> class.
        /// </summary>
        /// <remarks>
        /// This static constructor sets up the necessary properties for the program.
        /// </remarks>
        static ConsoleApplication()
        {
            ClassConstructing();
            var reposPath = Path.Combine(SourcePath, "repos");
            if (Directory.Exists(reposPath))
            {
                ReposPath = reposPath;
            }
            if (string.IsNullOrEmpty(SolutionPath))
            {
                SolutionPath = TemplatePath.GetSolutionPathByExecution();
            }
            ClassConstructed();
        }
        /// <summary>
        /// This method is called during the construction of the class.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// Represents a method that is called when a class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region Instance-Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleApplication"/> class.
        /// </summary>
        public ConsoleApplication()
        {
            Constructing();
            MaxSubPathDepth = 1;
            Constructed();
        }
        /// <summary>
        /// This method is called during the construction of the object.
        /// </summary>
        partial void Constructing();
        /// <summary>
        /// This method is called when the object is constructed.
        /// </summary>
        partial void Constructed();
        #endregion Instance-Constructors

        #region properties
        /// <summary>
        /// Gets or sets the path to the repositories directory.
        /// Defaults to <see cref="SourcePath"/> if no repos directory is found.
        /// </summary>
        protected static string ReposPath { get; set; } = SourcePath;
        #endregion properties

        #region overrides
        /// <summary>
        /// Creates the menu items for exiting the application.
        /// </summary>
        /// <returns>
        /// An array of <see cref="MenuItem"/> objects, including a separator and an exit option.
        /// Selecting the exit option will set <c>RunApp</c> to <c>false</c>, terminating the application loop.
        /// </returns>
        protected override MenuItem[] CreateExitMenuItems()
        {
            return
            [
              CreateMenuSeparator(),
              new MenuItem
              {
                Key = "x|X",
                OptionalKey = "exit",
                Text = ToLabelText("Exit", "Exits the application"),
                Action = delegate
                {
                  RunApp = false;
                }
              }
            ];
        }
        #endregion overrides

        #region methods
        /// <summary>
        /// Prints an error message to the console in red color.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        protected void PrintErrorLine(string message)
        {
            var saveForegroundColor = ForegroundColor;

            ForegroundColor = ConsoleColor.Red;
            PrintLine(message);
            ForegroundColor = saveForegroundColor;
        }
        /// <summary>
        /// Retrieves a collection of source code files from a given directory path.
        /// </summary>
        /// <param name="path">The root directory path where the search will begin.</param>
        /// <param name="searchPattern">The search pattern used to filter the files.</param>
        /// <returns>A collection of file paths that match the search pattern and contain the specified label.</returns>
        public static List<string> GetFilesByExtension(string path, string searchPattern)
        {
            var result = new List<string>();
            var files = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories)
                                 .Where(f => CommonStaticLiterals.GenerationIgnoreFolders.Any(e => f.Contains(e)) == false)
                                 .OrderBy(i => i);

            result.AddRange(files);
            return result;
        }
        /// <summary>
        /// Converts an array of command-line arguments into an array of key-value pairs.
        /// </summary>
        /// <param name="args">The array of command-line arguments, typically in the form "key=value".</param>
        /// <returns>
        /// An array of <see cref="KeyValuePair{TKey, TValue}"/> where each key is the argument name and each value is the argument value.
        /// Only arguments containing an '=' character are included.
        /// </returns>
        public static KeyValuePair<string, string>[] ConvertArgs(string[] args)
        {
            var argsLine = string.Join(" ", args);

            return [.. argsLine.Split(' ')
                .Select(pair => pair.Split('='))
                .Where(arr => arr.Length == 2)
                .Select(arr => new KeyValuePair<string, string>(arr[0], arr[1]))];
        }
        #endregion methods

        #region helpers
        /// <summary>
        /// Creates the parent namespace string from the given full namespace, stopping at the specified item.
        /// Splits the full namespace by '.', and collects each item until <paramref name="toItem"/> is found.
        /// Each item is cleaned of non-letter/digit characters.
        /// </summary>
        /// <param name="fullNamespace">The full namespace string to process.</param>
        /// <param name="toItem">The namespace item at which to stop collecting parent namespace items.</param>
        /// <returns>The parent namespace string up to (but not including) <paramref name="toItem"/>.</returns>
        public static string CreateParentNamespace(string fullNamespace, string toItem)
        {
            var start = true;
            var result = new List<string>();
            var items = fullNamespace.Replace("namespace", string.Empty)
                                     .Split('.', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(ni => ClearNamespaceItem(ni));

            foreach (var item in items)
            {
                if (start && item == toItem)
                {
                    start = false;
                }
                if (start)
                {
                    result.Add(item);
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
        public static string CreateSubNamespace(string fullNamespace, string startItem)
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
        public static string ClearNamespaceItem(string item)
        {
            return string.Concat(item.Where(char.IsLetterOrDigit));
        }

        /// <summary>
        /// Deletes all files in the specified source path that are identified as ChatGPT-generated code files.
        /// The method searches for files matching the source file extensions defined in <see cref="StaticLiterals.SourceFileExtensions"/>,
        /// and containing the <see cref="Common.StaticLiterals.AiCodeLabel"/> in their first line.
        /// For each file found, if it belongs to a template project and the configuration allows deletion, the file is deleted.
        /// PlantUML files with the extension defined in <see cref="StaticLiterals.PlantUmlFileExtension"/> are also deleted.
        /// </summary>
        /// <param name="sourcePath">The root directory to search for ChatGPT-generated files to delete.</param>
        public static void DeleteChatGptFiles(string sourcePath)
        {
            var solutionProperties = SolutionProperties.Create(sourcePath);
            var configuration = new Configuration(solutionProperties);

            foreach (var searchPattern in StaticLiterals.SourceFileExtensions.Split("|"))
            {
                var deleteFiles = GetGeneratedFiles(sourcePath, searchPattern, [Common.StaticLiterals.AiCodeLabel]);

                foreach (var file in deleteFiles)
                {
                    var isTemplateProjectFile = solutionProperties.IsTemplateProjectFile(file);

                    if (isTemplateProjectFile)
                    {
                        var projectName = solutionProperties.GetProjectNameFromFile(file); ;
                        var defaultValue = configuration.QuerySettingValue(projectName, StaticLiterals.AllItems, StaticLiterals.AllItems, "Delete", true.ToString());
                        var canDelete = configuration.QuerySettingValue<bool>(projectName, "File", Path.GetFileName(file), "Delete", defaultValue);

                        if (canDelete)
                        {
                            File.Delete(file);
                        }
                    }
                    else if (Path.GetExtension(file).Equals(StaticLiterals.PlantUmlFileExtension, StringComparison.CurrentCultureIgnoreCase))
                    {
                        File.Delete(file);
                    }
                }
            }
        }
        /// <summary>
        /// Retrieves a collection of generated files located in the specified path that match the given search pattern, excluding files in ignored folders.
        /// </summary>
        /// <param name="path">The root directory path to search for generated files.</param>
        /// <param name="searchPattern">The search pattern to match against the file names.</param>
        /// <param name="labels">An array of labels used to filter the files based on their contents.</param>
        /// <returns>A IEnumerable of string representing the paths of the generated files.</returns>
        private static List<string> GetGeneratedFiles(string path, string searchPattern, string[] labels)
        {
            var result = new List<string>();

            foreach (var file in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories)
                                          .Where(f => CommonStaticLiterals.GenerationIgnoreFolders.Any(e => f.Contains(e)) == false))
            {
                var lines = File.ReadAllLines(file, Encoding.Default);

                if (lines.Length > 0 && labels.Any(l => lines.First().Contains(l)))
                {
                    result.Add(file);
                }
            }
            return result;
        }

        /// <summary>
        /// Opens a text file in the default text editor for the current operating system.
        /// </summary>
        /// <param name="filePath">The full path to the text file to open.</param>
        /// <exception cref="FileNotFoundException">Thrown if the specified file does not exist.</exception>
        /// <exception cref="PlatformNotSupportedException">Thrown if the operating system is not supported.</exception>
        protected static void OpenTextFile(string filePath)
        {
            var fileExists = File.Exists(filePath);

            if (fileExists == false)
                throw new FileNotFoundException($"Datei nicht gefunden: {filePath}");

            ProcessStartInfo psi;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Notepad is available on all Windows systems
                psi = new ProcessStartInfo("notepad.exe", $"\"{filePath}\"");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // xdg-open opens the file with the default program
                psi = new ProcessStartInfo("xdg-open", $"\"{filePath}\"");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // open is the macOS equivalent to xdg-open
                psi = new ProcessStartInfo("open", $"\"{filePath}\"");
            }
            else
            {
                throw new PlatformNotSupportedException("Unknown operating system");
            }

            // For these direct commands, UseShellExecute=false (default),
            // since we are starting the program (notepad/xdg-open/open) ourselves.
            Process.Start(psi);
        }
        /// <summary>
        /// Opens a URL in the default web browser for the current operating system.
        /// </summary>
        /// <param name="url">The URL to open in the browser.</param>
        /// <exception cref="PlatformNotSupportedException">Thrown if the operating system is not supported and no fallback method works.</exception>
        protected static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    Process.Start("xdg-open", url);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    Process.Start("open", url);
                else
                    throw;
            }
        }
        #endregion helpers
    }
}
