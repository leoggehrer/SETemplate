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
        /// Gets the queryable set of entities.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities.</returns>
        public virtual IQueryable<TView> AsQuerySet()
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!);

            return ExecuteAsQuerySet();
        }

        /// <summary>
        /// Returns an <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.</returns>
        public virtual IQueryable<TView> AsNoTrackingSet()
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!);

            return ExecuteAsNoTrackingSet();
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
