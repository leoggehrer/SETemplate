//@BaseCode
using SETemplate.Logic.Entities;

namespace SETemplate.Logic.Contracts
{
    /// <summary>
    /// Interface for a set of entities.
    /// </summary>
    /// <typeparam name="TView">The type of the entity.</typeparam>
    public partial interface IViewSet<TView> where TView : ViewObject, new()
    {
        /// <summary>
        /// Gets the count of entities in the set.
        /// </summary>
        /// <returns>The count of entities in the set.</returns>
        int Count();

        /// <summary>
        /// Asynchronously gets the count of entities in the set.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the count of entities in the set.</returns>
        Task<int> CountAsync();

        /// <summary>
        /// Returns an <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.</returns>
        IQueryable<TView> AsNoTrackingSet();

        /// <summary>
        /// Disposes the entity set.
        /// </summary>
        void Dispose();
    }
}
