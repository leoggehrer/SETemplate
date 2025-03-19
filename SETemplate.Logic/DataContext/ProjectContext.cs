//@BaseCode
using SETemplate.Logic.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Represents the database context for the SETemplate application.
    /// </summary>
    internal partial class ProjectContext : DbContext, IContext
    {
        #region fields
        /// <summary>
        /// The type of the database (e.g., "Sqlite", "SqlServer").
        /// </summary>
        private static string DatabaseType = "Sqlite";

        /// <summary>
        /// The connection string for the database.
        /// </summary>
        private static string ConnectionString = "data source=SETemplate.db";
        #endregion fields

        /// <summary>
        /// Initializes static members of the <see cref="ProjectContext"/> class.
        /// </summary>
        static ProjectContext()
        {
            var appSettings = Common.Modules.Configuration.AppSettings.Instance;

            DatabaseType = appSettings["Database:Type"] ?? DatabaseType;
            ConnectionString = appSettings[$"ConnectionStrings:{DatabaseType}ConnectionString"] ?? ConnectionString;
        }

        #region properties
        #endregion properties

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectContext"/> class.
        /// </summary>
        public ProjectContext()
        {
        }
        #endregion constructors

        #region methods
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

            base.OnConfiguring(optionsBuilder);
        }
        #endregion methods
    }
}
