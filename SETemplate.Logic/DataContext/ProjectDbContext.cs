//@BaseCode
using SETemplate.Logic.Contracts;

namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Represents the database context for the SETemplate application.
    /// </summary>
    internal sealed partial class ProjectDbContext : DbContext, IContext
    {
        #region fields
        /// <summary>
        /// The type of the database (e.g., "Sqlite", "SqlServer").
        /// </summary>
        private static readonly string DatabaseType = "Sqlite";

        /// <summary>
        /// The connection string for the database.
        /// </summary>
        private static readonly string ConnectionString = "data source=SETemplate.db";
        #endregion fields

        /// <summary>
        /// Initializes static members of the <see cref="ProjectDbContext"/> class.
        /// </summary>
        static ProjectDbContext()
        {
            var appSettings = Common.Modules.Configuration.AppSettings.Instance;

            ClassConstructing();

#if POSTGRES_ON
            DatabaseType = "Postgres";
#endif

#if SQLSERVER_ON
            DatabaseType = "SqlServer";
#endif

#if SQLITE_ON
            DatabaseType = "Sqlite";
#endif

            ConnectionString = appSettings[$"ConnectionStrings:{DatabaseType}ConnectionString"] ?? ConnectionString;
            ClassConstructed();
        }
        /// <summary>
        /// This method is called before the construction of the class.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        static partial void ClassConstructed();

        #region properties
        #endregion properties

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDbContext"/> class (created by the generator.)
        /// </summary>
        public ProjectDbContext()
        {
            Constructing();
            Constructed();
        }
        /// <summary>
        /// This method is called the object is being constraucted.
        /// </summary>
        partial void Constructing();
        /// <summary>
        /// This method is called when the object is constructed.
        /// </summary>
        partial void Constructed();
        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database.</returns>
        public override int SaveChanges()
        {
            return ExecuteSaveChanges();
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the underlying database.</returns>
        public Task<int> SaveChangesAsync()
        {
            return ExecuteSaveChangesAsync();
        }
        /// <summary>
        /// Configures the database context options.
        /// </summary>
        /// <param name="optionsBuilder">The options builder to be used for configuration.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if POSTGRES_ON
            optionsBuilder.UseNpgsql(ConnectionString);
#endif

#if SQLSERVER_ON
            optionsBuilder.UseSqlServer(ConnectionString);
#endif

#if SQLITE_ON
            optionsBuilder.UseSqlite(ConnectionString);
#endif

            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Determines the DbSet depending on the type E
        /// </summary>
        /// <typeparam name="E">The entity type E</typeparam>
        /// <returns>The DbSet depending on the type E</returns>
        internal DbSet<E> GetDbSet<E>() where E : Entities.EntityObject
        {
            var handled = false;
            var result = default(DbSet<E>);

            GetDbSet(ref result, ref handled);
            if (handled == false || result == null)
            {
                GetGeneratorDbSet(ref result, ref handled);
            }
            return result ?? Set<E>();
        }

        internal EntitySet<E>? GetEntitySet<E>() where E : Entities.EntityObject, new()
        {
            var handled = false;
            var result = default(EntitySet<E>);

            GetEntitySet(ref result, ref handled);
            if (handled == false || result == null)
            {
                GetGeneratorEntitySet(ref result, ref handled);
            }
            return result;
        }
        #endregion methods

        #region partial methods
        /// <summary>
        /// Determines the domain project DbSet depending on the type E
        /// </summary>
        /// <typeparam name="E">The entity type E</typeparam>
        /// <param name="dbSet">The DbSet depending on the type E</param>
        /// <param name="handled">Indicates whether the method found the DbSet</param>
        partial void GetDbSet<E>(ref DbSet<E>? dbSet, ref bool handled) where E : Entities.EntityObject;
        /// <summary>
        /// Determines the domain project DbSet depending on the type E
        /// </summary>
        /// <typeparam name="E">The entity type E</typeparam>
        /// <param name="dbSet">The DbSet depending on the type E</param>
        /// <param name="handled">Indicates whether the method found the DbSet</param>
        partial void GetGeneratorDbSet<E>(ref DbSet<E>? dbSet, ref bool handled) where E : Entities.EntityObject;

        /// <summary>
        /// Determines the domain project EntitySet depending on the type E
        /// </summary>
        /// <typeparam name="E">The entity type E</typeparam>
        /// <param name="entitySet">The EntitySet depending on the type E</param>
        /// <param name="handled">Indicates whether the method found the EntitySet</param>
        partial void GetEntitySet<E>(ref EntitySet<E>? entitySet, ref bool handled) where E : Entities.EntityObject, new();
        /// <summary>
        /// Determines the domain project DbSet depending on the type E
        /// </summary>
        /// <typeparam name="E">The entity type E</typeparam>
        /// <param name="entitySet">The EntitySet depending on the type E</param>
        /// <param name="handled">Indicates whether the method found the EntitySet</param>
        partial void GetGeneratorEntitySet<E>(ref EntitySet<E>? entitySet, ref bool handled) where E : Entities.EntityObject, new();
        #endregion partial methods
    }
}
