//@BaseCode
using SETemplate.Logic.Contracts;
using System.Reflection;

namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Represents a set of entities that can be queried from a database and provides methods to manipulate them.
    /// </summary>
    /// <typeparam name="TView">The type of the entity.</typeparam>
    /// <param name="context">The database context.</param>
    /// <param name="dbSet">The set of entities.</param>
    internal abstract partial class ViewSet<TView>(ProjectDbContext context, DbSet<TView> dbSet) : IViewSet<TView>, IDisposable
        where TView : Entities.ViewObject, new()
    {
        #region fields
        private ProjectDbContext? _context = context;
        private DbSet<TView>? _dbSet = dbSet;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets the database context.
        /// </summary>
        internal ProjectDbContext Context => _context!;
        /// <summary>
        /// Gets the database context.
        /// </summary>
        protected DbSet<TView> DbSet => _dbSet!;
        #endregion properties

        #region methods
        /// <summary>
        /// Returns the count of entities in the set.
        /// </summary>
        /// <returns>The count of entities.</returns>
        public virtual int Count()
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!);

            return ExecuteCount();
        }

        /// <summary>
        /// Returns the count of entities in the set asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the count of entities.</returns>
        public virtual Task<int> CountAsync()
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteCountAsync();
        }

        /// <summary>
        /// Retrieves all entities from the set without tracking changes.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a collection of entities limited to <see cref="MaxCount"/>.
        /// </returns>
        /// <remarks>
        /// This method queries entities without change tracking for better performance when read-only access is needed.
        /// The results are automatically limited to the maximum count defined by <see cref="MaxCount"/> to prevent excessive data retrieval.
        /// </remarks>
        public virtual Task<IEnumerable<TView>> GetAsync()
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteGetAsync();
        }

        /// <summary>
        /// Queries entities from the set based on the provided query parameters.
        /// </summary>
        /// <param name="queryParams">The query parameters containing filter, values, and includes.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of entities matching the query criteria.</returns>
        /// <exception cref="Modules.Exceptions.LogicException">Thrown when the filter expression is empty or invalid.</exception>
        /// <remarks>
        /// The query will include the specified navigation properties and apply the provided filter expression.
        /// Results are limited to the <see cref="MaxCount"/> value to prevent excessive data retrieval.
        /// </remarks>
        public virtual Task<IEnumerable<TView>> QueryAsync(Models.QueryParams queryParams)
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteQueryAsync(queryParams);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _dbSet = null;
            _context = null;
            GC.SuppressFinalize(this);
        }
        #endregion methods

        #region partial methods
        /// <summary>
        /// Method that is called before accessing any method in the EntitySet class.
        /// </summary>
        /// <param name="methodBase">The method that is being accessed.</param>
        partial void BeforeAccessing(MethodBase methodBase);
        #endregion partial methods
    }
}
