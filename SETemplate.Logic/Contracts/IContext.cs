//@BaseCode
namespace SETemplate.Logic.Contracts
{
    /// <summary>
    /// Represents a context that provides access to entity sets and supports saving changes.
    /// </summary>
    public interface IContext : IDisposable
    {
        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database.</returns>
        int SaveChanges();
    }
}
