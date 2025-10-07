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
        /// Returns an <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.</returns>
        IQueryable<TEntity> AsNoTrackingSet();

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
