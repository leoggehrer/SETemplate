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
        /// Returns an <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities.</returns>
        IQueryable<TEntity> AsQuerySet();

        /// <summary>
        /// Returns an <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that can be used to query the set of entities without tracking changes.</returns>
        IQueryable<TEntity> AsNoTrackingSet();

        /// <summary>
        /// Returns the element of type T with the identification of id.
        /// </summary>
        /// <param name = "id">The identification.</param>
        /// <returns>The element of the type T with the corresponding identification.</returns>
        ValueTask<TEntity?> GetByIdAsync(IdType id);

        /// <summary>
        /// Adds a new entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        TEntity Add(TEntity entity);

        /// <summary>
        /// Adds a range of entities to the set.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        void AddRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Asynchronously adds a new entity to the set.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
        Task<TEntity> AddAsync(TEntity entity);

        /// <summary>
        /// Asynchronously adds a range of entities to the set.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates an entity in the set by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="entity">The updated entity.</param>
        /// <returns>The updated entity, or null if the entity was not found.</returns>
        TEntity? Update(IdType id, TEntity entity);

        /// <summary>
        /// Asynchronously updates an entity in the set by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="entity">The updated entity.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity, or null if the entity was not found.</returns>
        Task<TEntity?> UpdateAsync(IdType id, TEntity entity);

        /// <summary>
        /// Removes an entity from the set by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to remove.</param>
        /// <returns>The removed entity, or null if the entity was not found.</returns>
        TEntity? Remove(IdType id);

        /// <summary>
        /// Disposes the entity set.
        /// </summary>
        void Dispose();
    }
}
