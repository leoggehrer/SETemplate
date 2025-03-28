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
            DatabaseType = appSettings["Database:Type"] ?? DatabaseType;
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
        #endregion constructors

        #region methods
        public override int SaveChanges()
        {
            // Vor dem Speichern alle Entitäten validieren
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IValidatable && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var validatableEntity = (IValidatable)entry.Entity;
                
                validatableEntity.Validate(this);
            }

            return base.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            // Vor dem Speichern alle Entitäten validieren
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IValidatable && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var validatableEntity = (IValidatable)entry.Entity;

                validatableEntity.Validate(this);
            }

            return base.SaveChangesAsync();
        }
        /// <summary>
        /// Configures the database context options.
        /// </summary>
        /// <param name="optionsBuilder">The options builder to be used for configuration.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (DatabaseType == "Sqlite")
            {
                optionsBuilder.UseSqlite(ConnectionString);
            }
            else if (DatabaseType == "SqlServer")
            {
                optionsBuilder.UseSqlServer(ConnectionString);
            }
            else if (DatabaseType == "Postgres")
            {
                optionsBuilder.UseNpgsql(ConnectionString);
            }

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
        #endregion partial methods
    }
}
