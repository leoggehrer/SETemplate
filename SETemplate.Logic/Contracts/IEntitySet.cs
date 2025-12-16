//@BaseCode
using SETemplate.Logic.Entities;

namespace SETemplate.Logic.Contracts
{
    /// <summary>
    /// Interface for a set of entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public partial interface IEntitySet<TEntity> where TEntity : EntityObject, new()
    {
        int MaxCount => 500;
        /// <summary>
        /// Creates a new instance of the entity.
        /// </summary>
        /// <returns>A new instance of the entity.</returns>
        TEntity Create();

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
        Task<IEnumerable<TEntity>> GetAllAsync();

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
        Task<IEnumerable<TEntity>> QueryAsync(Models.QueryParams queryParams);

        /// <summary>
        /// Asynchronously adds a new entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
        Task<TEntity> AddAsync(TEntity entity);

        /// <summary>
        /// Asynchronously adds a range of new entities to the set.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added entities.</returns>
        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Disposes the entity set.
        /// </summary>
        void Dispose();
    }
}
