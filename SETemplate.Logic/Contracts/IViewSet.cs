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
        /// Retrieves all entities from the set without tracking changes.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a collection of entities limited to <see cref="MaxCount"/>.
        /// </returns>
        /// <remarks>
        /// This method queries entities without change tracking for better performance when read-only access is needed.
        /// The results are automatically limited to the maximum count defined by <see cref="MaxCount"/> to prevent excessive data retrieval.
        /// </remarks>
        Task<IEnumerable<TView>> GetAllAsync();

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
        Task<IEnumerable<TView>> QueryAsync(Models.QueryParams queryParams);

        /// <summary>
        /// Disposes the entity set.
        /// </summary>
        void Dispose();
    }
}
