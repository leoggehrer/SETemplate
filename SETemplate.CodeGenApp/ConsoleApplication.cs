//@BaseCode
using System.Text.RegularExpressions;

namespace SETemplate.CodeGenApp
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
        /// This method is called after the object has been initialized.
        /// </summary>
        partial void Constructed();
        #endregion Instance-Constructors

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
            return new MenuItem[2]
            {
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
            };
        }
        #endregion overrides

        #region helpers
        protected static string ReposPath { get; set; } = SourcePath;
        /// <summary>
        /// Splits a camel case or Pascal case string into separate words, separated by spaces, and converts the result to lowercase.
        /// </summary>
        /// <param name="input">The camel case or Pascal case string to split.</param>
        /// <returns>
        /// A string with spaces inserted before each uppercase letter (except the first character), converted to lowercase.
        /// If the input is null, empty, or consists only of whitespace, the original input is returned.
        /// </returns>
        public static string SplitCamelCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return Regex.Replace(input, "(?<!^)([A-Z])", " $1").ToLower();
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
        #endregion helpers
    }
}
