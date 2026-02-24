//@BaseCode
using System.Text;
using System.Text.RegularExpressions;

namespace SETemplate.ConApp
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
        #endregion methods

        #region helpers
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
