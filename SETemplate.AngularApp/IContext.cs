//@BaseCode
namespace SETemplate.Logic.Contracts
{
    /// <summary>
    /// Represents a context that provides access to entity sets and supports saving changes.
    /// </summary>
    public partial interface IContext : IDisposable
    {
        #region methods
        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database.</returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the underlying database.</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Rejects all changes made in this context, reverting entities to their original state.
        /// </summary>
        /// <returns>The number of state entries reverted to their original state.</returns>
        int RejectChanges();

        /// <summary>
        /// Asynchronously rejects all changes made in this context, reverting entities to their original state.
        /// </summary>
        /// <returns>A task that represents the asynchronous reject operation. The task result contains the number of state entries reverted to their original state.</returns>
        Task<int> RejectChangesAsync();
        #endregion methods
    }
}
