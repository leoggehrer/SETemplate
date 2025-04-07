//@BaseCode
using System.Reflection;

namespace SETemplate.ConApp
{
    internal partial class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(/*string[] args*/)
        {
            string input = string.Empty;
            using Logic.Contracts.IContext context = CreateContext();

            while (!input.Equals("x", StringComparison.CurrentCultureIgnoreCase))
            {
                int index = 1;
                Console.Clear();
                Console.WriteLine("SETemplate");
                Console.WriteLine("==========================================");

                Console.WriteLine($"{nameof(InitDatabase),-25}....{index++}");

                CreateMenu(ref index);

                Console.WriteLine();
                Console.WriteLine($"Exit...............x");
                Console.WriteLine();
                Console.Write("Your choice: ");

                input = Console.ReadLine()!;
                if (Int32.TryParse(input, out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            InitDatabase();
                            Console.WriteLine();
                            Console.Write("Continue with Enter...");
                            Console.ReadLine();
                            break;

                        default:
                            ExecuteMenuItem(choice, context);
                            break;
                    }
                }
            }
        }

        private static Logic.Contracts.IContext CreateContext()
        {

#if ACCOUNT_ON
            Logic.Contracts.IContext? result = null;

            try
            {
                Task.Run(async () =>
                {
                    var login = await Logic.AccountAccess.LogonAsync(AaEmail, AaPwd, string.Empty);

                    result = Logic.DataContext.Factory.CreateContext(login.SessionToken);
                    return result;
                }).Wait();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
            }

            return result ?? Logic.DataContext.Factory.CreateContext();
#else
            return Logic.DataContext.Factory.CreateContext();
#endif
        }

        public static void InitDatabase()
        {
#if DEBUG
            BeforeInitDatabase();
            Logic.DataContext.Factory.InitDatabase();
            AfterInitDatabase();
#endif
        }

        static void AfterInitDatabase()
        {
#if ACCOUNT_ON
            CreateAccounts();
#endif
            ImportData();
        }

        #region partial methods
        static partial void BeforeInitDatabase();
#if ACCOUNT_ON
        static partial void CreateAccounts();
#endif
        static partial void ImportData();
        static partial void CreateMenu(ref int index);
        static partial void ExecuteMenuItem(int choice, Logic.Contracts.IContext context);
        #endregion partial methods
    }
}
