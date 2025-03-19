//@BaseCode
using SETemplate.Logic.Contracts;

namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Factory class to create instances of IMusicStoreContext.
    /// </summary>
    public static partial class Factory
    {
        /// <summary>
        /// Creates an instance of IContext.
        /// </summary>
        /// <returns>An instance of IContext.</returns>
        public static IContext CreateContext()
        {
            var result = new ProjectContext();

            return result;
        }

#if DEBUG
        public static void CreateDatabase()
        {
            var context = new ProjectContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public static void InitDatabase()
        {
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
            var context = CreateContext();

            CreateDatabase();

            // Hier koennen Daten importiert werden

#endif
        }
    }
}
