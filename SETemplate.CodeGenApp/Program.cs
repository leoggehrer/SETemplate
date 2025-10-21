//@BaseCode

using TemplateTools.Logic;
using TemplateTools.Logic.Contracts;
using TemplateTools.Logic.Git;

namespace SETemplate.CodeGenApp
{
    /// <summary>
    /// Represents the Program class.
    /// </summary>
    internal partial class Program
    {
        /// <summary>
        /// Initializes the <see cref="Program"/> class.
        /// </summary>
        /// <remarks>
        /// This static constructor is called when the <see cref="Program"/> class is loaded into memory.
        /// It performs necessary initialization tasks before the class can be used.
        /// </remarks>
        static Program()
        {
            ClassConstructing();
            WriteToGroupFile = false;
            WriteInfoHeader = true;
            IncludeCleanDirectory = true;
            ExcludeGeneratedFilesFromGIT = true;
            HomePath = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) 
                                                                           ? Environment.GetEnvironmentVariable("HOME")
                                                                           : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            
            UserPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            SourcePath = GetCurrentSolutionPath();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called before the class is being constructed.
        /// </summary>
        /// <remarks>
        /// This method can be overridden in a partial class to add custom behavior before the class initialization.
        /// </remarks>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the file should be grouped.
        /// </summary>
        private static bool WriteToGroupFile { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the file should be added the info text.
        /// </summary>
        private static bool WriteInfoHeader { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether the empty folders in the source path will be deleted.
        /// </summary>
        private static bool IncludeCleanDirectory { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether generated files are excluded from GIT.
        /// </summary>
        private static bool ExcludeGeneratedFilesFromGIT { get; set; }
        /// <summary>
        /// Gets or sets the home path.
        /// </summary>
        private static string? HomePath { get; set; }
        /// <summary>
        /// Gets or sets the user path.
        /// </summary>
        private static string UserPath { get; set; }
        /// <summary>
        /// Gets or sets the source path for the property.
        /// </summary>
        /// <value>The source path.</value>
        private static string SourcePath { get; set; }
        #endregion Properties
        
        /// <summary>
        /// The entry point of the program.
        /// </summary>
        /// <param name="args">The arguments passed to the program.</param>
        static void Main(string[] args)
        {
            var app = new CodeGeneratorApp();

            app.Run(args);
        }
        #region Console methods
        
        /// <summary>
        /// Executes a command based on the provided input.
        /// </summary>
        /// <param name="command">The command to execute. Should be either 1, 2, 3, or 4.</param>
        /// <returns>Void.</returns>
        private static void ExecuteCommand(string command)
        {
            if (command == "1")
            {
                WriteToGroupFile = !WriteToGroupFile;
            }
            else if (command == "2")
            {
                WriteInfoHeader = !WriteInfoHeader;
            }
            else if (command == "3")
            {
                IncludeCleanDirectory = !IncludeCleanDirectory;
            }
            else if (command == "4")
            {
                ProgressBar.Start();
                ExcludeGeneratedFilesFromGIT = !ExcludeGeneratedFilesFromGIT;
                if (ExcludeGeneratedFilesFromGIT)
                {
                    Console.WriteLine("All generated files are added to gitignore...");
                    GitIgnoreManager.Run(SourcePath);
                }
                else
                {
                    Console.WriteLine("Remove all generated files from gitignore...");
                    GitIgnoreManager.DeleteIgnoreEntries(SourcePath);
                }
            }
            else if (command == "5")    // delete generated files
            {
                ProgressBar.Start();
                Console.WriteLine("Delete all generated files...");
                Generator.DeleteGeneratedFiles(SourcePath);
                if (IncludeCleanDirectory)
                {
                    Console.WriteLine("Delete all empty folders...");
                    Generator.CleanDirectories(SourcePath);
                }
                Console.WriteLine("Delete all generated files ignored from git...");
                GitIgnoreManager.DeleteIgnoreEntries(SourcePath);
            }
            else if (command == "6")    // delete empty folders
            {
                ProgressBar.Start();
                Console.WriteLine("Delete all empty folders...");
                Generator.CleanDirectories(SourcePath);
            }
            else if (command == "7")    // start code generation
            {
                var logicAssemblyTypes = Logic.Modules.CodeGenerator.AssemblyAccess.AllTypes;
                var solutionProperties = SolutionProperties.Create(SourcePath, logicAssemblyTypes);
                IEnumerable<IGeneratedItem>? generatedItems;

                ProgressBar.Start();
                Console.WriteLine("Create code items...");
                generatedItems = Generator.Generate(solutionProperties);

                Console.WriteLine("Delete all generated files...");
                Generator.DeleteGeneratedFiles(SourcePath);
                if (IncludeCleanDirectory)
                {
                    Console.WriteLine("Delete all empty folders...");
                    Generator.CleanDirectories(SourcePath);
                }
                Console.WriteLine("Write code items to files...");
                Writer.WriteToGroupFile = WriteToGroupFile;
                Writer.WriteInfoHeader = WriteInfoHeader;
                Writer.WriteAll(SourcePath, solutionProperties, generatedItems);
                if (ExcludeGeneratedFilesFromGIT)
                {
                    Console.WriteLine("All generated files are added to gitignore...");
                    GitIgnoreManager.Run(SourcePath);
                }
                else
                {
                    Console.WriteLine("Remove all generated files from gitignore...");
                    GitIgnoreManager.DeleteIgnoreEntries(SourcePath);
                }
                Thread.Sleep(700);
            }
            ProgressBar.Stop();
        }
        #endregion Console methods
        
        #region Helpers
        /// <summary>
        /// Returns the current solution path.
        /// </summary>
        /// <returns>The path of the solution file without the extension, or an empty string if the file does not exist.</returns>
        private static string GetCurrentSolutionPath()
        {
            var codeGenApp = $"{nameof(SETemplate)}.{nameof(CodeGenApp)}";
            var endPos = AppContext.BaseDirectory.IndexOf(codeGenApp, StringComparison.CurrentCultureIgnoreCase);
            
            return AppContext.BaseDirectory[..endPos];
        }
        #endregion Helpers
        
        #region Partial methods
        /// <summary>
        /// This method is called before retrieving the target paths. It can be used to perform any necessary operations or checks.
        /// </summary>
        /// <param name="sourcePath">The source path from where the target paths will be retrieved.</param>
        /// <param name="targetPaths">The list of target paths to be retrieved.</param>
        /// <param name="handled">A reference boolean value indicating whether the operation has been handled.</param>
        static partial void BeforeGetTargetPaths(string sourcePath, List<string> targetPaths, ref bool handled);
        /// <summary>
        /// Method called after getting the target paths.
        /// </summary>
        /// <param name="sourcePath">The path of the source.</param>
        /// <param name="targetPaths">The list of target paths.</param>
        static partial void AfterGetTargetPaths(string sourcePath, List<string> targetPaths);
        #endregion Partial methods
    }
}

