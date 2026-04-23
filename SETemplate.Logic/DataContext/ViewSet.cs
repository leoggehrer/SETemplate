//@BaseCode
using SETemplate.Logic.Contracts;
using System.Reflection;

namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Represents a set of view entities that can be queried from a database (read-only).
    /// </summary>
    /// <typeparam name="TView">The type of the view entity.</typeparam>
    internal abstract partial class ViewSet<TView> : SetBase<TView>, IViewSet<TView>
        where TView : Entities.ViewObject, new()
    {
        #region constructors
        /// <summary>
        /// Initializes a new instance of the ViewSet class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="dbSet">The set of view entities.</param>
        internal ViewSet(ProjectDbContext context, DbSet<TView> dbSet)
            : base(context, dbSet)
        {
        }
        #endregion constructors

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
        public virtual Task<IEnumerable<TView>> GetAllAsync()
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteGetAllAsync();
        }

        /// <summary>
        /// Queries entities from the set based on the provided query parameters.
        /// </summary>
        /// <param name="queryParams">The query parameters containing filter, values, and includes.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of entities matching the query criteria.</returns>
        public virtual Task<IEnumerable<TView>> QueryAsync(Models.QueryParams queryParams)
        {
            BeforeAccessing(MethodBase.GetCurrentMethod()!.GetAsyncOriginal());

            return ExecuteQueryAsync(queryParams);
        }
        #endregion methods

        #region partial methods
        /// <summary>
        /// Method that is called before accessing any method in the ViewSet class.
        /// </summary>
        /// <param name="methodBase">The method that is being accessed.</param>
        partial void BeforeAccessing(MethodBase methodBase);
        #endregion partial methods
    }
}
