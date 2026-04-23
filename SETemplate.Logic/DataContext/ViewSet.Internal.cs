//@BaseCode
namespace SETemplate.Logic.DataContext
{
    partial class ViewSet<TView>
    {
        #region methods
        /// <summary>
        /// Returns the count of entities in the set.
        /// </summary>
        /// <returns>The count of entities.</returns>
        internal virtual int ExecuteCount()
        {
            return DbSet.Count();
        }
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
