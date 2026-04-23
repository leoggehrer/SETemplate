//@BaseCode
using SETemplate.Common.Modules.Template;

namespace SETemplate.ConApp.Apps
{
    /// <summary>
    /// Represents the main application class for the ToolsApp.
    /// </summary>
    public partial class StarterApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="Program"/> class.
        /// This static constructor sets up the necessary properties for the program.
        /// </summary>
        static StarterApp()
        {
            ClassConstructing();
            SourcePath = SolutionPath = TemplatePath.GetSolutionPathByExecution();
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
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        public StarterApp()
        {
            Constructing();
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
        /// <summary>
        /// Gets or sets the application arguments that are passed to sub-applications.
        /// </summary>
        private string[] AppArgs { get; set; } = [];
        #endregion properties

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
#if DEBUG && DEVELOP_ON && DBOPERATION_ON
                new()
                {
                    Key = "----",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },

                new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "reset_db",
                    Text = ToLabelText($"{nameof(ResetDatabase).ToCamelCaseSplit()}", "Drop and recreate the database (destructive, dev only)"),
                    Action = (self) =>
                    {
                        ResetDatabase();
                    },
                },
#endif

#if DBOPERATION_ON
                new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "migrate_db",
                    Text = ToLabelText($"{nameof(MigrateDatabase).ToCamelCaseSplit()}", "Apply pending EF Core migrations (production-safe)"),
                    Action = (self) =>
                    {
                        MigrateDatabase();
                    },
                },
#endif
            };
            CreateImportMenuItems(ref mnuIdx, menuItems);

#if ACCOUNT_ON
            menuItems.Add(
                new()
                {
                    Key = "----",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                });

            menuItems.Add(
                new()
                {
                    Key = $"{++mnuIdx}",
                    OptionalKey = "create_acc",
                    Text = ToLabelText($"{nameof(CreateAccount).ToCamelCaseSplit()}", "Started the creation of account"),
                    Action = (self) =>
                    {
                        PrintHeader();
                        CreateAccount();
                    },
                });
#endif
            return [.. menuItems.Union(CreateExitMenuItems())];
        }

        /// <summary>
        /// Prints the header for the application.
        /// </summary>
        protected override void PrintHeader()
        {
            List<KeyValuePair<string, object>> headerParams = [new("Solution path:", SolutionPath)];

            base.PrintHeader($"{nameof(SETemplate)} Starter", [.. headerParams]);
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
                if (arg.Key.Equals(nameof(HomePath), StringComparison.OrdinalIgnoreCase))
                {
                    HomePath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(UserPath), StringComparison.OrdinalIgnoreCase))
                {
                    UserPath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(SourcePath), StringComparison.OrdinalIgnoreCase))
                {
                    SourcePath = arg.Value;
                }
                else if (arg.Key.Equals(nameof(SolutionPath), StringComparison.OrdinalIgnoreCase))
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
            base.BeforeRun(AppArgs);
        }
        protected override void AfterRun()
        {
            PrintHeader();

            ConsoleColor foregroundColor = ForegroundColor;
            ForegroundColor = ConsoleColor.Green;
            PrintLine("Application finished successfully.");
            ForegroundColor = foregroundColor;

            base.AfterRun();
        }
        #endregion overrides

        #region app methods
        private void ResetDatabase()
        {
#if DEBUG && DEVELOP_ON && DBOPERATION_ON
            PrintHeader();
            StartProgressBar();
            PrintLine("Reset database...");

            BeforeResetDatabase();
            try
            {
                Logic.DataContext.Factory.ResetDatabase();
            }
            catch (Exception ex)
            {
                PrintLine($"Error resetting database: {ex.Message}");
                ReadLine("Press ENTER to continue...");
            }
            AfterResetDatabase();

#if ACCOUNT_ON
            PrintLine("Create accounts...");
            CreateAccounts();
#endif
            StopProgressBar();
#endif
        }
        private void MigrateDatabase()
        {
#if DBOPERATION_ON
            PrintHeader();
            StartProgressBar();
            PrintLine("Migrate database...");

            BeforeMigrateDatabase();
            try
            {
                Logic.DataContext.Factory.MigrateDatabase();
                PrintLine("Migration completed successfully.");
            }
            catch (Exception ex)
            {
                PrintLine($"Error migrating database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    PrintLine($"Inner exception: {ex.InnerException.Message}");
                }
                ReadLine("Press ENTER to continue...");
            }
            AfterMigrateDatabase();

            StopProgressBar();
#endif
        }
        private static Logic.Contracts.IContext CreateContext()
        {

#if ACCOUNT_ON
            Logic.Contracts.IContext? result = null;

            try
            {
                Task.Run(async () =>
                {
                    var login = await Logic.AccountAccess.LoginAsync(SAEmail, SAPwd, string.Empty);

                    result = Logic.DataContext.Factory.CreateContext(login.SessionToken);
                    return result;
                }).Wait();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in {System.Reflection.MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }

            return result ?? Logic.DataContext.Factory.CreateContext();
#else
            return Logic.DataContext.Factory.CreateContext();
#endif
        }
        #endregion app methods

        #region partial methods
        partial void BeforeResetDatabase();
        partial void AfterResetDatabase();
        partial void BeforeMigrateDatabase();
        partial void AfterMigrateDatabase();
        partial void CreateImportMenuItems(ref int menuIdx, List<MenuItem> menuItems);

#if ACCOUNT_ON
        static partial void CreateAccount();
        static partial void CreateAccounts();
#endif
        #endregion partial methods
    }
}

