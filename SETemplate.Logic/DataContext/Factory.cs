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
            var result = new ProjectDbContext();

            return result;
        }

#if DEBUG
        public static void CreateDatabase()
        {
            var context = new ProjectDbContext();

            BevoreCreateDatabase(context);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            AfterCreateDatabase(context);
        }


        public static void InitDatabase()
        {
            BeforeInitDatabase();
            CreateDatabase();

            // Hier koennen Daten importiert werden

            AfterInitDatabase();
        }
#endif

        #region partial methods
        static partial void BeforeInitDatabase();
        static partial void AfterInitDatabase();
        static partial void BevoreCreateDatabase(ProjectDbContext context);
        static partial void AfterCreateDatabase(ProjectDbContext context);
        #endregion partial methods
    }
}
