//@BaseCode
namespace TemplateTools.ConApp.Apps
{
    /// <summary>
    /// Represents a console application for navigating and changing file system paths.
    /// Provides an interactive menu for browsing directories and selecting a target path.
    /// </summary>
    internal partial class PathChangerApp : ConsoleApplication
    {
        /// <summary>
        /// Gets or sets the title displayed in the application header.
        /// </summary>
        protected string Title { get; set; }
        
        /// <summary>
        /// Gets or sets the label text for the current path display.
        /// </summary>
        protected string PathLabel { get; set; }
        
        /// <summary>
        /// Gets the function that retrieves the current path value.
        /// </summary>
        protected Func<string> GetPath { get; }
        
        /// <summary>
        /// Gets the action that sets a new path value.
        /// </summary>
        protected Action<string> SetPath { get; }
        
        /// <summary>
        /// Gets or sets an optional predicate to filter which paths are displayed in the menu.
        /// If null, all paths are shown.
        /// </summary>
        protected Predicate<string>? Predicate { get; set; }
        
        /// <summary>
        /// Gets or sets the current file system path.
        /// The getter retrieves the path via <see cref="GetPath"/>, and the setter applies it via <see cref="SetPath"/>.
        /// </summary>
        internal string Path
        {
            get => GetPath != null ? GetPath() : string.Empty;
            set
            {
                if (SetPath != null)
                {
                    SetPath(value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathChangerApp"/> class.
        /// </summary>
        /// <param name="title">The title to display in the application header.</param>
        /// <param name="pathLabel">The label for the current path display.</param>
        /// <param name="getPath">Function to retrieve the current path.</param>
        /// <param name="setPath">Action to set a new path.</param>
        /// <param name="predicate">Optional predicate to filter displayed paths.</param>
        public PathChangerApp(string title, string pathLabel, Func<string> getPath, Action<string> setPath, Predicate<string>? predicate = null)
        {
            MaxSubPathDepth = 1;

            Title = title;
            PathLabel = pathLabel;
            GetPath = getPath;
            SetPath = setPath;
            Predicate = predicate;
        }

        #region override methods
        /// <summary>
        /// Prints the application header with the title, current sub-path depth, and current path.
        /// </summary>
        protected override void PrintHeader()
        {
            List<KeyValuePair<string, object>> headerParams = [
                new("Sub path depth:", MaxSubPathDepth),
                new(PathLabel, Path)
            ];

            base.PrintHeader(Title, [.. headerParams]);
        }
        
        /// <summary>
        /// Creates an array of menu items for the application menu.
        /// Includes options to change sub-path depth, navigate to parent directory, and browse available subdirectories.
        /// </summary>
        /// <returns>An array of <see cref="MenuItem"/> objects representing the menu items.</returns>
        protected override MenuItem[] CreateMenuItems()
        {
            var mnuIdx = 0;
            var menuItems = new List<MenuItem>()
            {
                CreateMenuSeparator(),
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Depth", "Change max sub path depth"),
                    Action = (self) => ChangeMaxSubPathDepth(),
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Parent", "Change to parent"),
                    Action = (self) =>
                    {
                        var parentPath = Directory.GetParent(Path)?.FullName;

                        if (string.IsNullOrEmpty(parentPath) == false && Directory.Exists(parentPath))
                        {
                            Path = parentPath;
                        }
                    },
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Input", "Change with input"),
                    Action = (self) =>
                    {
                        Path = InputPath(Path);
                    },
                },
                CreateMenuSeparator(),
            };

            var paths = TemplatePath.GetSubPaths(Path, MaxSubPathDepth)
                                    .Where(p => p.Equals(Path) == false && (Predicate == null || Predicate(p)))
                                    .OrderBy(p => p)
                                    .ToArray();

            menuItems.AddRange(CreatePageMenuItems(ref mnuIdx, paths, (item, menuItem) =>
            {
                var subPath = item.Replace(Path, string.Empty);

                menuItem.Text = ToLabelText("Source path", $"{subPath}");
                menuItem.Tag = "path";
                menuItem.Action = (self) =>
                {
                    var sourcePath = self.Params["sourcePath"] as string;

                    if (Directory.Exists(sourcePath))
                    {
                        Path = sourcePath;
                    }
                };
                menuItem.Params = new() { { "sourcePath", item }, { "subPath", subPath } };
            }));
            return [.. menuItems.Union(CreateExitMenuItems())];
        }
        #endregion override methods

        /// <summary>
        /// Prompts the user to input a file system path, reads the console input,
        /// validates that the input refers to an existing directory, and returns
        /// the validated path. If the user input is null, empty, whitespace, or
        /// not an existing directory, the <paramref name="currentPath"/> is returned.
        /// </summary>
        /// <param name="currentPath">The current path to return if the user input is invalid.</param>
        /// <returns>
        /// The user-provided path when it exists on disk; otherwise the original <paramref name="currentPath"/>.
        /// </returns>
        private string InputPath(string currentPath)
        {
            var result = currentPath;

            PrintLine();
            Print("Input path: ");

            string? input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input) && Directory.Exists(input))
            {
                result = input;
            }
            return result;
        }
    }
}
