//@BaseCode
namespace SETemplate.Logic.DataContext
{
    partial class ViewSet<TView>
    {
        #region overridables
        /// <summary>
        /// Copies properties from the source entity to the target entity.
        /// </summary>
        /// <param name="target">The target entity.</param>
        /// <param name="source">The source entity.</param>
        protected abstract void CopyProperties(TView target, TView source);
        #endregion overridables

        #region methods
        /// <summary>
        /// Returns the count of entities in the set.
        /// </summary>
        /// <returns>The count of entities.</returns>
        internal virtual int ExecuteCount()
        {
            return DbSet.Count();
        }

        /// <summary>
        /// Returns the count of entities in the set asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the count of entities.</returns>
        internal virtual Task<int> ExecuteCountAsync()
        {
            return DbSet.CountAsync();
        }

        /// <summary>
        /// Gets the queryable set of entities.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities.</returns>
        internal virtual IQueryable<TView> ExecuteAsQuerySet() => DbSet.AsQueryable();

        /// <summary>
        /// Gets the no-tracking queryable set of entities.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.</returns>
        internal virtual IQueryable<TView> ExecuteAsNoTrackingSet() => ExecuteAsQuerySet().AsNoTracking();
        #endregion methods

        #region context methods
        /// <summary>
        /// Saves all changes made in the context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        internal virtual int ExecuteSaveChanges()
        {
            return Context.ExecuteSaveChanges();
        }

        /// <summary>
        /// Asynchronously saves all changes made in the context to the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        internal virtual Task<int> ExecuteSaveChangesAsync()
        {
            return Context.ExecuteSaveChangesAsync();
        }
        #endregion context methods
    }
}
