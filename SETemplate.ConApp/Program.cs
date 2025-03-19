//@BaseCode
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
            using Logic.Contracts.IContext context = Logic.DataContext.Factory.CreateContext();

            while (!input.Equals("x", StringComparison.CurrentCultureIgnoreCase))
            {
                int index = 1;
                Console.Clear();
                Console.WriteLine("SETemplate");
                Console.WriteLine("==========================================");

                Console.WriteLine($"{nameof(InitDatabase),-25}....{index++}");

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
                            break;
                    }
                }
            }
        }

        public static void InitDatabase()
        {
#if DEBUG
            Logic.DataContext.Factory.InitDatabase();
#endif
        }

    }
}
