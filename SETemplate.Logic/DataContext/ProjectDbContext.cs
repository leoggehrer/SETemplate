//@BaseCode
using SETemplate.Common.Modules.Exceptions;
using SETemplate.Logic.Contracts;
using System.Reflection;

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
        /// This method is called when the object is constructed.
        /// </summary>
        partial void Constructing();
        /// <summary>
        /// This method is called after the object has been initialized.
        /// </summary>
        partial void Constructed();
        #endregion constructors

        #region methods
        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database.</returns>
        public override int SaveChanges()
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!);

            return ExecuteSaveChanges();
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the underlying database.</returns>
        public Task<int> SaveChangesAsync()
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteSaveChangesAsync();
        }
        /// <summary>
        /// Configures the database context options.
        /// </summary>
        /// <param name="optionsBuilder">The options builder to be used for configuration.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var handled = false;

            BeforeOnConfiguring(optionsBuilder, ref handled);
            if (handled == false)
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
            }
            AfterOnConfiguring(optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Discards all changes in the current context.
        /// </summary>
        /// <returns>Number of changed entities.</returns>
        public int RejectChanges()
        {
            int count = 0;

            foreach (var entry in ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged).ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        count++;
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        count++;
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        count++;
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
            return count;
        }

        /// <summary>
        /// Discards all changes in the current context.
        /// </summary>
        /// <returns>Number of changed entities.</returns>
        public Task<int> RejectChangesAsync()
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return Task.Run(() => RejectChanges());
        }

        /// <summary>
        /// Configures the model for the database context.
        /// </summary>
        /// <param name="modelBuilder">The builder used to define the model for the context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var handled = false;

            OnViewModelCreating(modelBuilder);
            BeforeOnModelCreating(modelBuilder, ref handled);
            if (handled == false)
            {
                foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                {
                    relationship.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }
            AfterOnModelCreating(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Determines the DbSet depending on the type E
        /// </summary>
        /// <typeparam name="E">The entity type E</typeparam>
        /// <returns>The DbSet depending on the type E</returns>
        internal DbSet<E> GetDbSet<E>() where E : Entities.DbObject
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

        /// <summary>
        /// Determines the domain project EntitySet depending on the type E
        /// </summary>
        /// <typeparam name="E">The entity type E</typeparam>
        /// <returns>The EntitySet depending on the type E</returns>
        internal EntitySet<E> GetEntitySet<E>() where E : Entities.EntityObject, new()
        {
            var handled = false;
            var result = default(EntitySet<E>);

            HandleGetEntitySet(ref result, ref handled);
            if (handled == false || result == null)
            {
                GetGeneratorEntitySet(ref result, ref handled);
            }

            if (result != null)
            {
            }
            return result ?? throw new Modules.Exceptions.LogicException(ErrorType.InvalidEntitySet);  
        }

        internal ViewSet<E> GetViewSet<E>() where E : Entities.ViewObject, new()
        {
            var handled = false;
            var result = default(ViewSet<E>);
            HandleGetViewSet(ref result, ref handled);
            if (handled == false || result == null)
            {
                GetGeneratorViewSet(ref result, ref handled);
            }
            return result ?? throw new Modules.Exceptions.LogicException(ErrorType.InvalidEntitySet);
        }
        #endregion methods

        #region partial methods
        /// <summary>
        /// This method is called before configuring the database context options.
        /// </summary>
        /// <param name="optionsBuilder">The options builder to be used for configuration.</param>
        /// <param name="handled">Indicates whether the configuration was handled.</param>
        static partial void BeforeOnConfiguring(DbContextOptionsBuilder optionsBuilder, ref bool handled);
        /// <summary>
        /// This method is called after configuring the database context options.
        /// </summary>
        /// <param name="optionsBuilder">The options builder to be used for configuration.</param>
        static partial void AfterOnConfiguring(DbContextOptionsBuilder optionsBuilder);

        /// <summary>
        /// This method is called during the creation of the model for the database context.
        /// It allows customization of the model by adding configurations, constraints, or relationships
        /// between entities before the model is finalized.
        /// </summary>
        /// <param name="modelBuilder">The builder used to define the model for the context.</param>
        static partial void OnViewModelCreating(ModelBuilder modelBuilder);

        /// <summary>
        /// This method is called before the model for a derived context has been initialized.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <param name="handled">Indicates whether the method handled the model creation process.</param>
        static partial void BeforeOnModelCreating(ModelBuilder modelBuilder, ref bool handled);
        /// <summary>
        /// This method is called after the model for a derived context has been initialized.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        static partial void AfterOnModelCreating(ModelBuilder modelBuilder);

        /// <summary>
        /// This method is called before accessing a method.
        /// </summary>
        /// <param name="methodBase">The method being accessed.</param>
        partial void BeforeAccessing(MethodBase methodBase);

        /// <summary>
        /// Determines the domain project DbSet depending on the type E
        /// </summary>
        /// <typeparam name="E">The entity type E</typeparam>
        /// <param name="dbSet">The DbSet depending on the type E</param>
        /// <param name="handled">Indicates whether the method found the DbSet</param>
        partial void GetDbSet<E>(ref DbSet<E>? dbSet, ref bool handled) where E : Entities.DbObject;
        /// <summary>
        /// Determines the domain project DbSet depending on the type E
        /// </summary>
        /// <typeparam name="E">The entity type E</typeparam>
        /// <param name="dbSet">The DbSet depending on the type E</param>
        /// <param name="handled">Indicates whether the method found the DbSet</param>
        partial void GetGeneratorDbSet<E>(ref DbSet<E>? dbSet, ref bool handled) where E : Entities.DbObject;

        /// <summary>
        /// Determines the domain project EntitySet depending on the type E
        /// </summary>
        /// <typeparam name="E">The entity type E</typeparam>
        /// <param name="entitySet">The EntitySet depending on the type E</param>
        /// <param name="handled">Indicates whether the method found the EntitySet</param>
        partial void HandleGetEntitySet<E>(ref EntitySet<E>? entitySet, ref bool handled) where E : Entities.EntityObject, new();
        /// <summary>
        /// Determines the domain project DbSet depending on the type E
        /// </summary>
        /// <typeparam name="E">The entity type E</typeparam>
        /// <param name="entitySet">The EntitySet depending on the type E</param>
        /// <param name="handled">Indicates whether the method found the EntitySet</param>
        partial void GetGeneratorEntitySet<E>(ref EntitySet<E>? entitySet, ref bool handled) where E : Entities.EntityObject, new();

        /// <summary>
        /// Determines the domain project ViewSet depending on the type E.
        /// </summary>
        /// <typeparam name="E">The view object type E.</typeparam>
        /// <param name="viewSet">The ViewSet depending on the type E.</param>
        /// <param name="handled">Indicates whether the method found the ViewSet.</param>
        partial void HandleGetViewSet<E>(ref ViewSet<E>? viewSet, ref bool handled) where E : Entities.ViewObject, new();
        /// <summary>
        /// Determines the domain project ViewSet depending on the type E.
        /// </summary>
        /// <typeparam name="E">The view object type E.</typeparam>
        /// <param name="viewSet">The ViewSet depending on the type E.</param>
        /// <param name="handled">Indicates whether the method found the ViewSet.</param>
        partial void GetGeneratorViewSet<E>(ref ViewSet<E>? viewSet, ref bool handled) where E : Entities.ViewObject, new();
        #endregion partial methods
    }
}
