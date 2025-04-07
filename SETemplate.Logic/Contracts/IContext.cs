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
        #endregion methods
    }
}
