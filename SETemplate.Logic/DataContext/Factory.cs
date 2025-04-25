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

#if DEBUG && DEVELOP_ON && DBOPERATION_ON
        /// <summary>
        /// Creates the database by ensuring it is deleted and then created anew.
        /// </summary>
        /// <remarks>
        /// This method is intended for use in development environments where the database schema
        /// needs to be reset. It calls partial methods <see cref="BevoreCreateDatabase(ProjectDbContext)"/> 
        /// and <see cref="AfterCreateDatabase(ProjectDbContext)"/> to allow for custom logic before and after 
        /// the database creation process.
        /// </remarks>
        public static void CreateDatabase()
        {
            var context = new ProjectDbContext();

            BevoreCreateDatabase(context);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            AfterCreateDatabase(context);
        }

        /// <summary>
        /// Initializes the database by creating it and allowing for data import.
        /// </summary>
        /// <remarks>
        /// This method is intended for use in development environments. It ensures the database is created
        /// and provides hooks for custom logic before and after initialization. Data import can also be performed
        /// within this method.
        /// </remarks>
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
