//@BaseCode
namespace TemplateTools.ConApp
{
    internal partial class Program
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="Program"/> class.
        /// </summary>
        /// <remarks>
        /// This static constructor sets up the necessary properties for the program.
        /// </remarks>
        static Program()
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

        static void Main(string[] args)
        {
            var app = new Apps.ToolsApp();
            var appArgs = new List<string>();

            foreach (var arg in args)
            {
                AddAppArg(arg, appArgs);
            }
            app.Run([.. appArgs]);
        }
        static void AddAppArg(string arg, List<string> appArgs)
        {
            if (arg.HasContent() && arg.StartsWith('#') == false)
            {
                if (arg.StartsWith("commandfile=", StringComparison.CurrentCultureIgnoreCase))
                {
                    var argParts = arg.Split('=');

                    if (argParts.Length > 1 && File.Exists(argParts[1]))
                    {
                        foreach(var line in File.ReadAllLines(argParts[1]))
                        {
                            AddAppArg(line, appArgs);
                        }
                    }
                }
                else
                {
                    appArgs.Add(arg);
                }
            }
        }
    }
}
