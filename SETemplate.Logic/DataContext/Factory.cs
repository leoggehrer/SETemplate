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
            using var context = new ProjectDbContext();

            BevoreCreateDatabase(context);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            AfterCreateDatabase(context);
        }

        /// <summary>
        /// Resets the database by dropping and recreating it from scratch.
        /// </summary>
        /// <remarks>
        /// This method is intended for use in development environments only — it is destructive
        /// and deletes all existing data. It provides hooks for custom logic before and after the reset.
        /// Data import can also be performed within this method.
        /// </remarks>
        public static void ResetDatabase()
        {
            BeforeResetDatabase();
            CreateDatabase();

            // Hier koennen Daten importiert werden
            AfterResetDatabase();
        }
#endif

#if DBOPERATION_ON
        /// <summary>
        /// Applies all pending Entity Framework Core migrations to the database.
        /// </summary>
        /// <remarks>
        /// This method is safe for use in production environments. Unlike <see cref="ResetDatabase"/>,
        /// it does not delete existing data — it only applies migrations that have not yet been executed.
        /// If the database does not exist, it will be created based on the migration history.
        /// </remarks>
        public static void MigrateDatabase()
        {
            using var context = new ProjectDbContext();

            BeforeMigrateDatabase(context);
            context.Database.Migrate();
            AfterMigrateDatabase(context);
        }
#endif

        #region partial methods
        static partial void BeforeResetDatabase();
        static partial void AfterResetDatabase();
        static partial void BevoreCreateDatabase(ProjectDbContext context);
        static partial void AfterCreateDatabase(ProjectDbContext context);
        static partial void BeforeMigrateDatabase(ProjectDbContext context);
        static partial void AfterMigrateDatabase(ProjectDbContext context);
        #endregion partial methods
    }
}
